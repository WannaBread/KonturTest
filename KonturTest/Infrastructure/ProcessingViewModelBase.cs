using System.Windows.Input;
using Microsoft.Win32;

namespace KonturTest.Infrastructure;

public abstract class ProcessingViewModelBase : ViewModelBase
{
    private string _inputPath     = string.Empty;
    private string _outputPath    = string.Empty;
    private double _progress;
    private string _statusMessage = string.Empty;
    private CancellationTokenSource _cts = new();

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

    public string StatusColor => _statusMessage switch
    {
        var s when s.StartsWith("Ошибка")  => "Red",
        var s when s.StartsWith("Готово")  => "Green",
        _ => "Gray"
    };

    public ICommand BrowseInputCommand  { get; }
    public ICommand BrowseOutputCommand { get; }
    public ICommand StartCommand        { get; }
    public ICommand CancelCommand       { get; }

    protected ProcessingViewModelBase()
    {
        BrowseInputCommand  = new RelayCommand(_ => BrowseInput());
        BrowseOutputCommand = new RelayCommand(_ => BrowseOutput());
        StartCommand        = new AsyncRelayCommand(_ => RunAsync());
        CancelCommand       = new RelayCommand(_ => _cts.Cancel());
    }

    protected abstract void Process(string inputPath, string outputPath,
        IProgress<double> progress, CancellationToken ct);

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

        _cts.Dispose();
        _cts = new CancellationTokenSource();
        Progress      = 0;
        StatusMessage = "Обработка…";

        var progressReporter = new Progress<double>(v => Progress = v);

        try
        {
            await Task.Run(() => Process(InputPath, OutputPath, progressReporter, _cts.Token));
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
