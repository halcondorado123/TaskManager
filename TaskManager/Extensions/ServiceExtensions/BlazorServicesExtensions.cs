using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManager.Application.Service;

public static class BlazorServicesExtensions
{
    public static IServiceCollection AddBlazorServices(this IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();

        // Registrar HttpClientFactory
        services.AddHttpClient();

        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            var httpClient = httpClientFactory.CreateClient("default");
            httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
            return httpClient;
        });

        services.AddScoped<CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CustomAuthenticationStateProvider>());

        return services;
    }
}
