namespace KonturTest.Models;

public sealed record AppSettings
{
    public string Task1InputPath  { get; set; } = string.Empty;
    public string Task1OutputPath { get; set; } = string.Empty;
    public string Task2InputPath  { get; set; } = string.Empty;
    public string Task2OutputPath { get; set; } = string.Empty;
}
