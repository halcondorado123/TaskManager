using Microsoft.EntityFrameworkCore;
using TaskManager.Infraestructure.Data;

namespace TaskManager.ServiceExtensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLConnection"),
                    b => b.MigrationsAssembly("TaskManager.Infraestructure.Data")
                ));

            return services;
        }
    }
}