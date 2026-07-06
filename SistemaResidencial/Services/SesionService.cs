using SistemaResidencial.Models;

namespace SistemaResidencial.Services
{
    public class SesionService
    {
        public Usuario? UsuarioActual { get; private set; }

        public Rol RolActual => UsuarioActual?.Rol ?? Rol.Admin;

        public event Action? SesionCambiada;

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
            SesionCambiada?.Invoke();
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            SesionCambiada?.Invoke();
        }

        public bool HaySesionActiva => UsuarioActual != null;
    }
}
