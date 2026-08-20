namespace NotesReminders.Desktop.DTOs.Reminder;

public record ReminderResponseDto
(
    int Id,
    int NoteId,
    DateTime NotifyAt
);