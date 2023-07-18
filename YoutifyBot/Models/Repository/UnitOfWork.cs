namespace YoutifyBot.Models.Repository;

public class UnitOfWork : IUnitOfWork
{
    private bool disposedValue;
    YoutifyBotContext context;

    public UnitOfWork(YoutifyBotContext context)
    {
        this.context = context;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class => new Repository<TEntity>(context);

    public async Task SaveAsync() => await context.SaveChangesAsync();

    public async Task CreateDatabase() => await context.Database.EnsureCreatedAsync();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                context.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
