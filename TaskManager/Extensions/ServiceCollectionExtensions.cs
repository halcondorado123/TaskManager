using Microsoft.EntityFrameworkCore;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Repositories;

namespace TaskManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IContextDefaultProvider, ContextDefaultProvider>();


            //services.AddScoped(typeof(IEntityDomain<>), typeof(EntityDomain<>));

            //services.AddScoped<IAppService<MasterDTO, ResponseVM>, AppService<IdentificationTypeME, MasterDTO, ResponseVM>>();
            //services.AddScoped<IAppService<BillDTO, ResponseBillInfoVM>, AppService<BillInformationME, BillDTO, ResponseBillInfoVM>>();
            //services.AddScoped<IAppService<ProductDetailDTO, ResponseProductDetailVM>, AppService<ProductDetailME, ProductDetailDTO, ResponseProductDetailVM>>();
            //services.AddAutoMapper(cfg => { }, typeof(MappingsProfile).Assembly);

            ////services.AddScoped(typeof(IAppService<>), typeof(AppService<>));

            //// Registrar los repositorios como servicio
            //services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            //services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLConnection"),
                    b => b.MigrationsAssembly("TaskManager.Infraestructure.Data")
                ));


            return services;
        }
    }
}
