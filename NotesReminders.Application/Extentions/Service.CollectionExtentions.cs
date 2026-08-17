using Microsoft.Extensions.DependencyInjection;

using NotesReminders.Application.Interfaces;
using NotesReminders.Application.Services;

namespace NotesReminders.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IReminderProcessor, ReminderProcessor>();


        return services;
    }
}