using System.Collections.Generic;
using SistemaResidencial.Models;

namespace SistemaResidencial.Interfaces
{
    public interface IContratoRepository : IRepository<Contrato>
    {
        List<Contrato> ObtenerContratosActivos();

        bool TieneContratoActivo(int apartamentoId);
    }
}
