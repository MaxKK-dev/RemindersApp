using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Note;

public record UpdateNoteRequestDto
{
    [property: Required, StringLength(50, MinimumLength = 3)]
    public string Title {get; set; } = string.Empty;
    [property: StringLength(200)]
    public string Content {get; set; } = string.Empty;
}