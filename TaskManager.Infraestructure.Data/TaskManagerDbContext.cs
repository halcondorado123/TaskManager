using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities.Primitives;
using TaskManager.Infraestructure.Data.EntityConfigurations.Conventions;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;
using TaskManager.Infraestructure.Data.EntityConfigurations.Interceptors;

namespace TaskManager.Infraestructure.Data
{
    public class TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options, IContextDefaultProvider contextDefaultProvider)
            : DbContext(options), IUnitOfWork, IDatabaseContext
    {
        private readonly IContextDefaultProvider _contextDefaultProvider = contextDefaultProvider;

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new EntityConfigurationConvention());
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);

            // Filtro global de SoftDelete
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