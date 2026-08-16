
namespace NotesReminders.Application.DTOs.Note;

public record DeletedNoteResponseDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? ReminderTime,
    DateTime? UpdatedAt,
    DateTime DeletedAt,
    bool IsComlete

);