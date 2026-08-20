using NotesReminders.Domain.Entities;
using NotesReminders.Desktop.DTOs.Reminder;

namespace NotesReminders.Desktop.DTOs.Note;

public record NoteResponseDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsComlete,
    ICollection<ReminderResponseDto> Reminders

);