namespace TaskManager.Infraestructure.Interface
{
    public interface IWriteRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}