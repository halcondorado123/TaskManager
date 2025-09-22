using TaskManager.Infraestructure.Interface;
using TaskManager.Infraestructure.Repository;

namespace TaskManager.ServiceExtensions
{
    public static class RepositoryServicesExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            return services;
        }
    }
}