using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    public partial class DashboardRecepcionistaViewModel : BaseViewModel
    {
        private readonly IContratoRepository _contratoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly SesionService _sesionService;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        private string _bienvenida = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Contrato> _pagosPendientes = new();

        [ObservableProperty]
        private ObservableCollection<Contrato> _contratosProximosVencer = new();

        [ObservableProperty]
        private int _totalPendientes;

        [ObservableProperty]
        private int _totalProximosVencer;

        public DashboardRecepcionistaViewModel(
            IContratoRepository contratoRepo,
            IPagoRepository pagoRepo,
            SesionService sesionService,
            NavigationService navigationService)
        {
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _sesionService = sesionService;
            _navigationService = navigationService;

            Titulo = "Dashboard — Recepcionista";
        }

        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                if (_sesionService.UsuarioActual != null)
                    Bienvenida = $"Bienvenido, {_sesionService.UsuarioActual.NombreUsuario}";

                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;
                DateTime hoy = DateTime.Now;
                DateTime en30Dias = hoy.AddDays(30);

                PagosPendientes.Clear();
                ContratosProximosVencer.Clear();

                var contratosActivos = _contratoRepo.ObtenerContratosActivos();

                foreach (var contrato in contratosActivos)
                {
                    if (!_pagoRepo.PagoMesRegistrado(contrato.Id, mesActual, anioActual))
                    {
                        PagosPendientes.Add(contrato);
                    }

                    if (contrato.FechaFin >= hoy && contrato.FechaFin <= en30Dias)
                    {
                        ContratosProximosVencer.Add(contrato);
                    }
                }

                TotalPendientes = PagosPendientes.Count;
                TotalProximosVencer = ContratosProximosVencer.Count;
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar datos: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        [RelayCommand]
        private void IrAApartamentos()
        {
            _navigationService.Navegar("Apartamento");
        }

        [RelayCommand]
        private void IrAInquilinos()
        {
            _navigationService.Navegar("Inquilino");
        }

        [RelayCommand]
        private void IrAContratos()
        {
            _navigationService.Navegar("Contrato");
        }

        [RelayCommand]
        private void IrAPagos()
        {
            _navigationService.Navegar("Pago");
        }
    }
}
