using System.Windows.Input;
using KonturTest.Infrastructure;
using KonturTest.Services;
using Microsoft.Win32;

namespace KonturTest.ViewModels;

public sealed class Task2ViewModel : ViewModelBase
{
    private readonly Task2ProcessingService _service;
    private CancellationTokenSource _cts = new();

    private string _inputPath  = string.Empty;
    private string _outputPath = string.Empty;
    private double _progress;
    private string _statusMessage = string.Empty;

    public string InputPath
    {
        get => _inputPath;
        set => SetProperty(ref _inputPath, value);
    }

    public string OutputPath
    {
        get => _outputPath;
        set => SetProperty(ref _outputPath, value);
    }

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            SetProperty(ref _statusMessage, value);
            OnPropertyChanged(nameof(StatusColor));
        }
    }

    public string StatusColor => _statusMessage.StartsWith("Ошибка") || _statusMessage.StartsWith("Error")
        ? "Red"
        : _statusMessage.StartsWith("Готово") || _statusMessage.StartsWith("Done")
            ? "Green"
            : "Gray";

    public ICommand BrowseInputCommand  { get; }
    public ICommand BrowseOutputCommand { get; }
    public ICommand StartCommand        { get; }

    public Task2ViewModel(Task2ProcessingService service)
    {
        _service = service;

        BrowseInputCommand  = new RelayCommand(_ => BrowseInput());
        BrowseOutputCommand = new RelayCommand(_ => BrowseOutput());
        StartCommand        = new AsyncRelayCommand(_ => RunAsync());
    }

    private void BrowseInput()
    {
        var dlg = new OpenFileDialog
        {
            Title  = "Выберите входной файл",
            Filter = "Data files (*.dat)|*.dat|All files (*.*)|*.*"
        };
        if (dlg.ShowDialog() == true)
            InputPath = dlg.FileName;
    }

    private void BrowseOutput()
    {
        var dlg = new SaveFileDialog
        {
            Title        = "Выберите файл для сохранения",
            Filter       = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            DefaultExt   = ".csv",
            AddExtension = true
        };
        if (dlg.ShowDialog() == true)
            OutputPath = dlg.FileName;
    }

    private async Task RunAsync()
    {
        if (string.IsNullOrWhiteSpace(InputPath) || string.IsNullOrWhiteSpace(OutputPath))
        {
            StatusMessage = "Укажите оба файла.";
            return;
        }

        _cts = new CancellationTokenSource();
        Progress      = 0;
        StatusMessage = "Обработка…";

        var progressReporter = new Progress<double>(v => Progress = v);

        try
        {
            await Task.Run(() => _service.Process(InputPath, OutputPath, progressReporter, _cts.Token));
            StatusMessage = "Готово.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Отменено.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
