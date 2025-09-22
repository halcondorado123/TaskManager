using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TaskManager.Domain.Entities.Models.Identity;

namespace TaskManager.Infraestructure.Data.Configuration.Identity.Seeder
{
    public static class ApplicationUserSeed
    {
        public static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager)
        {
            var users = new[]
            {
                new { Email = "superadmin@taskmanager.com", FullName = "Super Administrador", Role = "SuperAdmin" },
                new { Email = "supervisor@taskmanager.com", FullName = "Supervisor", Role = "Supervisor" },
                new { Email = "user1@taskmanager.com", FullName = "Asesor", Role = "Asesor" },
                new { Email = "user2@taskmanager.com", FullName = "Auditor", Role = "Auditor" },
                new { Email = "user3@taskmanager.com", FullName = "Otro", Role = "Otro" }
            };

            foreach (var u in users)
            {
                var user = await userManager.FindByEmailAsync(u.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.FullName,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "User123!"); // contraseña por defecto
                    await userManager.AddToRoleAsync(user, u.Role);
                }
            }
        }
    }
}
