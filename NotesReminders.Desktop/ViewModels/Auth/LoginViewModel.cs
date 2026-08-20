using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotesReminders.Desktop.Common;
using NotesReminders.Desktop.DTOs.Auth;
using NotesReminders.Desktop.Navigation;
using NotesReminders.Desktop.Services;
using NotesReminders.Desktop.ViewModels.Notes;

namespace NotesReminders.Desktop.ViewModels.Auth;

public partial class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly NavigationService _navigation;

    public LoginViewModel(
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
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        var result = await _authService.LoginAsync(
            new LoginRequestDto(
                Username,
                Password));

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage ??
                           "Login failed.";

            return;
        }

        await _navigation.NavigateTo<NotesViewModel>();
    }

    [RelayCommand]
    private async Task OpenRegister()
    {
        ErrorMessage = string.Empty;

        await _navigation.NavigateTo<RegisterViewModel>();
    }
}