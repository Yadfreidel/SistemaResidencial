using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Services;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel principal de la aplicación.
    /// Controla la navegación entre vistas usando VistaActual.
    /// El menú lateral y el ContentControl central se enlazan aquí.
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        // Servicios inyectados por constructor
        private readonly NavigationService _navegacionService;
        private readonly SesionService _sesionService;

        // Vista que se muestra actualmente en el ContentControl central
        [ObservableProperty]
        private BaseViewModel _vistaActual = null!;

        // Nombre del usuario logueado que se muestra en la barra superior
        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        // Rol del usuario (Admin / Recepcionista / Usuario)
        [ObservableProperty]
        private string _rolUsuario = string.Empty;

        // Controla si el menú lateral está expandido o contraído
        [ObservableProperty]
        private bool _menuExpandido = true;

        // Controla si el menú lateral es visible (solo cuando hay sesión activa)
        [ObservableProperty]
        private bool _menuVisible = false;

        public MainViewModel(NavigationService navegacionService, SesionService sesionService)
        {
            _navegacionService = navegacionService;
            _sesionService = sesionService;

            Titulo = "Sistema Residencial";

            // Suscribirse a cambios de vista desde el NavigationService
            _navegacionService.VistaActualCambiada += OnVistaActualCambiada;

            // Suscribirse a cambios de sesión para actualizar UI cuando el usuario hace login/logout
            _sesionService.SesionCambiada += OnSesionCambiada;

            // Cargar datos del usuario logueado
            CargarDatosUsuario();
        }

        /// <summary>
        /// Se ejecuta cuando NavigationService cambia la vista activa.
        /// </summary>
        private void OnVistaActualCambiada(BaseViewModel nuevaVista)
        {
            VistaActual = nuevaVista;
        }

        /// <summary>
        /// Se ejecuta cuando SesionService cambia el estado de la sesión (login/logout).
        /// Actualiza los datos del usuario en la UI.
        /// </summary>
        private void OnSesionCambiada()
        {
            CargarDatosUsuario();
        }

        /// <summary>
        /// Carga los datos del usuario actualmente logueado desde SesionService.
        /// </summary>
        private void CargarDatosUsuario()
        {
            if (_sesionService.UsuarioActual != null)
            {
                NombreUsuario = _sesionService.UsuarioActual.NombreUsuario;
                RolUsuario = _sesionService.RolActual.ToString();
                MenuVisible = true;
            }
            else
            {
                NombreUsuario = string.Empty;
                RolUsuario = string.Empty;
                MenuVisible = false;
            }
        }

        /// <summary>
        /// Comando para alternar el menú lateral entre expandido y contraído.
        /// </summary>
        [RelayCommand]
        private void AlternarMenu()
        {
            MenuExpandido = !MenuExpandido;
        }

        /// <summary>
        /// Comando para navegar a una vista por su nombre.
        /// </summary>
        [RelayCommand]
        private void Navegar(string nombreVista)
        {
            _navegacionService.Navegar(nombreVista);
        }

        /// <summary>
        /// Comando para cerrar sesión y volver al Login.
        /// </summary>
        [RelayCommand]
        private void CerrarSesion()
        {
            _sesionService.CerrarSesion();
            _navegacionService.Navegar("Login");
        }
    }
}
