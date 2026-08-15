using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Infrastructure.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> entity)
    {
        entity.HasKey(n => n.Id);

        entity.Property(n => n.Title).IsRequired().HasMaxLength(50);
        entity.Property(n => n.Content).HasMaxLength(500);
        entity.HasIndex(n => n.ReminderTime);

        entity.HasOne(n => n.User)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.ToTable("Notes");
    }
}
