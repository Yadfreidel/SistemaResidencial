using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;
using SistemaResidencial.Services;
using System.Collections.ObjectModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel del módulo de Inquilinos.
    /// Igual que Apartamento: CRUD completo + búsqueda por nombre y por número de documento.
    /// Incluye gestión del campo de foto del inquilino.
    /// </summary>
    public partial class InquilinoViewModel : BaseViewModel
    {
        private readonly IInquilinoRepository _inquilinoRepo;
        private readonly DialogService _dialogService;

        // Lista de inquilinos mostrada en el DataGrid
        [ObservableProperty]
        private ObservableCollection<Inquilino> _listaInquilinos = new();

        // Inquilino seleccionado en el DataGrid
        [ObservableProperty]
        private Inquilino? _inquilinoSeleccionado;

        // Controla visibilidad del sidebar
        [ObservableProperty]
        private bool _sidebarVisible;

        // Modo del formulario: "Agregar" o "Editar"
        [ObservableProperty]
        private string _modoFormulario = "Agregar";

        // ─── Campos del formulario ────────────────────────────────────────────

        [ObservableProperty]
        private string _nombre = string.Empty;

        [ObservableProperty]
        private string _apellido = string.Empty;

        [ObservableProperty]
        private TipoDocumento _tipoDocumentoSeleccionado = TipoDocumento.Cedula;

        [ObservableProperty]
        private string _numeroDocumento = string.Empty;

        [ObservableProperty]
        private string _telefono = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        // Ruta de la foto del inquilino (imagen de perfil)
        [ObservableProperty]
        private string _fotoRuta = string.Empty;

        // Lista de tipos de documento para el ComboBox
        [ObservableProperty]
        private ObservableCollection<TipoDocumento> _listaTiposDocumento = new();

        // ─── Campos de búsqueda ───────────────────────────────────────────────

        // Búsqueda general (busca en todos los campos)
        [ObservableProperty]
        private string _busquedaGeneral = string.Empty;

        // Búsqueda por nombre o apellido
        [ObservableProperty]
        private string _busquedaNombre = string.Empty;

        // Búsqueda por número de documento
        [ObservableProperty]
        private string _busquedaDocumento = string.Empty;

        // Filtro por tipo de documento
        [ObservableProperty]
        private TipoDocumento? _filtroTipoDocumento = null;

        // Filtro por teléfono
        [ObservableProperty]
        private string _filtroTelefono = string.Empty;

        // Filtro por email
        [ObservableProperty]
        private string _filtroEmail = string.Empty;

        // Copia completa de la lista para filtrado local
        private List<Inquilino> _inquilinosTodos = new();

        public InquilinoViewModel(IInquilinoRepository inquilinoRepo, DialogService dialogService)
        {
            _inquilinoRepo = inquilinoRepo;
            _dialogService = dialogService;

            Titulo = "Gestión de Inquilinos";

            // Cargar tipos de documento en el ComboBox
            foreach (TipoDocumento tipo in Enum.GetValues(typeof(TipoDocumento)))
            {
                ListaTiposDocumento.Add(tipo);
            }
        }

        /// <summary>
        /// Carga todos los inquilinos desde la base de datos.
        /// </summary>
        [RelayCommand]
        private void CargarDatos()
        {
            EstaCargando = true;
            MensajeError = string.Empty;

            try
            {
                _inquilinosTodos = _inquilinoRepo.ObtenerTodos().ToList();
                ActualizarLista(_inquilinosTodos);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar inquilinos: {ex.Message}";
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
            InquilinoSeleccionado = null;
            SidebarVisible = true;
        }

        /// <summary>
        /// Abre el sidebar en modo Editar con los datos del inquilino seleccionado.
        /// </summary>
        [RelayCommand]
        private void Editar()
        {
            if (InquilinoSeleccionado == null)
            {
                _dialogService.MostrarError("Seleccione un inquilino para editar.");
                return;
            }

            ModoFormulario = "Editar";
            CargarDatosEnFormulario(InquilinoSeleccionado);
            SidebarVisible = true;
        }

        /// <summary>
        /// Guarda el inquilino (crea nuevo o actualiza existente según el modo).
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
                    var nuevo = new Inquilino
                    {
                        Nombre = Nombre,
                        Apellido = Apellido,
                        TipoDocumento = TipoDocumentoSeleccionado,
                        NumeroDocumento = NumeroDocumento,
                        Telefono = Telefono,
                        Email = Email,
                        FotoRuta = FotoRuta
                    };

                    _inquilinoRepo.Agregar(nuevo);
                    _dialogService.MostrarInfo("Inquilino registrado correctamente.");
                }
                else
                {
                    if (InquilinoSeleccionado == null) return;

                    InquilinoSeleccionado.Nombre = Nombre;
                    InquilinoSeleccionado.Apellido = Apellido;
                    InquilinoSeleccionado.TipoDocumento = TipoDocumentoSeleccionado;
                    InquilinoSeleccionado.NumeroDocumento = NumeroDocumento;
                    InquilinoSeleccionado.Telefono = Telefono;
                    InquilinoSeleccionado.Email = Email;
                    InquilinoSeleccionado.FotoRuta = FotoRuta;

                    _inquilinoRepo.Actualizar(InquilinoSeleccionado);
                    _dialogService.MostrarInfo("Inquilino actualizado correctamente.");
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
        /// Elimina el inquilino seleccionado tras confirmación del usuario.
        /// </summary>
        [RelayCommand]
        private void Eliminar()
        {
            if (InquilinoSeleccionado == null)
            {
                _dialogService.MostrarError("Seleccione un inquilino para eliminar.");
                return;
            }

            bool confirmado = _dialogService.MostrarConfirmacion(
                $"¿Está seguro de eliminar a {InquilinoSeleccionado.Nombre} {InquilinoSeleccionado.Apellido}?");

            if (!confirmado) return;

            EstaCargando = true;

            try
            {
                _inquilinoRepo.Eliminar(InquilinoSeleccionado.Id);
                SidebarVisible = false;
                InquilinoSeleccionado = null;
                CargarDatos();
                _dialogService.MostrarInfo("Inquilino eliminado correctamente.");
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
        /// Busca inquilinos por nombre o apellido.
        /// </summary>
        [RelayCommand]
        private void BuscarPorNombre()
        {
            if (string.IsNullOrWhiteSpace(BusquedaNombre))
            {
                ActualizarLista(_inquilinosTodos);
                return;
            }

            var resultados = _inquilinoRepo.BuscarPorNombre(BusquedaNombre).ToList();
            ActualizarLista(resultados);
        }

        /// <summary>
        /// Busca inquilinos por número de documento.
        /// </summary>
        [RelayCommand]
        private void BuscarPorDocumento()
        {
            if (string.IsNullOrWhiteSpace(BusquedaDocumento))
            {
                ActualizarLista(_inquilinosTodos);
                return;
            }

            var inquilino = _inquilinoRepo.BuscarPorDocumento(BusquedaDocumento);
            var resultados = inquilino != null ? new List<Inquilino> { inquilino } : new List<Inquilino>();
            ActualizarLista(resultados);
        }

        /// <summary>
        /// Limpia los filtros de búsqueda y restaura la lista completa.
        /// </summary>
        [RelayCommand]
        private void LimpiarBusqueda()
        {
            BusquedaGeneral = string.Empty;
            BusquedaNombre = string.Empty;
            BusquedaDocumento = string.Empty;
            FiltroTipoDocumento = null;
            FiltroTelefono = string.Empty;
            FiltroEmail = string.Empty;
            ActualizarLista(_inquilinosTodos);
        }

        /// <summary>
        /// Búsqueda general que filtra por todos los campos disponibles.
        /// </summary>
        [RelayCommand]
        private void BuscarGeneral()
        {
            if (string.IsNullOrWhiteSpace(BusquedaGeneral))
            {
                ActualizarLista(_inquilinosTodos);
                return;
            }

            var termino = BusquedaGeneral.ToLower();
            var resultados = _inquilinosTodos.Where(i =>
                i.Nombre.ToLower().Contains(termino) ||
                i.Apellido.ToLower().Contains(termino) ||
                i.NumeroDocumento.ToLower().Contains(termino) ||
                (i.Telefono != null && i.Telefono.ToLower().Contains(termino)) ||
                (i.Email != null && i.Email.ToLower().Contains(termino))
            ).ToList();

            ActualizarLista(resultados);
        }

        /// <summary>
        /// Aplica todos los filtros combinados.
        /// </summary>
        [RelayCommand]
        private void AplicarFiltros()
        {
            var resultados = _inquilinosTodos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(BusquedaNombre))
            {
                resultados = resultados.Where(i =>
                    i.Nombre.ToLower().Contains(BusquedaNombre.ToLower()) ||
                    i.Apellido.ToLower().Contains(BusquedaNombre.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(BusquedaDocumento))
            {
                resultados = resultados.Where(i =>
                    i.NumeroDocumento.ToLower().Contains(BusquedaDocumento.ToLower()));
            }

            if (FiltroTipoDocumento.HasValue)
            {
                resultados = resultados.Where(i => i.TipoDocumento == FiltroTipoDocumento.Value);
            }

            if (!string.IsNullOrWhiteSpace(FiltroTelefono))
            {
                resultados = resultados.Where(i =>
                    i.Telefono != null && i.Telefono.ToLower().Contains(FiltroTelefono.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(FiltroEmail))
            {
                resultados = resultados.Where(i =>
                    i.Email != null && i.Email.ToLower().Contains(FiltroEmail.ToLower()));
            }

            ActualizarLista(resultados.ToList());
        }

        /// <summary>
        /// Abre un diálogo para seleccionar la foto del inquilino.
        /// </summary>
        [RelayCommand]
        private void SeleccionarFoto()
        {
            // La selección de archivo se hace desde la vista (code-behind del XAML)
            // Este comando sirve como punto de notificación
            _dialogService.MostrarInfo("Use el botón de selección de archivo en la vista.");
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

        private void ActualizarLista(IEnumerable<Inquilino> inquilinos)
        {
            ListaInquilinos.Clear();
            foreach (var inq in inquilinos)
                ListaInquilinos.Add(inq);
        }

        private void CargarDatosEnFormulario(Inquilino inquilino)
        {
            Nombre = inquilino.Nombre;
            Apellido = inquilino.Apellido;
            TipoDocumentoSeleccionado = inquilino.TipoDocumento;
            NumeroDocumento = inquilino.NumeroDocumento;
            Telefono = inquilino.Telefono ?? string.Empty;
            Email = inquilino.Email ?? string.Empty;
            FotoRuta = inquilino.FotoRuta ?? string.Empty;
        }

        private void LimpiarFormulario()
        {
            Nombre = string.Empty;
            Apellido = string.Empty;
            TipoDocumentoSeleccionado = TipoDocumento.Cedula;
            NumeroDocumento = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            FotoRuta = string.Empty;
            MensajeError = string.Empty;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MensajeError = "El nombre del inquilino es requerido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Apellido))
            {
                MensajeError = "El apellido del inquilino es requerido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NumeroDocumento))
            {
                MensajeError = "El número de documento es requerido.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains('@'))
            {
                MensajeError = "El formato del correo electrónico no es válido.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Se ejecuta automáticamente cuando el inquilino seleccionado cambia.
        /// Abre el sidebar con sus datos.
        /// </summary>
        partial void OnInquilinoSeleccionadoChanged(Inquilino? value)
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
