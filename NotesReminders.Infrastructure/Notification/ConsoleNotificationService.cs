using NotesReminders.Domain.Entities;
using NotesReminders.Application.Interfaces;

namespace NotesReminders.Infrastructure.Notification;

public class ConsoleNotificationService : INotificationService
{
    public Task SendAsync(Reminder reminder)
    {
        Console.WriteLine(
            $"[{DateTime.Now:T}] Reminder: {reminder.Note.Title}");

        return Task.CompletedTask;
    }
}