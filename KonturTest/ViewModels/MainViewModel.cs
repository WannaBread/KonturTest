using KonturTest.Models;
using KonturTest.Services;

namespace KonturTest.ViewModels;

public sealed class MainViewModel
{
    public Task1ViewModel Task1 { get; }
    public Task2ViewModel Task2 { get; }

    public MainViewModel(SettingsService settingsService,
                         Task1ProcessingService svc1,
                         Task2ProcessingService svc2)
    {
        Task1 = new Task1ViewModel(svc1);
        Task2 = new Task2ViewModel(svc2);
        LoadSettings(settingsService);
    }

    private void LoadSettings(SettingsService s)
    {
        AppSettings cfg = s.Load();
        Task1.InputPath  = cfg.Task1InputPath;
        Task1.OutputPath = cfg.Task1OutputPath;
        Task2.InputPath  = cfg.Task2InputPath;
        Task2.OutputPath = cfg.Task2OutputPath;
    }

    public void SaveSettings(SettingsService s)
    {
        s.Save(new AppSettings
        {
            Task1InputPath  = Task1.InputPath,
            Task1OutputPath = Task1.OutputPath,
            Task2InputPath  = Task2.InputPath,
            Task2OutputPath = Task2.OutputPath
        });
    }
}
