namespace TaskManager.ServiceExtensions
{
    public static class MvcServicesExtensions
    {
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }
    }
}