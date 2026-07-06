using System.Collections.Generic;
using SistemaResidencial.Models;

namespace SistemaResidencial.Interfaces
{
    public interface IApartamentoRepository : IRepository<Apartamento>
    {
        List<Apartamento> ObtenerPorEstado(EstadoApartamento estado);
    }
}
