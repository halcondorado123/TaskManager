using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManager.Domain.Entities.Primitives;
using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;

namespace TaskManager.Infraestructure.Data.EntityConfigurations.Interceptors
{
    public class EntityConfigurationInterceptor<TContext>(IContextDefaultProvider contextDefaultProvider) : SaveChangesInterceptor
           where TContext : DbContext
    {
        private readonly IContextDefaultProvider _contextDefaultProvider = contextDefaultProvider;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not TContext context)
                return result;
            SetConfigurationInterception(context);
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not TContext context)
                return result;
            SetConfigurationInterception(context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetConfigurationInterception(TContext context)
        {
            foreach (EntityEntry entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    // Lógica para el estado Added
                    if (IsAuditable(entry))
                    {
                        entry.Property("CreatedAt").CurrentValue = _contextDefaultProvider.UtcNow;
                        entry.Property("CreatedBy").CurrentValue = _contextDefaultProvider.CurrentUserId;
                    }
                    if (IsSoftDeleteable(entry))
                    {
                        entry.Property("IsDeleted").CurrentValue = false;
                        entry.Property("DeletedBy").CurrentValue = Guid.Empty;
                        entry.Property("DeletedAt").CurrentValue = null;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Lógica para el estado Modified
                    if (IsAuditable(entry))
                    {
                        entry.Property("UpdatedAt").CurrentValue = _contextDefaultProvider.UtcNow;
                        entry.Property("UpdatedBy").CurrentValue = _contextDefaultProvider.CurrentUserId;
                    }
                    if (IsSoftDeleteable(entry))
                    {
                        // Asegura que no se están borrando en una actualización
                        if ((bool)entry.Property("IsDeleted").CurrentValue)
                        {
                            entry.Property("DeletedAt").CurrentValue = _contextDefaultProvider.UtcNow;
                            entry.Property("DeletedBy").CurrentValue = _contextDefaultProvider.CurrentUserId;
                        }
                        else
                        {
                            entry.Property("DeletedAt").CurrentValue = null;
                            entry.Property("DeletedBy").CurrentValue = Guid.Empty;
                        }
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    if (IsSoftDeleteable(entry))
                    {
                        entry.State = EntityState.Modified;
                        entry.Property("IsDeleted").CurrentValue = true;
                        entry.Property("DeletedBy").CurrentValue = _contextDefaultProvider.CurrentUserId;
                        entry.Property("DeletedAt").CurrentValue = _contextDefaultProvider.UtcNow;
                        entry.Property("Active").CurrentValue = false;
                        SetConfigurationNestedInterception(entry);
                    }
                }
            }
        }

        // Los métodos auxiliares no necesitan cambios
        private static bool IsAuditable(EntityEntry entry)
        {
            return entry.Metadata.ClrType.IsAssignableTo(typeof(IAuditableEntity<int>)) ||
                   entry.Metadata.ClrType.IsAssignableTo(typeof(IAuditableEntity<Guid>)) ||
                   entry.Metadata.ClrType.IsAssignableTo(typeof(IAuditableEntity<string>));
        }

        private static bool IsSoftDeleteable(EntityEntry entry)
        {
            return entry.Metadata.ClrType.IsAssignableTo(typeof(ISoftDeleteableEntity<int>)) ||
                   entry.Metadata.ClrType.IsAssignableTo(typeof(ISoftDeleteableEntity<Guid>)) ||
                   entry.Metadata.ClrType.IsAssignableTo(typeof(ISoftDeleteableEntity<string>));
        }

        private static void SetConfigurationNestedInterception(EntityEntry entry)
        {
            foreach (var reference in entry.References)
            {
                if (reference.TargetEntry == null)
                    continue;

                if (IsSoftDeleteable(reference.TargetEntry))
                {
                    reference.TargetEntry.State = EntityState.Unchanged;
                }
            }
        }
    }
}
