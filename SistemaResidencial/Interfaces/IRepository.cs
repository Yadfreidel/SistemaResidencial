using System.Collections.Generic;

namespace SistemaResidencial.Interfaces
{
    // Interfaz genérica de repositorio: define las operaciones básicas (CRUD)
    // que van a compartir todos los repositorios específicos.
    public interface IRepository<T> where T : class
    {
        List<T> ObtenerTodos();

        T? ObtenerPorId(int id);

        void Agregar(T entidad);

        void Actualizar(T entidad);

        void Eliminar(int id);
    }
}
