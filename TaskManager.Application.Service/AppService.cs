using AutoMapper;
using TaskManager.Application.Interface;
using TaskManager.Domain.Entities.Models;
using TaskManager.Domain.Entities.Pagination;
using TaskManager.Domain.Interface;

namespace TaskManager.Application.Service
{
    public class AppService<TEntity, TDto, TViewModel> : IAppService<TDto, TViewModel>
     where TEntity : class
     where TDto : class
     where TViewModel : class
    {
        private readonly IEntityDomain<TEntity> _domain;
        private readonly IMapper _mapper;

        public AppService(IEntityDomain<TEntity> domain, IMapper mapper)
        {
            _domain = domain;
            _mapper = mapper;
        }

        public async Task<PagedResult<TViewModel>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0)
                throw new ArgumentException("El número de página debe ser mayor que 0.", nameof(page));
            if (pageSize <= 0)
                throw new ArgumentException("El tamaño de página debe ser mayor que 0.", nameof(pageSize));

            IEnumerable<TEntity> entities;
            int totalCount;

            // Solo para TaskItemME incluimos el Status
            if (typeof(TEntity) == typeof(TaskItemME))
            {
                var result = await _domain.GetAllAsync(
                    page, pageSize,
                    include => ((TaskItemME)(object)include).Status);

                entities = result.Entities;
                totalCount = result.TotalCount;
            }
            else
            {
                var result = await _domain.GetAllAsync(page, pageSize);
                entities = result.Entities;
                totalCount = result.TotalCount;
            }

            var items = _mapper.Map<IEnumerable<TViewModel>>(entities);

            return new PagedResult<TViewModel>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TViewModel?> GetByIdAsync(int id)
        {
           var entity = await _domain.GetByIdAsync(id);
           if (entity == null) 
                throw new ArgumentException($"No se encontro ningún objeto con el id {id}.");

            var viewModel = _mapper.Map<TViewModel>(entity);
            return viewModel;
        }

        public async Task<TViewModel> CreateAsync(TDto dto)
        {
            if(dto == null) 
                throw new ArgumentNullException(nameof(dto), "Asegurese de ingresar todos los campos");

            var entity = _mapper.Map<TEntity>(dto);
            var created = await _domain.AddAsync(entity);
            return _mapper.Map<TViewModel>(created);
        }

        public async Task<TViewModel> UpdateAsync(TDto dto, int id)
        {
            var entity = await _domain.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"No se encontro ningún objeto con el id {id}.");

            _mapper.Map(dto, entity);
            await _domain.UpdateAsync(entity);

            return _mapper.Map<TViewModel>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _domain.DeleteAsync(id);

            if (!deleted)
                throw new KeyNotFoundException($"No se encontró ningún objeto con el id {id}.");

            return deleted;
        }
    }
}