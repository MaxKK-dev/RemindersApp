using CommunityToolkit.Mvvm.ComponentModel;
using NotesReminders.Desktop.DTOs.Reminder;

namespace NotesReminders.Desktop.ViewModels.Notes;

public partial class ReminderItemViewModel : ObservableObject
{
    public int Id { get; }

    public int NoteId { get; }

    [ObservableProperty]
    private DateTimeOffset? date;

    [ObservableProperty]
    private TimeSpan time;

    public ReminderItemViewModel(ReminderResponseDto reminder)
    {
        Id = reminder.Id;
        NoteId = reminder.NoteId;

        var localDateTime = reminder.NotifyAt.ToLocalTime();

        Date = new DateTimeOffset(localDateTime.Date);
        Time = localDateTime.TimeOfDay;
    }
}