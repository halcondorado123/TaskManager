using System.Linq.Expressions;

namespace TaskManager.Infraestructure.Interface
{
    public interface IReadRepository<T> where T : class
    {
        Task<(IEnumerable<T> Entities, int TotalCount)> GetAllAsync(
            int page, int pageSize,
            params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(int id);
        IQueryable<T> Query();
    }
}