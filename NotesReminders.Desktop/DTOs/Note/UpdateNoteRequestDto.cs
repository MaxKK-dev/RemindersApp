using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Desktop.DTOs.Note;

public record UpdateNoteRequestDto(
    [property: Required, StringLength(50, MinimumLength = 3)]
    string Title,
    [property: StringLength(200)]
    string Content
);