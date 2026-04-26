namespace KonturTest.Models;

public sealed class BitFieldRecord
{
    public bool IsEnabled1 { get; init; }  // bit 0
    public uint Value11    { get; init; }  // bits 1–3   (3 bits)
    public uint Value12    { get; init; }  // bits 4–6   (3 bits)
    public uint Value13    { get; init; }  // bits 7–15  (9 bits)
    public bool IsEnabled2 { get; init; }  // bit 16
    public uint Value21    { get; init; }  // bits 17–27 (11 bits)
    public uint Value22    { get; init; }  // bits 28–31 (4 bits)
}
