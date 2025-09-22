using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Application.DTO;
using TaskManager.Application.DTO.DTO;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;
using TaskManager.Domain.Core;
using TaskManager.Domain.Entities.Models;
using TaskManager.Domain.Entities.Models.Identity;
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
            services.AddScoped<IAppService<TaskStatusDTO, TaskStatusVM>, AppService<TaskStatusME, TaskStatusDTO, TaskStatusVM>>();
            services.AddScoped<IAppService<TaskItemDTO, TaskItemVM>, AppService<TaskItemME, TaskItemDTO, TaskItemVM>>();

            // Repositorios como servicio
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

            services.AddDbContext<TaskManagerDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SQLConnection"),
                    b => b.MigrationsAssembly("TaskManager.Infraestructure.Data")
                ));

            // Configurar Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Configuración de contraseñas
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Configuración de usuario
                options.User.RequireUniqueEmail = true;

                // Configuración de lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<TaskManagerDbContext>()
            .AddDefaultTokenProviders();

            // Configurar autenticación con cookies para Blazor Server
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/api/account/logout"; // Importante: debe coincidir con tu controller
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromDays(1); // Aumentar tiempo de expiración
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;

                // Eventos para debugging
                options.Events.OnRedirectToLogin = context =>
                {
                    // Si es una llamada API, devolver 401 en lugar de redirigir
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}