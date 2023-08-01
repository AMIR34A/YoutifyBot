using Microsoft.EntityFrameworkCore;

namespace YoutifyBot.Models;

public class YoutifyBotContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Rule> Rules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(local);Database=YoutifyBotDb;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=true");
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.ChatId);
        modelBuilder.Entity<User>().Property(user => user.ChatId).ValueGeneratedNever();
        base.OnModelCreating(modelBuilder);
    }
}
