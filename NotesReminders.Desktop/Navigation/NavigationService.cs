using Microsoft.Extensions.DependencyInjection;
using NotesReminders.Desktop.ViewModels;

namespace NotesReminders.Desktop.Navigation;

public class NavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MainWindowViewModel _mainWindow;

    public NavigationService(
        IServiceProvider serviceProvider,
        MainWindowViewModel mainWindow)
    {
        _serviceProvider = serviceProvider;
        _mainWindow = mainWindow;
    }

    public async Task NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        _mainWindow.CurrentViewModel = viewModel;

        if (viewModel is INavigationAware navigationAware)
        {
            await navigationAware.OnNavigatedToAsync();
        }
    }
}