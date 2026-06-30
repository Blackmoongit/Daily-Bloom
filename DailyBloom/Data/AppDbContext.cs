using DailyBloom.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyBloom.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    public DbSet<IncomeEntry> Incomes => Set<IncomeEntry>();
    public DbSet<ExpenseEntry> Expenses => Set<ExpenseEntry>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.SubTasks).WithOne(s => s.TaskItem!).HasForeignKey(s => s.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.Comments).WithOne(c => c.TaskItem!).HasForeignKey(c => c.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.Attachments).WithOne(a => a.TaskItem!).HasForeignKey(a => a.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>().Property(t => t.Priority).HasConversion<string>();
    }
}
