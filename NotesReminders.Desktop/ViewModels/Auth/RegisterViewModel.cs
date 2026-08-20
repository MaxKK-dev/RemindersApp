using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesReminders.Desktop.DTOs.Auth;
using NotesReminders.Desktop.Navigation;
using NotesReminders.Desktop.Services;

namespace NotesReminders.Desktop.ViewModels.Auth;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly NavigationService _navigation;

    public RegisterViewModel(
        AuthService authService,
        NavigationService navigation)
    {
        _authService = authService;
        _navigation = navigation;
    }

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        var result = await _authService.RegisterAsync(
            new RegisterRequestDto(
                Username,
                Password));

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ??
                           "Registration failed.";

            return;
        }

        _navigation.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void Back()
    {
        ErrorMessage = string.Empty;

        _navigation.NavigateTo<LoginViewModel>();
    }
}