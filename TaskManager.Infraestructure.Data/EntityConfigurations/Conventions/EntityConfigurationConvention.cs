using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TaskManager.Domain.Entities.Primitives;

namespace TaskManager.Infraestructure.Data.EntityConfigurations.Conventions
{
    public sealed class EntityConfigurationConvention : IModelFinalizingConvention
    {
        private readonly List<Action<IEnumerable<IConventionEntityType>>> _actions =
        [
          entities => entities
              .Where(entity => entity.ClrType is { IsAbstract: false, IsInterface: false } && entity.ClrType.IsAssignableTo(typeof(IBaseEntity<>)))
              .ToList()
              .ForEach(entity =>
              {
                IConventionProperty idProperty = entity.FindProperty(nameof(IBaseEntity<int>.Id)) ?? throw new NullReferenceException();
                idProperty.SetValueGenerated(ValueGenerated.OnUpdate);
                if(idProperty.ClrType == typeof(Guid))
                  idProperty.SetDefaultValueSql("newsequentialid()");
              }),
            entities => entities
              .Where(entity => entity.ClrType is { IsAbstract: false, IsInterface: false } && entity.ClrType.IsAssignableTo(typeof(IAuditableEntity<>)))
              .ToList()
              .ForEach(entity =>
              {
                IConventionProperty createdAtUtc = entity.FindProperty(nameof(IAuditableEntity<int>.CreatedAt)) ?? throw new NullReferenceException();
                IConventionProperty createdBy = entity.FindProperty(nameof(IAuditableEntity<int>.CreatedBy)) ?? throw new NullReferenceException();
                createdAtUtc.SetDefaultValueSql("getutcdate()");
                if(createdBy.ClrType == typeof(Guid))
                  createdBy.SetDefaultValue(Guid.Empty);
              }),
            entities => entities
              .Where(entity => entity.ClrType is { IsAbstract: false, IsInterface: false } && entity.ClrType.IsAssignableTo(typeof(ISoftDeleteableEntity<>)))
              .ToList()
              .ForEach(entity =>
              {
                IConventionProperty isDeleted = entity.FindProperty(nameof(ISoftDeleteableEntity<int>.IsDeleted)) ?? throw new NullReferenceException();
                isDeleted.SetDefaultValue(false);
              }),
          ];
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
          => _actions.ForEach(action => action(modelBuilder.Metadata.GetEntityTypes()));

    }
}
