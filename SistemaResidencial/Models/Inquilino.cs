using System.Collections.Generic;

namespace SistemaResidencial.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public TipoDocumento TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FotoRuta { get; set; } = string.Empty;

        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
