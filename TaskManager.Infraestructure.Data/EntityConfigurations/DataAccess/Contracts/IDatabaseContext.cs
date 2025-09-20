using Microsoft.EntityFrameworkCore;

namespace TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts
{
    public interface IDatabaseContext
    {
        DbSet<TDbSet> Repository<TDbSet>() where TDbSet : class;
    }
}