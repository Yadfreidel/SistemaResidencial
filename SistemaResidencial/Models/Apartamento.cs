using System.Collections.Generic;

namespace SistemaResidencial.Models
{
    public class Apartamento
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public int Piso { get; set; }
        public string Bloque { get; set; }
        public int NumHabitaciones { get; set; }
        public double MetrosCuadrados { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioAlquiler { get; set; }
        public EstadoApartamento Estado { get; set; }

        public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}
