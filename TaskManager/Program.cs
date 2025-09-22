using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Service;
using TaskManager.Components;
using TaskManager.Domain.Entities.Models.Identity;
using TaskManager.Extensions;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Data.Configuration.Identity.Seeder;

var builder = WebApplication.CreateBuilder(args);

// Servicios requeridos por Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar HttpClient con BaseAddress
builder.Services.AddHttpClient("default", client =>
{
    // En desarrollo, esto se configurará automáticamente
    // En producción, asegúrate de que coincida con tu dominio
});

builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var httpClient = httpClientFactory.CreateClient("default");
    httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
    return httpClient;
});
builder.Services.AddControllers();

// *** IMPORTANTE: El orden de estos servicios importa ***
// Primero los servicios de aplicación (incluye Identity)
builder.Services.AddApplicationServices(builder.Configuration);


// AuthorizationCore se agrega después
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();


// Sweet Alert Service
builder.Services.AddScoped<ISweetAlertService, SweetAlertService>();

var app = builder.Build();

// Seeders y migraciones
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TaskManagerDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await context.Database.MigrateAsync();
        await ApplicationUserSeed.SeedUsersAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante la migración o seeding");
    }
}

// *** Middleware en orden correcto ***
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Autenticación ANTES que autorización
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Mapear controllers ANTES que Blazor
app.MapControllers();

// Mapear Blazor Components al final
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();