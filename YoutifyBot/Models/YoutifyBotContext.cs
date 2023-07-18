using Microsoft.EntityFrameworkCore;

namespace YoutifyBot.Models;

public class YoutifyBotContext : DbContext
{
    public YoutifyBotContext(DbContextOptions contextOptions) : base(contextOptions)
    {
    }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.ChatId);
        modelBuilder.Entity<User>().Property(user => user.ChatId).ValueGeneratedNever();
        base.OnModelCreating(modelBuilder);
    }
}
