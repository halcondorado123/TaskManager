using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interface;
using TaskManager.Application.Service;
using TaskManager.Components;
using TaskManager.Extensions;
using TaskManager.Transversal.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<ISweetAlertService, SweetAlertService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();