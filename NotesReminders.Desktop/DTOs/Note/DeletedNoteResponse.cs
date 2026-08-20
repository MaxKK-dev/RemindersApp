using NotesReminders.Desktop.DTOs.Reminder;

namespace NotesReminders.Desktop.DTOs.Note;

public record DeletedNoteResponseDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime DeletedAt,
    bool IsComlete,
    ICollection<ReminderResponseDto> Reminders


);