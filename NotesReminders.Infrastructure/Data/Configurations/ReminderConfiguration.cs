using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Infrastructure.Data.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> entity)
    {
        entity.HasKey(r => r.Id);

        entity.HasOne(r => r.Note)
            .WithMany(n => n.Reminders)
            .HasForeignKey(r => r.NoteId)
            .OnDelete(DeleteBehavior.Cascade);
            
        entity.ToTable("Reminders");
    }
}