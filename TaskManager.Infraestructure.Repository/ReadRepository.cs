using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Interface;

namespace TaskManager.Infraestructure.Repository
{
    public class ReadRepository<T> : IReadRepository<T> where T : class
    {
        private readonly TaskManagerDbContext _context;
        private readonly DbSet<T> _dbSet;

        public ReadRepository(TaskManagerDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<(IEnumerable<T> Entities, int TotalCount)> GetAllAsync(
            int page, int pageSize,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            // Aplicar los includes dinámicamente
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            var totalCount = await query.CountAsync();

            var entities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (entities, totalCount);
        }


        // El método GetByIdAsync ahora devuelve un Task<T>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }
    }
}
