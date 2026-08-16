using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Auth;
public record AuthResponseDto
(
    [property: Required]
    string Token,
    [property: Required]
    string UserName
);