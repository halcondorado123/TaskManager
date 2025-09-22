using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities.Models.Identity;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Data.Configuration.Identity.Seeder;

namespace TaskManager.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task MigrateAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<TaskManagerDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                // Migraciones pendientes
                await context.Database.MigrateAsync();

                // Seed de usuarios
                await ApplicationUserSeed.SeedUsersAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocurrió un error durante la migración o seeding");
            }
        }
    }
}
