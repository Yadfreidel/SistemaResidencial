using System.Collections.Generic;
using SistemaResidencial.Models;

namespace SistemaResidencial.Interfaces
{
    // Repositorio específico de Apartamento.
    // TODO: cambiar "string estado" por el enum EstadoApartamento cuando Ángel lo suba.
    public interface IApartamentoRepository : IRepository<Apartamento>
    {
        List<Apartamento> ObtenerPorEstado(string estado);
    }
}
