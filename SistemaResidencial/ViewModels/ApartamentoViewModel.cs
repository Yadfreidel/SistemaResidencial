using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del módulo de Apartamentos.
    /// Gestiona el CRUD completo: listar, agregar, editar, eliminar y filtrar.
    /// El sidebar se activa al seleccionar un apartamento en el DataGrid.
    /// </summary>
    public partial class ApartamentoViewModel : BaseViewModel
    {
        private readonly IApartamentoRepository _apartamentoRepo;
        private readonly DialogService _dialogService;

        // Lista completa de apartamentos (enlazada al DataGrid)
        [ObservableProperty]
        private ObservableCollection<Apartamento> _listaApartamentos = new();

        // Apartamento seleccionado en el DataGrid (abre el sidebar)
        [ObservableProperty]
        private Apartamento? _apartamentoSeleccionado;

        // Controla si el sidebar de formulario está visible
        [ObservableProperty]
        private bool _sidebarVisible;

        // Modo del formulario: "Agregar" o "Editar"
        [ObservableProperty]
        private string _modoFormulario = "Agregar";

        // ─── Campos del formulario en el sidebar ─────────────────────────────

        [ObservableProperty]
        private string _numero = string.Empty;

        [ObservableProperty]
        private int _piso;

        [ObservableProperty]
        private string _bloque = string.Empty;

        [ObservableProperty]
        private int _numHabitaciones;

        [ObservableProperty]
        private double _metrosCuadrados;

        [ObservableProperty]
        private string _descripcion = string.Empty;

        [ObservableProperty]
        private decimal _precioAlquiler;

        [ObservableProperty]
        private EstadoApartamento _estadoSeleccionado = EstadoApartamento.Disponible;

        // Lista de estados disponibles para el ComboBox de filtro
        [ObservableProperty]
        private ObservableCollection<EstadoApartamento> _listaEstados = new();

        // Filtro de búsqueda por texto
        [ObservableProperty]
        private string _textoBusqueda = string.Empty;

        // Estado seleccionado para filtrar
        [ObservableProperty]
        private EstadoApartamento? _filtroEstado;

        // Copia de toda la lista para búsqueda/filtrado sin ir a la BD
        private List<Apartamento> _apartamentosTodos = new();

        public ApartamentoViewModel(IApartamentoRepository apartamentoRepo, DialogService dialogService)
        {
            _apartamentoRepo = apartamentoRepo;
            _dialogService = dialogService;

            Titulo = "Gestión de Apartamentos";

            // Cargar los estados del enum en la lista del ComboBox
            foreach (EstadoApartamento estado in Enum.GetValues(typeof(EstadoApartamento)))
            {
                ListaEstados.Add(estado);
            }
        }

        /// <summary>
        /// Carga todos los apartamentos desde la base de datos.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                _apartamentosTodos = _apartamentoRepo.ObtenerTodos().ToList();
                ActualizarLista(_apartamentosTodos);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar apartamentos: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Abre el sidebar en modo Agregar con el formulario vacío.
        /// </summary>
        [RelayCommand]
        private void Agregar()
        {
            ModoFormulario = "Agregar";
            LimpiarFormulario();
            ApartamentoSeleccionado = null;
            SidebarVisible = true;
        }

        /// <summary>
        /// Abre el sidebar en modo Editar con los datos del apartamento seleccionado.
        /// </summary>
        [RelayCommand]
        private void Editar()
        {
            if (ApartamentoSeleccionado == null)
            {
                _dialogService.MostrarError("Seleccione un apartamento para editar.");
                return;
            }

            ModoFormulario = "Editar";
            CargarDatosEnFormulario(ApartamentoSeleccionado);
            SidebarVisible = true;
        }

        /// <summary>
        /// Guarda el apartamento (crea uno nuevo o actualiza el existente según el modo).
        /// </summary>
        [RelayCommand]
        private void Guardar()
        {
            if (!ValidarFormulario()) return;

            EstaCargando = true;

            try
            {
                if (ModoFormulario == "Agregar")
                {
                    var nuevo = new Apartamento
                    {
                        Numero = Numero,
                        Piso = Piso,
                        Bloque = Bloque,
                        NumHabitaciones = NumHabitaciones,
                        MetrosCuadrados = MetrosCuadrados,
                        Descripcion = Descripcion,
                        PrecioAlquiler = PrecioAlquiler,
                        Estado = EstadoSeleccionado
                    };

                    _apartamentoRepo.Agregar(nuevo);
                    _dialogService.MostrarInfo("Apartamento agregado correctamente.");
                }
                else
                {
                    if (ApartamentoSeleccionado == null) return;

                    ApartamentoSeleccionado.Numero = Numero;
                    ApartamentoSeleccionado.Piso = Piso;
                    ApartamentoSeleccionado.Bloque = Bloque;
                    ApartamentoSeleccionado.NumHabitaciones = NumHabitaciones;
                    ApartamentoSeleccionado.MetrosCuadrados = MetrosCuadrados;
                    ApartamentoSeleccionado.Descripcion = Descripcion;
                    ApartamentoSeleccionado.PrecioAlquiler = PrecioAlquiler;
                    ApartamentoSeleccionado.Estado = EstadoSeleccionado;

                    _apartamentoRepo.Actualizar(ApartamentoSeleccionado);
                    _dialogService.MostrarInfo("Apartamento actualizado correctamente.");
                }

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
        /// Elimina el apartamento seleccionado tras confirmación del usuario.
        /// </summary>
        [RelayCommand]
        private void Eliminar()
        {
            if (ApartamentoSeleccionado == null)
            {
                _dialogService.MostrarError("Seleccione un apartamento para eliminar.");
                return;
            }

            bool confirmado = _dialogService.MostrarConfirmacion(
                $"¿Está seguro de eliminar el apartamento {ApartamentoSeleccionado.Numero}?");

            if (!confirmado) return;

            EstaCargando = true;

            try
            {
                _apartamentoRepo.Eliminar(ApartamentoSeleccionado.Id);
                SidebarVisible = false;
                ApartamentoSeleccionado = null;
                CargarDatos();
                _dialogService.MostrarInfo("Apartamento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al eliminar: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Filtra la lista por el texto de búsqueda (número o bloque).
        /// </summary>
        [RelayCommand]
        private void Buscar()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                ActualizarLista(_apartamentosTodos);
                return;
            }

            string busqueda = TextoBusqueda.ToLower();
            var filtrados = _apartamentosTodos
                .Where(a => a.Numero.ToLower().Contains(busqueda)
                         || a.Bloque.ToLower().Contains(busqueda))
                .ToList();

            ActualizarLista(filtrados);
        }

        /// <summary>
        /// Filtra la lista por estado del apartamento.
        /// </summary>
        [RelayCommand]
        private void Filtrar()
        {
            if (FiltroEstado == null)
            {
                ActualizarLista(_apartamentosTodos);
                return;
            }

            var filtrados = _apartamentosTodos
                .Where(a => a.Estado == FiltroEstado)
                .ToList();

            ActualizarLista(filtrados);
        }

        /// <summary>
        /// Limpia todos los filtros y recarga la lista completa.
        /// </summary>
        [RelayCommand]
        private void LimpiarFiltros()
        {
            TextoBusqueda = string.Empty;
            FiltroEstado = null;
            ActualizarLista(_apartamentosTodos);
        }

        /// <summary>
        /// Cierra el sidebar de formulario.
        /// </summary>
        [RelayCommand]
        private void CerrarSidebar()
        {
            SidebarVisible = false;
            LimpiarFormulario();
        }

        // ─── Métodos privados de soporte ─────────────────────────────────────

        /// <summary>
        /// Actualiza el ObservableCollection con una nueva lista de apartamentos.
        /// </summary>
        private void ActualizarLista(IEnumerable<Apartamento> apartamentos)
        {
            ListaApartamentos.Clear();
            foreach (var apto in apartamentos)
                ListaApartamentos.Add(apto);
        }

        /// <summary>
        /// Carga los datos de un apartamento en los campos del formulario para edición.
        /// </summary>
        private void CargarDatosEnFormulario(Apartamento apartamento)
        {
            Numero = apartamento.Numero;
            Piso = apartamento.Piso;
            Bloque = apartamento.Bloque;
            NumHabitaciones = apartamento.NumHabitaciones;
            MetrosCuadrados = apartamento.MetrosCuadrados;
            Descripcion = apartamento.Descripcion ?? string.Empty;
            PrecioAlquiler = apartamento.PrecioAlquiler;
            EstadoSeleccionado = apartamento.Estado;
        }

        /// <summary>
        /// Limpia todos los campos del formulario.
        /// </summary>
        private void LimpiarFormulario()
        {
            Numero = string.Empty;
            Piso = 0;
            Bloque = string.Empty;
            NumHabitaciones = 0;
            MetrosCuadrados = 0;
            Descripcion = string.Empty;
            PrecioAlquiler = 0;
            EstadoSeleccionado = EstadoApartamento.Disponible;
            MensajeError = string.Empty;
        }

        /// <summary>
        /// Valida los campos requeridos del formulario antes de guardar.
        /// </summary>
        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(Numero))
            {
                MensajeError = "El número de apartamento es requerido.";
                return false;
            }

            if (PrecioAlquiler <= 0)
            {
                MensajeError = "El precio de alquiler debe ser mayor a 0.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Bloque))
            {
                MensajeError = "El bloque es requerido.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se ejecuta cuando cambia ApartamentoSeleccionado.
        /// Abre automáticamente el sidebar al seleccionar un apartamento en el DataGrid.
        /// </summary>
        partial void OnApartamentoSeleccionadoChanged(Apartamento? value)
        {
            if (value != null)
            {
                ModoFormulario = "Editar";
                CargarDatosEnFormulario(value);
                SidebarVisible = true;
            }
        }
    }
}
