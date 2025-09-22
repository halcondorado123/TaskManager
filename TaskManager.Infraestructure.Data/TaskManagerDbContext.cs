using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities.Models.Identity;
using TaskManager.Domain.Entities.Primitives;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;
using TaskManager.Infraestructure.Data.EntityConfigurations.Interceptors;

namespace TaskManager.Infraestructure.Data
{
    public class TaskManagerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IUnitOfWork, IDatabaseContext
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

        public DbSet<TDbSet> Repository<TDbSet>() where TDbSet : class
            => Set<TDbSet>();
    }
}
