using TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts;

namespace TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
