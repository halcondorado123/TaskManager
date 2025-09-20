using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;

namespace TaskManager.Testing.Test.Helpers
{
    internal class FakeContextDefaultProvider : IContextDefaultProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public Guid CurrentUserId => Guid.NewGuid();
    }
}