using TaskManager.Application.Service;
using TaskManager.Domain.Core;
using TaskManager.Domain.Interface;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Repositories;
using TaskManager.Transversal.Mapper;

namespace TaskManager.ServiceExtensions
{
    public static class DomainServicesExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEntityDomain<>), typeof(EntityDomain<>));
            services.AddAutoMapper(cfg => { }, typeof(MappingsProfile).Assembly);
            services.AddScoped<ISweetAlertService, SweetAlertService>();
            services.AddScoped<IContextDefaultProvider, ContextDefaultProvider>();

            return services;
        }
    }
}