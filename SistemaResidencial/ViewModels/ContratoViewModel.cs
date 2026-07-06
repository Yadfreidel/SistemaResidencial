using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del módulo de Contratos.
    /// Gestiona la creación y visualización de contratos de arrendamiento.
    /// Selección de inquilino y apartamento desde listas desplegables.
    /// Valida duración mínima de 1 año y que el apartamento no tenga contrato activo.
    /// </summary>
    public partial class ContratoViewModel : BaseViewModel
    {
        private readonly IContratoRepository _contratoRepo;
        private readonly IApartamentoRepository _apartamentoRepo;
        private readonly IInquilinoRepository _inquilinoRepo;
        private readonly DialogService _dialogService;

        // Lista de contratos mostrada en el DataGrid
        [ObservableProperty]
        private ObservableCollection<Contrato> _listaContratos = new();

        // Contrato seleccionado en el DataGrid
        [ObservableProperty]
        private Contrato? _contratoSeleccionado;

        // Controla visibilidad del sidebar
        [ObservableProperty]
        private bool _sidebarVisible;

        // Modo del formulario: "Agregar" o "Ver" (los contratos activos no se editan)
        [ObservableProperty]
        private string _modoFormulario = "Agregar";

        // ─── Listas para ComboBoxes ───────────────────────────────────────────

        // Inquilinos disponibles para seleccionar en el ComboBox
        [ObservableProperty]
        private ObservableCollection<Inquilino> _listaInquilinos = new();

        // Apartamentos disponibles (solo los que están en estado Disponible)
        [ObservableProperty]
        private ObservableCollection<Apartamento> _listaApartamentosDisponibles = new();

        // ─── Campos del formulario ────────────────────────────────────────────

        [ObservableProperty]
        private Inquilino? _inquilinoSeleccionado;

        [ObservableProperty]
        private Apartamento? _apartamentoSeleccionado;

        [ObservableProperty]
        private DateTime _fechaInicio = DateTime.Today;

        // La fecha fin mínima es 1 año después de la fecha de inicio
        [ObservableProperty]
        private DateTime _fechaFin = DateTime.Today.AddYears(1);

        [ObservableProperty]
        private decimal _montoMensual;

        [ObservableProperty]
        private decimal _deposito;

        [ObservableProperty]
        private string _observaciones = string.Empty;

        // Fecha fin mínima permitida (reactiva a FechaInicio)
        [ObservableProperty]
        private DateTime _fechaFinMinima = DateTime.Today.AddYears(1);

        // Copia completa para filtrado local
        private List<Contrato> _contratosTodos = new();

        public ContratoViewModel(
            IContratoRepository contratoRepo,
            IApartamentoRepository apartamentoRepo,
            IInquilinoRepository inquilinoRepo,
            DialogService dialogService)
        {
            _contratoRepo = contratoRepo;
            _apartamentoRepo = apartamentoRepo;
            _inquilinoRepo = inquilinoRepo;
            _dialogService = dialogService;

            Titulo = "Gestión de Contratos";
        }

        /// <summary>
        /// Carga todos los contratos y las listas de selección desde la base de datos.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                // Cargar contratos
                _contratosTodos = _contratoRepo.ObtenerTodos().ToList();
                ActualizarLista(_contratosTodos);

                // Cargar listas para los ComboBoxes
                CargarInquilinos();
                CargarApartamentosDisponibles();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar contratos: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Carga todos los inquilinos para el ComboBox de selección.
        /// </summary>
        private void CargarInquilinos()
        {
            ListaInquilinos.Clear();
            foreach (var inq in _inquilinoRepo.ObtenerTodos())
                ListaInquilinos.Add(inq);
        }

        /// <summary>
        /// Carga solo los apartamentos disponibles para el ComboBox.
        /// </summary>
        private void CargarApartamentosDisponibles()
        {
            ListaApartamentosDisponibles.Clear();
            var disponibles = _apartamentoRepo.ObtenerPorEstado(EstadoApartamento.Disponible);
            foreach (var apto in disponibles)
                ListaApartamentosDisponibles.Add(apto);
        }

        /// <summary>
        /// Abre el sidebar en modo Agregar con el formulario vacío.
        /// </summary>
        [RelayCommand]
        private void Agregar()
        {
            ModoFormulario = "Agregar";
            LimpiarFormulario();
            ContratoSeleccionado = null;
            SidebarVisible = true;

            // Recargar apartamentos disponibles al abrir el formulario
            CargarApartamentosDisponibles();
        }

        /// <summary>
        /// Guarda el nuevo contrato tras validar las reglas de negocio.
        /// </summary>
        [RelayCommand]
        private void Guardar()
        {
            if (!ValidarFormulario()) return;

            EstaCargando = true;

            try
            {
                // Verificar que el apartamento no tenga ya un contrato activo
                if (_contratoRepo.TieneContratoActivo(ApartamentoSeleccionado!.Id))
                {
                    MensajeError = "Este apartamento ya tiene un contrato activo.";
                    return;
                }

                var nuevo = new Contrato
                {
                    InquilinoId = InquilinoSeleccionado!.Id,
                    ApartamentoId = ApartamentoSeleccionado!.Id,
                    FechaInicio = FechaInicio,
                    FechaFin = FechaFin,
                    MontoMensual = MontoMensual,
                    Deposito = Deposito,
                    Estado = true, // true indica Activo según el modelo
                    Observaciones = Observaciones
                };

                _contratoRepo.Agregar(nuevo);

                // Cambiar el estado del apartamento a Ocupado
                ApartamentoSeleccionado.Estado = EstadoApartamento.Ocupado;
                _apartamentoRepo.Actualizar(ApartamentoSeleccionado);

                _dialogService.MostrarInfo("Contrato creado correctamente.");
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
        /// Cierra el sidebar.
        /// </summary>
        [RelayCommand]
        private void CerrarSidebar()
        {
            SidebarVisible = false;
            LimpiarFormulario();
        }

        // ─── Métodos privados ─────────────────────────────────────────────────

        private void ActualizarLista(IEnumerable<Contrato> contratos)
        {
            ListaContratos.Clear();
            foreach (var c in contratos)
                ListaContratos.Add(c);
        }

        private void LimpiarFormulario()
        {
            InquilinoSeleccionado = null;
            ApartamentoSeleccionado = null;
            FechaInicio = DateTime.Today;
            FechaFin = DateTime.Today.AddYears(1);
            FechaFinMinima = DateTime.Today.AddYears(1);
            MontoMensual = 0;
            Deposito = 0;
            Observaciones = string.Empty;
            MensajeError = string.Empty;
        }

        private bool ValidarFormulario()
        {
            if (InquilinoSeleccionado == null)
            {
                MensajeError = "Seleccione un inquilino.";
                return false;
            }

            if (ApartamentoSeleccionado == null)
            {
                MensajeError = "Seleccione un apartamento.";
                return false;
            }

            if (FechaFin <= FechaInicio)
            {
                MensajeError = "La fecha de fin debe ser posterior a la fecha de inicio.";
                return false;
            }

            // Regla: duración mínima de 1 año (12 meses)
            if (FechaFin < FechaInicio.AddMonths(12))
            {
                MensajeError = "El contrato debe tener una duración mínima de 1 año (12 meses).";
                return false;
            }

            if (MontoMensual <= 0)
            {
                MensajeError = "El monto mensual debe ser mayor a 0.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se ejecuta cuando cambia la fecha de inicio.
        /// Actualiza la fecha fin mínima automáticamente (1 año después).
        /// </summary>
        partial void OnFechaInicioChanged(DateTime value)
        {
            FechaFinMinima = value.AddYears(1);

            // Si la fecha fin actual es menor a la nueva mínima, ajustar automáticamente
            if (FechaFin < FechaFinMinima)
                FechaFin = FechaFinMinima;
        }

        /// <summary>
        /// Se ejecuta cuando se selecciona un contrato en el DataGrid.
        /// Muestra sus detalles en el sidebar.
        /// </summary>
        partial void OnContratoSeleccionadoChanged(Contrato? value)
        {
            if (value != null)
            {
                SidebarVisible = true;
                ModoFormulario = "Ver";
            }
        }
    }
}
