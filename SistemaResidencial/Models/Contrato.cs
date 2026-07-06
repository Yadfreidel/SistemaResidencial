using System;
using System.Collections.Generic;

namespace SistemaResidencial.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int ApartamentoId { get; set; }
        public int InquilinoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoMensual { get; set; }
        public decimal Deposito { get; set; }
        public bool Estado { get; set; }
        public string Observaciones { get; set; } = string.Empty;

        public virtual Apartamento Apartamento { get; set; } = null!;
        public virtual Inquilino Inquilino { get; set; } = null!;
        public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
