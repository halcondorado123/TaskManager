namespace TaskManager.Domain.Entities.Primitives
{
    public interface ISoftDeleteableEntity<TUser>
            where TUser : IEquatable<TUser>
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public TUser? DeletedBy { get; set; }
        public bool Active { get; set; }
    }
}
