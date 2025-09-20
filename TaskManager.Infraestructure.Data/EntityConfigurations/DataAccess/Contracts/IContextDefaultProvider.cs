namespace TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts
{
    public interface IContextDefaultProvider
    {
        DateTime UtcNow { get; }
        Guid CurrentUserId { get; }
    }
}