namespace TaskManager.Domain.Entities.Primitives
{
    public interface IBaseEntity<TKey>
              where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}
