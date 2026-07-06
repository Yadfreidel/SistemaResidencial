using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SistemaResidencial.Data;
using SistemaResidencial.Models;

namespace SistemaResidencial.Services
{
    /// <summary>
    /// Servicio de autenticación. Maneja login, cambio y hash de contraseñas.
    /// El hashing se hace aquí con SHA256. NUNCA se almacena texto plano.
    /// </summary>
    public class AuthService
    {
        private readonly ResidencialDbContext _context;
        private readonly SesionService _sesionService;

        public AuthService(ResidencialDbContext context, SesionService sesionService)
        {
            _context = context;
            _sesionService = sesionService;
        }

        /// <summary>
        /// Verifica credenciales y si son correctas inicia la sesión.
        /// Acepta tanto contraseñas hasheadas como texto plano (compatibilidad con seed).
        /// </summary>
        public bool Login(string username, string password)
        {
            var hash = HashPassword(password);
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == username);

            if (user != null)
            {
                // Acepta hash SHA256 o texto plano (para el seed inicial)
                if (user.PasswordHash == hash || user.PasswordHash == password)
                {
                    _sesionService.IniciarSesion(user);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cambia la contraseña usando el objeto Usuario (llamada desde código interno).
        /// </summary>
        public bool CambiarPassword(Usuario usuario, string actual, string nuevo)
        {
            var actualHash = HashPassword(actual);
            if (usuario.PasswordHash == actualHash || usuario.PasswordHash == actual)
            {
                usuario.PasswordHash = HashPassword(nuevo);
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cambia la contraseña usando el nombre de usuario (llamada desde CambiarPasswordViewModel).
        /// Este overload es el que usan los ViewModels de Yadfreidel.
        /// </summary>
        public bool CambiarPassword(string nombreUsuario, string actual, string nuevo)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            if (usuario == null) return false;

            return CambiarPassword(usuario, actual, nuevo);
        }

        /// <summary>
        /// Recuperación de contraseña (pendiente de implementación completa).
        /// </summary>
        public void RecuperarPassword(string username)
        {
            throw new NotImplementedException("Funcionalidad de recuperar contraseña pendiente de implementación.");
        }

        /// <summary>
        /// Calcula el hash SHA256 de una contraseña en texto plano.
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}
