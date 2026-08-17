using NotesReminders.Application.DTOs.Reminder;

namespace NotesReminders.Application.DTOs.Note;

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