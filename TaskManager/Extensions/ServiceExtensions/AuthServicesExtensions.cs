using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;

namespace TaskManager.ServiceExtensions
{
    public static class AuthServicesExtensions
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}