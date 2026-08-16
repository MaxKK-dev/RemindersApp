using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Auth;
public record LoginRequestDto
(
    [property: Required, StringLength(50, MinimumLength = 3)]
    string Username, 
    [property: Required, StringLength(100, MinimumLength = 6)]
    string Password
);