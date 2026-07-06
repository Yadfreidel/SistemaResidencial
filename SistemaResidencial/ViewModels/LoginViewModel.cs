using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Models;
using SistemaResidencial.Services;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel para la pantalla de inicio de sesión.
    /// Gestiona el login y redirige al Dashboard según el rol del usuario.
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly SesionService _sesionService;
        private readonly NavigationService _navegacionService;
        private readonly DialogService _dialogService;

        // Campo de nombre de usuario en el formulario
        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        // Campo de contraseña en el formulario (texto plano solo en VM, nunca se persiste)
        [ObservableProperty]
        private string _password = string.Empty;

        // Controla si hay un proceso de login en curso (deshabilita el botón)
        [ObservableProperty]
        private bool _iniciandoSesion;

        public LoginViewModel(
            AuthService authService,
            SesionService sesionService,
            NavigationService navegacionService,
            DialogService dialogService)
        {
            _authService = authService;
            _sesionService = sesionService;
            _navegacionService = navegacionService;
            _dialogService = dialogService;

            Titulo = "Iniciar Sesión";
        }

        /// <summary>
        /// Comando ejecutado al presionar el botón "Entrar".
        /// Verifica las credenciales y navega al dashboard correspondiente según el rol.
        /// </summary>
        [RelayCommand]
        private void Login()
        {
            // Validación básica de campos vacíos
            if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(Password))
            {
                _dialogService.MostrarError("Por favor ingrese usuario y contraseña.");
                return;
            }

            IniciandoSesion = true;
            MensajeError = string.Empty;

            try
            {
                // Intentar autenticar al usuario mediante AuthService
                bool loginExitoso = _authService.Login(NombreUsuario, Password);

                if (loginExitoso)
                {
                    // Navegar al dashboard según el rol del usuario logueado
                    Rol rolActual = _sesionService.RolActual;

                    string vistaDestino = rolActual switch
                    {
                        Rol.Admin => "DashboardAdmin",
                        Rol.Recepcionista => "DashboardRecepcionista",
                        Rol.Usuario => "DashboardUsuario",
                        _ => "DashboardAdmin"
                    };

                    _navegacionService.Navegar(vistaDestino);
                }
                else
                {
                    MensajeError = "Usuario o contraseña incorrectos.";
                    Password = string.Empty; // Limpiar contraseña por seguridad
                }
            }
            finally
            {
                IniciandoSesion = false;
            }
        }

        /// <summary>
        /// Comando para la opción "Olvidé mi contraseña".
        /// Muestra un mensaje informativo al usuario.
        /// </summary>
        [RelayCommand]
        private void OlvidePassword()
        {
            _dialogService.MostrarInfo(
                "Comuníquese con el administrador del sistema para restablecer su contraseña.");
        }

        /// <summary>
        /// Limpia los campos al salir o recargar la vista.
        /// </summary>
        public void LimpiarFormulario()
        {
            NombreUsuario = string.Empty;
            Password = string.Empty;
            MensajeError = string.Empty;
        }
    }
}
