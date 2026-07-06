using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del Dashboard para el Inquilino (rol Usuario).
    /// El inquilino ve su contrato activo y su historial de pagos.
    /// </summary>
    public partial class DashboardUsuarioViewModel : BaseViewModel
    {
        private readonly IContratoRepository _contratoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly IApartamentoRepository _apartamentoRepo;
        private readonly SesionService _sesionService;

        // Bienvenida personalizada
        [ObservableProperty]
        private string _bienvenida = string.Empty;

        // Contrato activo del inquilino logueado
        [ObservableProperty]
        private Contrato? _contratoActivo;

        // Historial de pagos del inquilino
        [ObservableProperty]
        private ObservableCollection<Pago> _historialPagos = new();

        // Información del apartamento del inquilino
        [ObservableProperty]
        private string _infoApartamento = string.Empty;

        // Indica si el inquilino tiene contrato activo
        [ObservableProperty]
        private bool _tieneContrato;

        // Días restantes para el próximo pago
        [ObservableProperty]
        private int _diasParaProximoPago;

        // Monto mensual del contrato
        [ObservableProperty]
        private decimal _montoMensual;

        public DashboardUsuarioViewModel(
            IContratoRepository contratoRepo,
            IPagoRepository pagoRepo,
            IApartamentoRepository apartamentoRepo,
            SesionService sesionService)
        {
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _apartamentoRepo = apartamentoRepo;
            _sesionService = sesionService;

            Titulo = "Mi Portal";
        }

        /// <summary>
        /// Carga el contrato y el historial de pagos del inquilino logueado.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                var usuario = _sesionService.UsuarioActual;
                if (usuario == null) return;

                Bienvenida = $"Bienvenido, {usuario.NombreUsuario}";

                // Si el usuario tiene un inquilino asociado, buscar su contrato activo
                if (usuario.InquilinoId.HasValue)
                {
                    var contratosActivos = _contratoRepo.ObtenerContratosActivos();

                    // Buscar el contrato del inquilino logueado
                    ContratoActivo = contratosActivos
                        .FirstOrDefault(c => c.InquilinoId == usuario.InquilinoId.Value);

                    if (ContratoActivo != null)
                    {
                        TieneContrato = true;
                        MontoMensual = ContratoActivo.MontoMensual;

                        // Calcular días para el próximo pago (primero del mes siguiente)
                        var hoy = DateTime.Now;
                        var primeroDiaSiguienteMes = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1);
                        DiasParaProximoPago = (primeroDiaSiguienteMes - hoy).Days;

                        // Información del apartamento
                        if (ContratoActivo.Apartamento != null)
                        {
                            var apto = ContratoActivo.Apartamento;
                            InfoApartamento = $"Bloque {apto.Bloque}, Piso {apto.Piso}, Apto {apto.Numero}";
                        }

                        // Cargar historial de pagos de este contrato
                        CargarHistorialPagos();
                    }
                    else
                    {
                        TieneContrato = false;
                        MensajeError = "No tienes un contrato activo en este momento.";
                    }
                }
                else
                {
                    TieneContrato = false;
                    MensajeError = "Tu cuenta no está vinculada a un inquilino.";
                }
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
        /// Carga el historial de pagos del contrato activo del inquilino.
        /// </summary>
        private void CargarHistorialPagos()
        {
            HistorialPagos.Clear();

            if (ContratoActivo == null) return;

            var pagos = _pagoRepo.ObtenerPagosPorContrato(ContratoActivo.Id);

            // Ordenar por fecha de pago descendente (más reciente primero)
            foreach (var pago in pagos.OrderByDescending(p => p.FechaPago))
            {
                HistorialPagos.Add(pago);
            }
        }
    }
}
