using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Services;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel para el formulario de cambio de contraseña.
    /// Permite al usuario cambiar su contraseña verificando la actual.
    /// El hashing es responsabilidad de AuthService, nunca del ViewModel.
    /// </summary>
    public partial class CambiarPasswordViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly SesionService _sesionService;
        private readonly DialogService _dialogService;
        private readonly NavigationService _navegacionService;

        // Contraseña actual (para verificación de identidad)
        [ObservableProperty]
        private string _passwordActual = string.Empty;

        // Nueva contraseña deseada
        [ObservableProperty]
        private string _nuevoPassword = string.Empty;

        // Confirmación de la nueva contraseña
        [ObservableProperty]
        private string _confirmarPassword = string.Empty;

        // Indica si el proceso de guardado está en curso
        [ObservableProperty]
        private bool _guardando;

        // Mensaje de éxito al cambiar la contraseña
        [ObservableProperty]
        private string _mensajeExito = string.Empty;

        public CambiarPasswordViewModel(
            AuthService authService,
            SesionService sesionService,
            DialogService dialogService,
            NavigationService navegacionService)
        {
            _authService = authService;
            _sesionService = sesionService;
            _dialogService = dialogService;
            _navegacionService = navegacionService;

            Titulo = "Cambiar Contraseña";
        }

        /// <summary>
        /// Comando para guardar la nueva contraseña.
        /// Valida los campos y delega el cambio a AuthService.
        /// </summary>
        [RelayCommand]
        private void Guardar()
        {
            if (!ValidarFormulario()) return;

            Guardando = true;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            try
            {
                var usuario = _sesionService.UsuarioActual;
                if (usuario == null)
                {
                    MensajeError = "No hay sesión activa.";
                    return;
                }

                // Delegar el cambio de contraseña al AuthService (él maneja el hashing)
                bool cambioExitoso = _authService.CambiarPassword(
                    usuario.NombreUsuario,
                    PasswordActual,
                    NuevoPassword);

                if (cambioExitoso)
                {
                    MensajeExito = "Contraseña actualizada correctamente.";
                    LimpiarFormulario();
                    _dialogService.MostrarInfo("Su contraseña ha sido actualizada exitosamente.");
                }
                else
                {
                    MensajeError = "La contraseña actual es incorrecta.";
                    PasswordActual = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cambiar la contraseña: {ex.Message}";
            }
            finally
            {
                Guardando = false;
            }
        }

        /// <summary>
        /// Comando para cancelar y volver al dashboard.
        /// </summary>
        [RelayCommand]
        private void Cancelar()
        {
            LimpiarFormulario();

            // Navegar de vuelta según el rol del usuario
            if (_sesionService.UsuarioActual != null)
            {
                string vistaDestino = _sesionService.RolActual switch
                {
                    Models.Rol.Admin => "DashboardAdmin",
                    Models.Rol.Recepcionista => "DashboardRecepcionista",
                    Models.Rol.Usuario => "DashboardUsuario",
                    _ => "DashboardAdmin"
                };

                _navegacionService.Navegar(vistaDestino);
            }
        }

        // ─── Métodos privados ─────────────────────────────────────────────────

        private void LimpiarFormulario()
        {
            PasswordActual = string.Empty;
            NuevoPassword = string.Empty;
            ConfirmarPassword = string.Empty;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(PasswordActual))
            {
                MensajeError = "Ingrese su contraseña actual.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NuevoPassword))
            {
                MensajeError = "Ingrese la nueva contraseña.";
                return false;
            }

            // Validar longitud mínima de la nueva contraseña
            if (NuevoPassword.Length < 6)
            {
                MensajeError = "La nueva contraseña debe tener al menos 6 caracteres.";
                return false;
            }

            if (NuevoPassword != ConfirmarPassword)
            {
                MensajeError = "Las contraseñas no coinciden.";
                ConfirmarPassword = string.Empty;
                return false;
            }

            // No permitir que la nueva contraseña sea igual a la actual
            if (NuevoPassword == PasswordActual)
            {
                MensajeError = "La nueva contraseña debe ser diferente a la actual.";
                return false;
            }

            return true;
        }
    }
}
