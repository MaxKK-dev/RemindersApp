using Microsoft.EntityFrameworkCore;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users {get; set; }
    public DbSet<Note> Notes {get; set; }
    public DbSet<Reminder> Reminders {get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}