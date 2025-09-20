using TaskManager.Domain.Entities.Pagination;

namespace TaskManager.Application.Interface
{
    public interface IAppService<TDto, TViewModel> 
        where TDto : class
        where TViewModel : class 
    {
        Task<PagedResult<TViewModel>> GetAllAsync(int page, int pageSize);
        Task<TViewModel> GetByIdAsync(int id);
        Task<TViewModel> CreateAsync(TDto dto);
        Task<TViewModel> UpdateAsync(TDto dto, int id);
        Task<bool> DeleteAsync(int id);
    }
}