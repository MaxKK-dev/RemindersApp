using NotesReminders.Application.Interfaces;

namespace NotesReminders.Application.Services;

public class ReminderProcessor : IReminderProcessor
{
    private readonly IReminderRepository _reminderRepository;
    private readonly INotificationService _notificationService;

    public ReminderProcessor(
        IReminderRepository reminderRepository,
        INotificationService notificationService)
    {
        _reminderRepository = reminderRepository;
        _notificationService = notificationService;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var reminders = await _reminderRepository
            .GetDueRemindersAsync(DateTime.UtcNow);

        foreach (var reminder in reminders)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _notificationService.SendAsync(reminder);
        }

        _reminderRepository.RemoveRange(reminders);

        await _reminderRepository.SaveChangesAsync();
    }
}