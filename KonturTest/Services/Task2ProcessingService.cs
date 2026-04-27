using System.IO;
using KonturTest.Models;

namespace KonturTest.Services;

public sealed class Task2ProcessingService
{
    public void Process(string inputPath, string outputPath,
        IProgress<double> progress, CancellationToken ct)
    {
        using var fs     = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new BinaryReader(fs);
        using var writer = new StreamWriter(outputPath, append: false, System.Text.Encoding.UTF8);

        writer.WriteLine("IsEnabled1,Value11,Value12,Value13,IsEnabled2,Value21,Value22");

        long totalRecords = fs.Length / 4;
        long index        = 0;

        while (fs.Position + 4 <= fs.Length)
        {
            ct.ThrowIfCancellationRequested();

            var r = BitFieldRecord.Read(reader);

            writer.Write(r.IsEnabled1 ? "1" : "0"); writer.Write(',');
            writer.Write(r.Value11);                 writer.Write(',');
            writer.Write(r.Value12);                 writer.Write(',');
            writer.Write(r.Value13);                 writer.Write(',');
            writer.Write(r.IsEnabled2 ? "1" : "0"); writer.Write(',');
            writer.Write(r.Value21);                 writer.Write(',');
            writer.WriteLine(r.Value22);

            index++;
            if (index % 1000 == 0 || index == totalRecords)
                progress.Report(index / (double)Math.Max(totalRecords, 1));
        }

        progress.Report(1.0);
    }
}
