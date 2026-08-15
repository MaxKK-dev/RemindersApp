using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
        entity.Property(u => u.PasswordHash).IsRequired();

        entity.ToTable("Users");
    }
}