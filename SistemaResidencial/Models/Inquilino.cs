using System.Collections.Generic;

namespace SistemaResidencial.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string FotoRuta { get; set; }

        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
