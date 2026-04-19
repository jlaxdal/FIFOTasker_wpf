using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.IO;

public class AppDbContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FIFOTasker.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.QueueOrder);
    }
}