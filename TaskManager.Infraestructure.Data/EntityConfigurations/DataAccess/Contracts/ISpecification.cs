namespace TaskManager.Infraestructure.Data.EntityConfigurations.DataAccess.Contracts
{
    public interface ISpecification<TQuery, TResult>
           where TQuery : class
           where TResult : class
    {
        IQueryable<TResult> Apply(IQueryable<TQuery> query);
    }
}