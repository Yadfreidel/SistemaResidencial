using SistemaResidencial.Models;

namespace SistemaResidencial.Services
{
    /// <summary>
    /// Servicio singleton que mantiene el estado de la sesión activa.
    /// Guarda el usuario logueado y expone su rol a toda la aplicación.
    /// Al ser Singleton, todos los ViewModels comparten la misma instancia.
    /// </summary>
    public class SesionService
    {
        /// <summary>El usuario actualmente logueado, o null si no hay sesión.</summary>
        public Usuario? UsuarioActual { get; private set; }

        /// <summary>
        /// Rol del usuario actual. Devuelve Admin como valor por defecto si no hay sesión.
        /// Los ViewModels que necesiten verificar sesión deben comprobar UsuarioActual != null primero.
        /// </summary>
        public Rol RolActual => UsuarioActual?.Rol ?? Rol.Admin;

        /// <summary>Evento que se dispara cuando cambia el estado de la sesión.</summary>
        public event Action? SesionCambiada;

        /// <summary>Inicia sesión guardando el usuario autenticado.</summary>
        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
            SesionCambiada?.Invoke();
        }

        /// <summary>Cierra la sesión actual, limpiando el usuario guardado.</summary>
        public void CerrarSesion()
        {
            UsuarioActual = null;
            SesionCambiada?.Invoke();
        }

        /// <summary>Indica si hay una sesión activa.</summary>
        public bool HaySesionActiva => UsuarioActual != null;
    }
}
