using System.Collections.Generic;
using System.Linq;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;

namespace SistemaResidencial.Repositories
{
    public class PagoRepository : Repository<Pago>, IPagoRepository
    {
        public PagoRepository(ResidencialDbContext context) : base(context)
        {
        }

        public List<Pago> ObtenerPagosPorContrato(int contratoId)
        {
            return _dbSet
                .Where(p => p.ContratoId == contratoId)
                .ToList();
        }

        public bool PagoMesRegistrado(int contratoId, int mes, int anio)
        {
            return _dbSet.Any(p =>
                p.ContratoId == contratoId &&
                p.FechaPago.Month == mes &&
                p.FechaPago.Year == anio);
        }
    }
}
