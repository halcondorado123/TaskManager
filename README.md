# TaskManager

## Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Arquitectura del Proyecto](#arquitectura-del-proyecto)
- [Tecnologías y Paquetes NuGet](#tecnologías-y-paquetes-nuget)
- [Configuración de Entity Framework](#configuración-de-entity-framework)
- [Configuración de AutoMapper](#configuración-de-automapper)
- [Patrones de Repositorio](#patrones-de-repositorio)
- [Servicios de Aplicación](#servicios-de-aplicación)
- [Configuración de la Aplicación](#configuración-de-la-aplicación)
- [Estructura de la Interfaz de Usuario](#estructura-de-la-interfaz-de-usuario)
- [Testing](#testing)
- [Instalación y Configuración](#instalación-y-configuración)
- [Comandos Útiles](#comandos-útiles)
- [Características Implementadas](#características-implementadas)
- [Contribución](#contribución)

## Descripción General

TaskManager es una aplicación web empresarial desarrollada con .NET 8 que implementa una arquitectura limpia y escalable para la gestión integral de tareas y usuarios. La aplicación utiliza patrones de diseño modernos, Entity Framework Core para persistencia de datos, y Blazor Server para proporcionar una experiencia de usuario rica e interactiva.

**Valor de Negocio:**
- Gestión centralizada de tareas y proyectos
- Sistema de autenticación y autorización robusto
- Interfaz moderna y responsive
- Arquitectura escalable para crecimiento futuro

## Arquitectura del Proyecto

### Estructura de Capas

El proyecto implementa **Clean Architecture** con separación clara de responsabilidades:

```
TaskManager/
├── TaskManager (Main)/                    # 🎨 Capa de Presentación (Blazor Server)
├── TaskManager.Application.Service/       # ⚙️ Servicios de Aplicación
├── TaskManager.Application.Interface/     # 📋 Interfaces de Aplicación
├── TaskManager.Application.DTO/           # 📦 Data Transfer Objects
├── TaskManager.Domain.Entities/           # 🏛️ Entidades del Dominio
├── TaskManager.Domain.Core/               # 💎 Núcleo del Dominio
├── TaskManager.Domain.Interface/          # 📋 Interfaces del Dominio
├── TaskManager.Infrastructure.Data/       # 🗄️ Acceso a Datos
├── TaskManager.Infrastructure.Repository/ # 📚 Repositorios
├── TaskManager.Infrastructure.Interface/  # 📋 Interfaces de Infraestructura
├── TaskManager.Transversal.Mapper/        # 🔄 Servicios Transversales
└── TaskManager.Testing.Test/              # 🧪 Pruebas Unitarias
```

### Principios SOLID Implementados

- **Single Responsibility Principle (SRP)** - Cada clase tiene una única razón para cambiar
- **Open/Closed Principle (OCP)** - Abierto para extensión, cerrado para modificación
- **Liskov Substitution Principle (LSP)** - Las implementaciones son intercambiables
- **Interface Segregation Principle (ISP)** - Interfaces específicas y cohesivas
- **Dependency Inversion Principle (DIP)** - Dependencias hacia abstracciones

### Patrones de Diseño
- **Repository Pattern** - Abstracción del acceso a datos
- **Dependency Injection** - Inversión de control nativa de .NET
- **DTO Pattern** - Transferencia de datos entre capas

## Tecnologías y Paquetes NuGet

### Framework Principal
- **.NET 8.0** - Framework principal de la aplicación
- **ASP.NET Core 8.0** - Para APIs y servicios web
- **Blazor Server** - Para la interfaz de usuario interactiva

### Base de Datos y ORM
- **Microsoft.EntityFrameworkCore (8.0.20)** - ORM principal
- **Microsoft.EntityFrameworkCore.InMemory (8.0.20)** - Base de datos en memoria para testing
- **Microsoft.EntityFrameworkCore.Tools (8.0.20)** - Herramientas de EF Core
- **Microsoft.EntityFrameworkCore.Design (8.0.20)** - Herramientas de diseño para migraciones

### Autenticación y Autorización
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.20)** - Sistema de identidad
- **Microsoft.AspNetCore.Components.Authorization (8.0.20)** - Autorización para Blazor
- **Microsoft.Identity.Model.Tokens (8.14.0)** - Manejo de tokens de seguridad
- **System.IdentityModel.Tokens.Jwt (8.14.0)** - Tokens JWT

### Mapeo de Objetos
- **AutoMapper (13.0.1)** - Mapeo automático entre DTOs y entidades

### Testing
- **Microsoft.NET.Test.Sdk (17.8.0)** - SDK de testing
- **coverlet.collector (6.0.0)** - Cobertura de código
- **xunit (2.5.3)** - Framework de pruebas unitarias
- **xunit.runner.visualstudio (2.5.3)** - Runner de xUnit para Visual Studio
- **Moq (4.20.72)** - Framework de mocking

### Herramientas de Desarrollo
- **Microsoft.AspNetCore.Components (8.0.20)** - Componentes de Blazor

## Configuración de Entity Framework

### Contexto de Base de Datos

```csharp
public class TaskManagerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IDatabaseContext
{
    private readonly IContextDefaultProvider _contextDefaultProvider;
    
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options, IContextDefaultProvider contextDefaultProvider)
        : base(options)
    {
        _contextDefaultProvider = contextDefaultProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);
        
        // Filtro global SoftDelete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var implementedInterface = entityType.ClrType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(ISoftDeleteableEntity<>));
            if (implementedInterface is not null)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, "IsDeleted");
                var condition = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new EntityConfigurationInterceptor<TaskManagerDbContext>(_contextDefaultProvider));
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<TDbSet> Repository<TDbSet>() where TDbSet : class => Set<TDbSet>();
}
```

### Características Avanzadas del DbContext

- **Identity con Guid**: Utiliza `ApplicationUser` y `ApplicationRole` con identificadores GUID
- **Soft Delete**: Filtro global automático para entidades que implementan `ISoftDeleteableEntity<>`
- **Interceptores**: `EntityConfigurationInterceptor` para auditoría automática
- **Configuraciones automáticas**: Aplicación automática de configuraciones de entidades
- **Patrón Repository**: Método genérico `Repository<T>()` para acceso a DbSets

### Configuraciones de Entidades

Las configuraciones se encuentran en `TaskManager.Infrastructure.Data/EntityConfigurations/`:

- **ApplicationUserConfiguration.cs** - Configuración de usuarios
- **ApplicationRoleConfiguration.cs** - Configuración de roles
- **TaskItemConfiguration.cs** - Configuración de tareas
- **TaskStatusConfiguration.cs** - Configuración de estados

### Migraciones

```bash
# Crear migración
dotnet ef migrations add InitCreate --project TaskManager.Infrastructure.Data

# Actualizar base de datos
dotnet ef database update --project TaskManager.Infrastructure.Data
```

## Configuración de AutoMapper

### Perfiles de Mapeo

Ubicados en `TaskManager.Transversal.Mapper/`:

```csharp
public class MappingProfile : Profile
{
   public MappingsProfile()
    {
        CreateMap<TaskItemDTO, TaskItemME>();
        CreateMap<TaskItemME, TaskItemVM>()
            .ForMember(dest => dest.StatusName,
                opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : string.Empty));
    
        CreateMap<TaskItemDTO, TaskItemME>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    
        CreateMap<TaskStatusME, TaskStatusVM>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    
        CreateMap<TaskStatusDTO, TaskStatusME>();
        CreateMap<TaskStatusME, TaskStatusDTO>();
    }
}
```

### Registro en DI Container

```csharp
services.AddAutoMapper(typeof(MappingProfile));
```

## Patrones de Repositorio

### Repositorio Base

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Repositorios Específicos

- **IWriteRepository** - Operaciones específicas de escritura generica
- **IReadRepository** - Operaciones específicas de lectura generica

## Servicios de Aplicación

### Estructura de Servicios

```
TaskManager.Application.Service/
├── AppService.cs          # Servicio base
├── AuthService.cs         # Autenticación
├── CustomAuthenticationStateProvider.cs
├── BlazorIdentityExtensions.cs
├── IdentityExtensions.cs
└── MvcServicesExtensions.cs
```

### Inyección de Dependencias

```csharp
// Servicios de aplicación
services.AddScoped<IAppService, AppService>();
services.AddScoped<IAuthService, AuthService>();

// Repositorios
services.AddScoped<IReadRepository, ReadRepository>();
services.AddScoped<IWriteRepository, WriteRepository>();
```

## Configuración de la Aplicación

### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagerDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
};
```

### Program.cs - Configuración de Servicios

Cadena de extensiones: Configuración limpia y organizada
Migración automática: await app.MigrateAndSeedAsync()
Pipeline completo: Middleware ordenado correctamente
Blazor interactivo: AddInteractiveServerRenderMode()

```csharp
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
```

## Estructura de la Interfaz de Usuario

### Páginas Principales

```
Components/
├── Layout/
│   ├── EmptyLayout.razor
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── Pages/
    ├── Error.razor
    ├── Home.razor
    ├── Index.razor
    └── Session/
        └── Register.razor
```

### Componentes de Autenticación

- **Register.razor** - Registro de usuarios con validación
- **Login.razor** - Inicio de sesión
- **Custom Authentication State Provider** - Manejo de estado de autenticación

## Extensiones y Helpers

### Service Extensions

```csharp
public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registro de servicios de aplicación
        return services;
    }
}
```

### Context Extensions

```csharp
public static class DbContextExtensions
{
    public static void SeedData(this TaskManagerDbContext context)
    {
        // Datos de prueba
    }
}
```

## Testing

### Estructura de Pruebas

```
TaskManager.Testing.Test/
├── Helpers/
│   ├── DbContextHelper.cs
│   └── FakeContextDefaultProvider.cs
├── Repositories/
│   └── RepositoryTests.cs
└── Services/
    └── AppServiceTests.cs
```

### Configuración de Testing

```csharp
public class TestBase
{
    protected TaskManagerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        return new TaskManagerDbContext(options);
    }
}
```

## Instalación y Configuración

### Prerrequisitos

- .NET 8.0 SDK
- SQL Server (LocalDB o completo)
- Visual Studio 2022 o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/TaskManager.git
   cd TaskManager
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar cadena de conexión**
   - Editar `appsettings.json` con tu cadena de conexión

4. **Crear y aplicar migraciones**
   ```bash
   dotnet ef database update --project TaskManager.Infrastructure.Data
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project TaskManager
   ```

## Comandos Útiles

### Entity Framework

```bash
# Crear migración
dotnet ef migrations add NombreMigracion --project TaskManager.Infrastructure.Data

# Aplicar migraciones
dotnet ef database update --project TaskManager.Infrastructure.Data

# Revertir migración
dotnet ef database update PreviousMigration --project TaskManager.Infrastructure.Data

# Eliminar última migración
dotnet ef migrations remove --project TaskManager.Infrastructure.Data
```

### Testing

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar pruebas específicas
dotnet test --filter "ClassName=AppServiceTests"
```

### Build y Deploy

```bash
# Build en modo Release
dotnet build --configuration Release

# Publicar aplicación
dotnet publish --configuration Release --output ./publish
```

## Características Implementadas

### Autenticación y Autorización
- Sistema de registro y login
- Roles y permisos
- Protección de rutas

### Gestión de Tareas
- CRUD completo de tareas
- Estados personalizables
- Asignación de usuarios
- Filtros y búsquedas

### Arquitectura
- Clean Architecture
- Repository Pattern
- Dependency Injection
- AutoMapper

### Testing
- Pruebas unitarias
- Mocking con Moq
- Base de datos en memoria
- Cobertura de código

## Contribución

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Crear un Pull Request

## Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para detalles.

Link del Proyecto: [https://github.com/tu-usuario/TaskManager](https://github.com/tu-usuario/TaskManager)

## Contacto

- **Nombre:** Jhonattan Halcón Casallas Felipe
- **LinkedIn:** [https://www.linkedin.com/in/jhonattanhalconcasallasfelipe/](https://www.linkedin.com/in/jhonattanhalconcasallasfelipe/)
- **GitHub:** [https://github.com/halcondorado123 ](https://github.com/halcondorado123 )
- **Correo:** [falconfelipedeveloper@gmail.com](mailto:falconfelipedeveloper@gmail.com)
