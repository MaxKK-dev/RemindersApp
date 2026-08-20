using Microsoft.Extensions.DependencyInjection;
using System;

using NotesReminders.Desktop.Services;
using NotesReminders.Desktop.ViewModels;
using NotesReminders.Desktop.ViewModels.Auth;
using NotesReminders.Desktop.ViewModels.Notes;
using NotesReminders.Desktop.Navigation;

namespace NotesReminders.Desktop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDesktop(this IServiceCollection services)
    {
        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5213/");
        });

        services.AddSingleton<TokenStorage>();

        services.AddSingleton<AuthService>();
        services.AddSingleton<NoteService>();

        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<NavigationService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<NotesViewModel>();

        return services;
    }
}