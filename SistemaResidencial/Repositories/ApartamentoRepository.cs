using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;

namespace SistemaResidencial.Repositories
{
    public class ApartamentoRepository : Repository<Apartamento>, IApartamentoRepository
    {
        public ApartamentoRepository(ResidencialDbContext context) : base(context)
        {
        }

        public List<Apartamento> ObtenerPorEstado(EstadoApartamento estado)
        {
            return _dbSet
                .Include(a => a.Contratos)
                .Where(a => a.Estado == estado)
                .ToList();
        }
    }
}
