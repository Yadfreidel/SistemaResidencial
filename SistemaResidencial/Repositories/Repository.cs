using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaResidencial.Data;
using SistemaResidencial.Interfaces;

namespace SistemaResidencial.Repositories
{
    // Implementación genérica del repositorio: usa context.Set<T>() para
    // acceder a cualquier entidad sin repetir código CRUD.
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ResidencialDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ResidencialDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public List<T> ObtenerTodos()
        {
            return _dbSet.ToList();
        }

        public T? ObtenerPorId(int id)
        {
            return _dbSet.Find(id);
        }

        public void Agregar(T entidad)
        {
            _dbSet.Add(entidad);
            _context.SaveChanges();
        }

        public void Actualizar(T entidad)
        {
            _dbSet.Update(entidad);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var entidad = ObtenerPorId(id);
            if (entidad != null)
            {
                _dbSet.Remove(entidad);
                _context.SaveChanges();
            }
        }
    }
}
