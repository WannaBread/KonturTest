using System.IO;

namespace KonturTest.Models;

public sealed record BitFieldRecord(
    bool IsEnabled1,   // bit 0
    uint Value11,      // bits 1–3
    uint Value12,      // bits 4–6
    uint Value13,      // bits 7–15
    bool IsEnabled2,   // bit 16
    uint Value21,      // bits 17–27
    uint Value22)      // bits 28–31
{
    public static BitFieldRecord Read(BinaryReader reader)
    {
        uint raw = reader.ReadUInt32();
        return new BitFieldRecord(
            IsEnabled1: (raw & 0x1u) != 0,
            Value11:    (raw >> 1)  & 0x7u,
            Value12:    (raw >> 4)  & 0x7u,
            Value13:    (raw >> 7)  & 0x1FFu,
            IsEnabled2: ((raw >> 16) & 0x1u) != 0,
            Value21:    (raw >> 17) & 0x7FFu,
            Value22:    (raw >> 28) & 0xFu);
    }
}
