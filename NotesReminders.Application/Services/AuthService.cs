using NotesReminders.Domain.Entities;
using NotesReminders.Application.DTOs.Auth;
using NotesReminders.Application.Exceptions;
using NotesReminders.Application.Interfaces;

namespace NotesReminders.Application.Services;

public class AuthService : IAuthService
{
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    public AuthService(
        IPasswordHasherService passwordHasherService,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _passwordHasherService = passwordHasherService;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists");
        }
        var user = new User
        {
            Username = request.Username,
        };

        var hashedPassword = _passwordHasherService.HashPassword(user, request.Password);

        user.PasswordHash = hashedPassword;
        _userRepository.Add(user);

        await _userRepository.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto (token, user.Username);
    }
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || 
            !_passwordHasherService.VerifyPassword(user, request.Password))
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto(token, user.Username);
    }

}
