using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

using NotesReminders.Desktop.Navigation;
using NotesReminders.Desktop.Extensions;
using NotesReminders.Desktop.ViewModels;
using NotesReminders.Desktop.Views;
using NotesReminders.Desktop.ViewModels.Auth;

namespace NotesReminders.Desktop;

public partial class App : Avalonia.Application
{
    public static ServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddDesktop();

        Services = services.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        var navigation = Services.GetRequiredService<NavigationService>();
            
        navigation.NavigateTo<LoginViewModel>();

        base.OnFrameworkInitializationCompleted();
    }
}