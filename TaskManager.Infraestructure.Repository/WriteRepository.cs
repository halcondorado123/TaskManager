using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Interface;

namespace TaskManager.Infraestructure.Repository
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class
    {
        private readonly TaskManagerDbContext _context;

        public WriteRepository(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
