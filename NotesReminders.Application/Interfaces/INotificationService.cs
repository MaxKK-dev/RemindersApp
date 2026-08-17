using NotesReminders.Domain.Entities;

namespace NotesReminders.Infrastructure.Notification;

public interface INotificationService
{
    Task SendAsync(Reminder reminder);
}