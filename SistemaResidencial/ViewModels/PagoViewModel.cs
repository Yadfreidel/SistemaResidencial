using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del módulo de Pagos.
    /// Registra el pago mensual de un contrato.
    /// Calcula automáticamente mora del 3% si el pago se realiza después del día 5 del mes.
    /// El número de recibo se genera automáticamente.
    /// </summary>
    public partial class PagoViewModel : BaseViewModel
    {
        private readonly IPagoRepository _pagoRepo;
        private readonly IContratoRepository _contratoRepo;
        private readonly DialogService _dialogService;

        // Constante de mora: 3% del monto mensual
        private const decimal PORCENTAJE_MORA = 0.03m;

        // Día límite para pago sin mora
        private const int DIA_LIMITE_SIN_MORA = 5;

        // Lista de pagos mostrada en el DataGrid
        [ObservableProperty]
        private ObservableCollection<Pago> _listaPagos = new();

        // Pago seleccionado en el DataGrid
        [ObservableProperty]
        private Pago? _pagoSeleccionado;

        // Controla visibilidad del sidebar
        [ObservableProperty]
        private bool _sidebarVisible;

        // ─── Listas para ComboBoxes ───────────────────────────────────────────

        // Lista de contratos activos para seleccionar
        [ObservableProperty]
        private ObservableCollection<Contrato> _listaContratos = new();

        // Lista de métodos de pago para el ComboBox
        [ObservableProperty]
        private ObservableCollection<MetodoPago> _listaMetodosPago = new();

        // ─── Campos del formulario ────────────────────────────────────────────

        // Contrato seleccionado para el pago
        [ObservableProperty]
        private Contrato? _contratoSeleccionado;

        // Fecha del pago (por defecto hoy)
        [ObservableProperty]
        private DateTime _fechaPago = DateTime.Today;

        // Monto mensual del contrato (solo lectura, se calcula del contrato)
        [ObservableProperty]
        private decimal _montoBase;

        // Mora calculada automáticamente
        [ObservableProperty]
        private decimal _montMora;

        // Monto total a pagar (base + mora si aplica)
        [ObservableProperty]
        private decimal _montoTotal;

        // Indica si hay mora aplicada
        [ObservableProperty]
        private bool _tieneMora;

        // Método de pago seleccionado
        [ObservableProperty]
        private MetodoPago _metodoPagoSeleccionado = MetodoPago.Efectivo;

        // Número de referencia del pago (transferencia / tarjeta)
        [ObservableProperty]
        private string _referencia = string.Empty;

        // Número de recibo generado automáticamente (solo lectura)
        [ObservableProperty]
        private string _numeroRecibo = string.Empty;

        // Copia completa para filtrado
        private List<Pago> _pagosTodos = new();

        public PagoViewModel(
            IPagoRepository pagoRepo,
            IContratoRepository contratoRepo,
            DialogService dialogService)
        {
            _pagoRepo = pagoRepo;
            _contratoRepo = contratoRepo;
            _dialogService = dialogService;

            Titulo = "Gestión de Pagos";

            // Cargar métodos de pago en el ComboBox
            foreach (MetodoPago metodo in Enum.GetValues(typeof(MetodoPago)))
            {
                ListaMetodosPago.Add(metodo);
            }
        }

        /// <summary>
        /// Carga todos los pagos y la lista de contratos activos desde la base de datos.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                // Cargar todos los pagos
                _pagosTodos = _pagoRepo.ObtenerTodos().ToList();
                ActualizarLista(_pagosTodos);

                // Cargar contratos activos para el ComboBox
                ListaContratos.Clear();
                foreach (var contrato in _contratoRepo.ObtenerContratosActivos())
                    ListaContratos.Add(contrato);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar pagos: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Abre el sidebar para registrar un nuevo pago.
        /// </summary>
        [RelayCommand]
        private void NuevoPago()
        {
            LimpiarFormulario();
            SidebarVisible = true;
        }

        /// <summary>
        /// Guarda el pago registrado.
        /// </summary>
        [RelayCommand]
        private void Guardar()
        {
            if (!ValidarFormulario()) return;

            EstaCargando = true;

            try
            {
                // Verificar que no se haya pagado ya este mes
                int mes = FechaPago.Month;
                int anio = FechaPago.Year;

                if (_pagoRepo.PagoMesRegistrado(ContratoSeleccionado!.Id, mes, anio))
                {
                    MensajeError = $"Ya existe un pago registrado para este contrato en {FechaPago:MMMM yyyy}.";
                    return;
                }

                // Generar número de recibo automático: REC-YYYYMM-ID del contrato
                string numRecibo = $"REC-{FechaPago:yyyyMM}-{ContratoSeleccionado.Id:D4}";

                var nuevoPago = new Pago
                {
                    ContratoId = ContratoSeleccionado!.Id,
                    FechaPago = FechaPago,
                    Monto = MontoTotal,        // Siempre el monto completo (base + mora)
                    MetodoPago = MetodoPagoSeleccionado,
                    Referencia = Referencia,
                    NumeroRecibo = numRecibo,
                    Estado = true // true indica Pagado
                };

                _pagoRepo.Agregar(nuevoPago);
                _dialogService.MostrarInfo($"Pago registrado correctamente.\nNº Recibo: {numRecibo}");
                SidebarVisible = false;
                CargarDatos();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al guardar: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Filtra los pagos por el contrato seleccionado.
        /// </summary>
        [RelayCommand]
        private void FiltrarPorContrato()
        {
            if (ContratoSeleccionado == null)
            {
                ActualizarLista(_pagosTodos);
                return;
            }

            var filtrados = _pagoRepo
                .ObtenerPagosPorContrato(ContratoSeleccionado.Id)
                .ToList();

            ActualizarLista(filtrados);
        }

        /// <summary>
        /// Cierra el sidebar.
        /// </summary>
        [RelayCommand]
        private void CerrarSidebar()
        {
            SidebarVisible = false;
            LimpiarFormulario();
        }

        // ─── Métodos privados ─────────────────────────────────────────────────

        private void ActualizarLista(IEnumerable<Pago> pagos)
        {
            ListaPagos.Clear();
            foreach (var p in pagos)
                ListaPagos.Add(p);
        }

        private void LimpiarFormulario()
        {
            ContratoSeleccionado = null;
            FechaPago = DateTime.Today;
            MontoBase = 0;
            MontMora = 0;
            MontoTotal = 0;
            TieneMora = false;
            MetodoPagoSeleccionado = MetodoPago.Efectivo;
            Referencia = string.Empty;
            NumeroRecibo = string.Empty;
            MensajeError = string.Empty;
        }

        private bool ValidarFormulario()
        {
            if (ContratoSeleccionado == null)
            {
                MensajeError = "Seleccione un contrato.";
                return false;
            }

            if (MontoTotal <= 0)
            {
                MensajeError = "El monto del pago debe ser mayor a 0.";
                return false;
            }

            // Si el método de pago es Transferencia o Tarjeta, la referencia es requerida
            if (MetodoPagoSeleccionado != MetodoPago.Efectivo
                && string.IsNullOrWhiteSpace(Referencia))
            {
                MensajeError = "Ingrese el número de referencia del pago.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se ejecuta cuando se selecciona un contrato.
        /// Carga el monto mensual y calcula la mora automáticamente.
        /// </summary>
        partial void OnContratoSeleccionadoChanged(Contrato? value)
        {
            if (value != null)
            {
                MontoBase = value.MontoMensual;
                CalcularMora();
            }
            else
            {
                MontoBase = 0;
                MontMora = 0;
                MontoTotal = 0;
                TieneMora = false;
            }
        }

        /// <summary>
        /// Se ejecuta cuando cambia la fecha de pago.
        /// Recalcula la mora si es necesario.
        /// </summary>
        partial void OnFechaPagoChanged(DateTime value)
        {
            CalcularMora();
        }

        /// <summary>
        /// Calcula automáticamente la mora si el pago se realiza después del día límite.
        /// Mora = 3% del monto mensual base.
        /// </summary>
        private void CalcularMora()
        {
            if (MontoBase <= 0) return;

            // Si el día de pago es mayor al día límite, aplica mora del 3%
            if (FechaPago.Day > DIA_LIMITE_SIN_MORA)
            {
                TieneMora = true;
                MontMora = Math.Round(MontoBase * PORCENTAJE_MORA, 2);
            }
            else
            {
                TieneMora = false;
                MontMora = 0;
            }

            MontoTotal = MontoBase + MontMora;
        }
    }
}
