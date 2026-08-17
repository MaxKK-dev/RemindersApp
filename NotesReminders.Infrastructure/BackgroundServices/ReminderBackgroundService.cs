using NotesReminders.Application.Interfaces;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NotesReminders.Infrastructure.BackgroundServices;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReminderBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var processor =
                scope.ServiceProvider.GetRequiredService<IReminderProcessor>();

            await processor.ProcessAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}