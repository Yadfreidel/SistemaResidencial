using System.Collections.Generic;
using SistemaResidencial.Models;

namespace SistemaResidencial.Interfaces
{
    public interface IPagoRepository : IRepository<Pago>
    {
        List<Pago> ObtenerPagosPorContrato(int contratoId);

        bool PagoMesRegistrado(int contratoId, int mes, int anio);
    }
}
