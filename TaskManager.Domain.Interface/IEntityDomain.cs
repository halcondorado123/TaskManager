using System.Linq.Expressions;

namespace TaskManager.Domain.Interface
{
    public interface IEntityDomain<T> where T : class 
    {
        Task<(IEnumerable<T> Entities, int TotalCount)> GetAllAsync(
          int page, int pageSize,
          params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}