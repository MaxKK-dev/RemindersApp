using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Note;

public record CreateNoteRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Title { get; init; } = string.Empty;

    [StringLength(200)]
    public string Content { get; init; } = string.Empty;
}