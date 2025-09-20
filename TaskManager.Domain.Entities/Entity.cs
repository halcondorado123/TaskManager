using TaskManager.Domain.Entities.Primitives;

namespace TaskManager.Domain.Entities
{
    public abstract class Entity<TKey> : IBaseEntity<TKey>, IAuditableEntity<Guid>, ISoftDeleteableEntity<Guid>
    where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid DeletedBy { get; set; }
        public bool Active { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}