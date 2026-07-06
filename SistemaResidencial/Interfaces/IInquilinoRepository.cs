using System.Collections.Generic;
using SistemaResidencial.Models;

namespace SistemaResidencial.Interfaces
{
    public interface IInquilinoRepository : IRepository<Inquilino>
    {
        List<Inquilino> BuscarPorNombre(string nombre);

        Inquilino? BuscarPorDocumento(string numeroDocumento);
    }
}
