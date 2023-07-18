using Microsoft.EntityFrameworkCore;

namespace YoutifyBot.Models.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private bool disposedValue;
        YoutifyBotContext context;
        public Repository(YoutifyBotContext context)
        {
            this.context = context;
        }
        public async Task<int> Count() => await context.Users.CountAsync();

        public async Task CreateAsync(TEntity entity) => await context.AddAsync<TEntity>(entity);

        public void Delete(TEntity entity) => context.Remove(entity);

        public async Task<TEntity> FindByChatId(long chatId) => await context.FindAsync<TEntity>(chatId);

        public async Task<IEnumerable<TEntity>> GetAllUserAsync() => await context.Set<TEntity>().ToListAsync();

        public void Update(TEntity entity) => context.Update<TEntity>(entity);

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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
