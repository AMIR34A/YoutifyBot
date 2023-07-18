namespace YoutifyBot.Models.Repository;

public interface IRepository<TEntity> : IDisposable
{
    Task<IEnumerable<TEntity>> GetAllUserAsync();
    Task CreateAsync(TEntity entity);
    Task<TEntity> FindByChatId(long chatId);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<int> Count();
}
