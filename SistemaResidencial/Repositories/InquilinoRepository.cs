using System.Collections.Generic;
using System.Linq;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;
using SistemaResidencial.Models;

namespace SistemaResidencial.Repositories
{
    public class InquilinoRepository : Repository<Inquilino>, IInquilinoRepository
    {
        public InquilinoRepository(ResidencialDbContext context) : base(context)
        {
        }

        public List<Inquilino> BuscarPorNombre(string nombre)
        {
            return _dbSet
                .Where(i => i.Nombre.Contains(nombre) || i.Apellido.Contains(nombre))
                .ToList();
        }

        public Inquilino? BuscarPorDocumento(string numeroDocumento)
        {
            return _dbSet.FirstOrDefault(i => i.NumeroDocumento == numeroDocumento);
        }
    }
}
