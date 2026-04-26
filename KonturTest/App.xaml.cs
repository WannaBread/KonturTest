using System.Windows;
using KonturTest.Services;
using KonturTest.ViewModels;

namespace KonturTest;

public partial class App : Application
{
    private readonly SettingsService _settingsService = new();
    private MainViewModel? _mainViewModel;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainViewModel = new MainViewModel(
            _settingsService,
            new Task1ProcessingService(),
            new Task2ProcessingService());

        new MainWindow { DataContext = _mainViewModel }.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mainViewModel?.SaveSettings(_settingsService);
        base.OnExit(e);
    }
}
