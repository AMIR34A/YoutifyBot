namespace YoutifyBot.Models.Repository;

public interface IUnitOfWork:IDisposable
{
    Task SaveAsync();
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task CreateDatabase();
}
