using KonturTest.Infrastructure;
using KonturTest.Services;

namespace KonturTest.ViewModels;

public sealed class Task1ViewModel(Task1ProcessingService service) : ProcessingViewModelBase
{
    protected override void Process(string inputPath, string outputPath,
        IProgress<double> progress, CancellationToken ct)
        => service.Process(inputPath, outputPath, progress, ct);
}
