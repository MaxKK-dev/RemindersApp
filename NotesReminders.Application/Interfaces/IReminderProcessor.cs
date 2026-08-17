namespace NotesReminders.Application.Interfaces;

public interface IReminderProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}