using TaskManager.Components;
using TaskManager.Extensions;
using TaskManager.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddBlazorServices()
    .AddDomainServices()
    .AddApplicationServicesLayer()
    .AddRepositoryServices()
    .AddDbContextServices(builder.Configuration)
    .AddCustomIdentity()
    .AddAuthServices()
    .AddMvcServices();

var app = builder.Build();

await app.MigrateAndSeedAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();