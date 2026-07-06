using System.Collections.Generic;

namespace SistemaResidencial.Models
{
    public class Apartamento
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int Piso { get; set; }
        public string Bloque { get; set; } = string.Empty;
        public int NumHabitaciones { get; set; }
        public double MetrosCuadrados { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioAlquiler { get; set; }
        public EstadoApartamento Estado { get; set; }

        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
