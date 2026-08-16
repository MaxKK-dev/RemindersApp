using NotesReminders.Application.DTOs.Note;
using NotesReminders.Domain.Entities;

namespace NotesReminders.Application.Mappings;

public static class NoteMapper
{
    public static NoteResponseDto ToResponseDto(this Note note)
    {
        return new NoteResponseDto(
            note.Id,
            note.Title,
            note.Content,
            note.CreatedAt,
            note.ReminderTime,
            note.UpdatedAt,
            note.IsCompleted);
    }
    public static DeletedNoteResponseDto ToDeletedResponseDto(this Note note)
    {
        return new DeletedNoteResponseDto(
            note.Id,
            note.Title,
            note.Content,
            note.CreatedAt,
            note.ReminderTime,
            note.UpdatedAt,
            note.DeletedAt ?? throw new InvalidOperationException("Deleted note must have a deletion date."),
            note.IsCompleted);
    }
}