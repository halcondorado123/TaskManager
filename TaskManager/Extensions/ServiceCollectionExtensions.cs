using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTO.DTO;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;
using TaskManager.Domain.Core;
using TaskManager.Domain.Entities.Models;
using TaskManager.Domain.Interface;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Repositories;
using TaskManager.Infraestructure.Interface;
using TaskManager.Infraestructure.Repository;
using TaskManager.Transversal.Mapper;

namespace TaskManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IContextDefaultProvider, ContextDefaultProvider>();
            services.AddScoped(typeof(IEntityDomain<>), typeof(EntityDomain<>));
            
            services.AddAutoMapper(cfg => { }, typeof(MappingsProfile).Assembly);


            services.AddScoped<IAppService<TaskItemDTO, TaskItemVM>, AppService<TaskItemME, TaskItemDTO, TaskItemVM>>();
            // Repositorios como servicio
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLConnection"),
                    b => b.MigrationsAssembly("TaskManager.Infraestructure.Data")
                ));


            return services;
        }
    }
}
