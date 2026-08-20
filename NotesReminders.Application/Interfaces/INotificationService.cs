using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(Reminder reminder);
}