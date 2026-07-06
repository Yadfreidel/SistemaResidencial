using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del Dashboard para Recepcionistas.
    /// Vista reducida: muestra pagos pendientes del día y contratos próximos a vencer.
    /// </summary>
    public partial class DashboardRecepcionistaViewModel : BaseViewModel
    {
        private readonly IContratoRepository _contratoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly SesionService _sesionService;

        // Nombre de bienvenida
        [ObservableProperty]
        private string _bienvenida = string.Empty;

        // Contratos activos sin pago registrado este mes (morosos)
        [ObservableProperty]
        private ObservableCollection<Contrato> _pagosPendientes = new();

        // Contratos que vencen en los próximos 30 días
        [ObservableProperty]
        private ObservableCollection<Contrato> _contratosProximosVencer = new();

        // Número total de pagos pendientes
        [ObservableProperty]
        private int _totalPendientes;

        // Número de contratos por vencer
        [ObservableProperty]
        private int _totalProximosVencer;

        public DashboardRecepcionistaViewModel(
            IContratoRepository contratoRepo,
            IPagoRepository pagoRepo,
            SesionService sesionService)
        {
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _sesionService = sesionService;

            Titulo = "Dashboard — Recepcionista";
        }

        /// <summary>
        /// Carga los datos de pagos pendientes y contratos por vencer.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                // Bienvenida personalizada
                if (_sesionService.UsuarioActual != null)
                    Bienvenida = $"Bienvenido, {_sesionService.UsuarioActual.NombreUsuario}";

                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;
                DateTime hoy = DateTime.Now;
                DateTime en30Dias = hoy.AddDays(30);

                PagosPendientes.Clear();
                ContratosProximosVencer.Clear();

                // Obtener contratos activos
                var contratosActivos = _contratoRepo.ObtenerContratosActivos();

                foreach (var contrato in contratosActivos)
                {
                    // Agregar a pendientes si no tiene pago registrado este mes
                    if (!_pagoRepo.PagoMesRegistrado(contrato.Id, mesActual, anioActual))
                    {
                        PagosPendientes.Add(contrato);
                    }

                    // Agregar a próximos a vencer si la fecha de fin está en los próximos 30 días
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
    }
}
