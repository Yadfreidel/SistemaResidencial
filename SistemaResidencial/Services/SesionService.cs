using SistemaResidencial.Models;

namespace SistemaResidencial.Services
{
    public class SesionService
    {
        public Usuario? UsuarioActual { get; private set; }
        public Rol? RolActual => UsuarioActual?.Rol;

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}
