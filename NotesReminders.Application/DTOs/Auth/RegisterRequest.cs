using System.ComponentModel.DataAnnotations;

namespace NotesReminders.Application.DTOs.Auth;

public record RegisterRequestDto
(
    [param: Required, StringLength(50, MinimumLength = 3)]
    string Username, 
    [param: Required, StringLength(100, MinimumLength = 6)]
    string Password
);