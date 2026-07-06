using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SistemaResidencial.Data;
using SistemaResidencial.Models;

namespace SistemaResidencial.Services
{
    public class AuthService
    {
        private readonly ResidencialDbContext _context;
        private readonly SesionService _sesionService;

        public AuthService(ResidencialDbContext context, SesionService sesionService)
        {
            _context = context;
            _sesionService = sesionService;
        }

        public bool Login(string username, string password)
        {
            var hash = HashPassword(password);
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == username);
            
            if (user != null)
            {
                if (user.PasswordHash == hash || user.PasswordHash == password)
                {
                    _sesionService.IniciarSesion(user);
                    return true;
                }
            }
            return false;
        }

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

        public void RecuperarPassword(string username)
        {
            throw new NotImplementedException("Funcionalidad de recuperar contraseña pendiente de implementación.");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
