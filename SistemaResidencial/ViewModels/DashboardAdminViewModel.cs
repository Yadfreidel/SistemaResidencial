using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    public partial class DashboardAdminViewModel : BaseViewModel
    {
        private readonly IApartamentoRepository _apartamentoRepo;
        private readonly IContratoRepository _contratoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly SesionService _sesionService;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        private int _totalApartamentos;

        [ObservableProperty]
        private int _apartamentosDisponibles;

        [ObservableProperty]
        private int _apartamentosOcupados;

        [ObservableProperty]
        private int _contratosActivos;

        [ObservableProperty]
        private decimal _ingresosMes;

        [ObservableProperty]
        private int _morosos;

        [ObservableProperty]
        private int _pagosDelMes;

        [ObservableProperty]
        private ObservableCollection<IngresoMes> _ingresoPorMes = new();

        [ObservableProperty]
        private double _porcentajeOcupacion;

        [ObservableProperty]
        private string _bienvenida = string.Empty;

        public DashboardAdminViewModel(
            IApartamentoRepository apartamentoRepo,
            IContratoRepository contratoRepo,
            IPagoRepository pagoRepo,
            SesionService sesionService,
            NavigationService navigationService)
        {
            _apartamentoRepo = apartamentoRepo;
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _sesionService = sesionService;
            _navigationService = navigationService;

            Titulo = "Dashboard — Administrador";
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

                var todosLosApartamentos = _apartamentoRepo.ObtenerTodos();
                TotalApartamentos = todosLosApartamentos.Count();

                ApartamentosDisponibles = _apartamentoRepo
                    .ObtenerPorEstado(Models.EstadoApartamento.Disponible).Count();

                ApartamentosOcupados = _apartamentoRepo
                    .ObtenerPorEstado(Models.EstadoApartamento.Ocupado).Count();

                PorcentajeOcupacion = TotalApartamentos > 0
                    ? (double)ApartamentosOcupados / TotalApartamentos * 100
                    : 0;

                var contratos = _contratoRepo.ObtenerContratosActivos();
                ContratosActivos = contratos.Count();

                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;

                var todosLosContratos = _contratoRepo.ObtenerTodos();
                int totalPagosDelMes = 0;
                decimal ingresosTotales = 0;

                foreach (var contrato in todosLosContratos)
                {
                    if (_pagoRepo.PagoMesRegistrado(contrato.Id, mesActual, anioActual))
                    {
                        totalPagosDelMes++;
                        ingresosTotales += contrato.MontoMensual;
                    }
                }

                PagosDelMes = totalPagosDelMes;
                IngresosMes = ingresosTotales;

                Morosos = ContratosActivos - PagosDelMes;
                if (Morosos < 0) Morosos = 0;

                CargarIngresoPorMes();
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

        private void CargarIngresoPorMes()
        {
            IngresoPorMes.Clear();

            for (int i = 5; i >= 0; i--)
            {
                var fecha = DateTime.Now.AddMonths(-i);
                int mes = fecha.Month;
                int anio = fecha.Year;

                decimal totalMes = 0;
                var contratos = _contratoRepo.ObtenerTodos();

                foreach (var contrato in contratos)
                {
                    if (_pagoRepo.PagoMesRegistrado(contrato.Id, mes, anio))
                        totalMes += contrato.MontoMensual;
                }

                IngresoPorMes.Add(new IngresoMes
                {
                    Mes = fecha.ToString("MMM yyyy"),
                    Total = totalMes
                });
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

    public class IngresoMes
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
