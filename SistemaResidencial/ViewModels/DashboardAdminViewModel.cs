using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del Dashboard para Administradores.
    /// Muestra métricas clave: total de apartamentos, contratos activos, pagos del mes y morosos.
    /// También expone datos para gráficos de ocupación e ingresos mensuales.
    /// </summary>
    public partial class DashboardAdminViewModel : BaseViewModel
    {
        private readonly IApartamentoRepository _apartamentoRepo;
        private readonly IContratoRepository _contratoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly SesionService _sesionService;

        // ─── Métricas principales (tarjetas del dashboard) ───────────────────

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
        private int _morosos; // Contratos con pagos pendientes

        [ObservableProperty]
        private int _pagosDelMes;

        // ─── Datos para gráficos ─────────────────────────────────────────────

        // Lista de meses con sus ingresos (para gráfico de barras)
        [ObservableProperty]
        private ObservableCollection<IngresoMes> _ingresoPorMes = new();

        // Porcentaje de ocupación (para gráfico circular)
        [ObservableProperty]
        private double _porcentajeOcupacion;

        // Nombre del usuario administrador logueado
        [ObservableProperty]
        private string _bienvenida = string.Empty;

        public DashboardAdminViewModel(
            IApartamentoRepository apartamentoRepo,
            IContratoRepository contratoRepo,
            IPagoRepository pagoRepo,
            SesionService sesionService)
        {
            _apartamentoRepo = apartamentoRepo;
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _sesionService = sesionService;

            Titulo = "Dashboard — Administrador";
        }

        /// <summary>
        /// Carga todas las métricas del dashboard. Se llama al navegar a esta vista.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                // Mensaje de bienvenida personalizado
                if (_sesionService.UsuarioActual != null)
                    Bienvenida = $"Bienvenido, {_sesionService.UsuarioActual.NombreUsuario}";

                // Métricas de apartamentos
                var todosLosApartamentos = _apartamentoRepo.ObtenerTodos();
                TotalApartamentos = todosLosApartamentos.Count();

                ApartamentosDisponibles = _apartamentoRepo
                    .ObtenerPorEstado(Models.EstadoApartamento.Disponible).Count();

                ApartamentosOcupados = _apartamentoRepo
                    .ObtenerPorEstado(Models.EstadoApartamento.Ocupado).Count();

                // Porcentaje de ocupación
                PorcentajeOcupacion = TotalApartamentos > 0
                    ? (double)ApartamentosOcupados / TotalApartamentos * 100
                    : 0;

                // Métricas de contratos
                var contratos = _contratoRepo.ObtenerContratosActivos();
                ContratosActivos = contratos.Count();

                // Métricas de pagos del mes actual
                int mesActual = DateTime.Now.Month;
                int anioActual = DateTime.Now.Year;

                var todosLosContratos = _contratoRepo.ObtenerTodos();
                int totalPagosDelMes = 0;
                decimal ingresosTotales = 0;

                foreach (var contrato in todosLosContratos)
                {
                    // Verificar si ya se realizó el pago del mes para este contrato
                    if (_pagoRepo.PagoMesRegistrado(contrato.Id, mesActual, anioActual))
                    {
                        totalPagosDelMes++;
                        ingresosTotales += contrato.MontoMensual;
                    }
                }

                PagosDelMes = totalPagosDelMes;
                IngresosMes = ingresosTotales;

                // Calcular morosos: contratos activos sin pago del mes actual
                Morosos = ContratosActivos - PagosDelMes;
                if (Morosos < 0) Morosos = 0;

                // Cargar datos para gráfico de ingresos por mes (últimos 6 meses)
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

        /// <summary>
        /// Construye la colección de ingresos por mes para los últimos 6 meses.
        /// </summary>
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
    }

    /// <summary>
    /// Clase auxiliar para representar el ingreso de un mes en el gráfico.
    /// </summary>
    public class IngresoMes
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
