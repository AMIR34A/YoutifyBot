namespace YoutifyBot.Models.Repository;

public interface IRepository<TEntity> : IDisposable
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task CreateAsync(TEntity entity);
    Task<TEntity> GetFirstAsync();
    Task<TEntity> FindByChatIdAsync(long chatId);
    Task<bool> AynAsync(long chatId);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<int> Count();
}
