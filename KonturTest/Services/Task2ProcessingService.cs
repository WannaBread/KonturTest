using System.IO;

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

            uint raw = reader.ReadUInt32();

            bool isEnabled1 = (raw & 0x1u) != 0;
            uint value11    = (raw >> 1)  & 0x7u;     // bits 1–3   (3 bits)
            uint value12    = (raw >> 4)  & 0x7u;     // bits 4–6   (3 bits)
            uint value13    = (raw >> 7)  & 0x1FFu;   // bits 7–15  (9 bits)
            bool isEnabled2 = ((raw >> 16) & 0x1u) != 0;
            uint value21    = (raw >> 17) & 0x7FFu;   // bits 17–27 (11 bits)
            uint value22    = (raw >> 28) & 0xFu;     // bits 28–31 (4 bits)

            writer.Write(isEnabled1 ? "1" : "0"); writer.Write(',');
            writer.Write(value11);               writer.Write(',');
            writer.Write(value12);               writer.Write(',');
            writer.Write(value13);               writer.Write(',');
            writer.Write(isEnabled2 ? "1" : "0"); writer.Write(',');
            writer.Write(value21);               writer.Write(',');
            writer.WriteLine(value22);

            index++;
            if (index % 1000 == 0 || index == totalRecords)
                progress.Report(index / (double)Math.Max(totalRecords, 1));
        }

        progress.Report(1.0);
    }
}
