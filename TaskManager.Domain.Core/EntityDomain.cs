using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Interface;
using TaskManager.Infraestructure.Interface;

namespace TaskManager.Domain.Core
{
    public class EntityDomain<T> : IEntityDomain<T> where T : class
    {
        private readonly IReadRepository<T> _readRepository;
        private readonly IWriteRepository<T> _writeRepository;

        public EntityDomain(IReadRepository<T> readRepository, IWriteRepository<T> writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        public async Task<(IEnumerable<T> Entities, int TotalCount)> GetAllAsync(
            int page, int pageSize,
            params Expression<Func<T, object>>[] includes)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<T> query = _readRepository.Query(); // o _context.Set<T>()

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


        public async Task<T?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El ID debe ser mayor a 0.", nameof(id));

            var entity = await _readRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"La enditad con ID {id} no existe");

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            return await _writeRepository.AddAsync(entity);
        }
        public async Task<T> UpdateAsync(T entity)
        {
            return await _writeRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _readRepository.GetByIdAsync(id);

            if (entity == null)
                return false;

            await _writeRepository.DeleteAsync(entity);
            return true;
        }
    }
}
