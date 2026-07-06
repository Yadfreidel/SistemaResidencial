using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;

namespace SistemaResidencial.Repositories
{
    public class ContratoRepository : Repository<Contrato>, IContratoRepository
    {
        public ContratoRepository(ResidencialDbContext context) : base(context)
        {
        }

        public List<Contrato> ObtenerContratosActivos()
        {
            return _dbSet
                .Include(c => c.Apartamento)
                .Include(c => c.Inquilino)
                .Where(c => c.Estado)
                .ToList();
        }

        public bool TieneContratoActivo(int apartamentoId)
        {
            return _dbSet.Any(c => c.ApartamentoId == apartamentoId && c.Estado);
        }
    }
}
