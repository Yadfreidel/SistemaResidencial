namespace SistemaResidencial.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public Rol Rol { get; set; }

        public int? InquilinoId { get; set; }
    }
}
