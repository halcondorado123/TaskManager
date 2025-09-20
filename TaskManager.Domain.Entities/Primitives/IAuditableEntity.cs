namespace TaskManager.Domain.Entities.Primitives
{
    public interface IAuditableEntity<TUser>
            where TUser : IEquatable<TUser>
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        TUser CreatedBy { get; set; }
        TUser? UpdatedBy { get; set; }
    }
}