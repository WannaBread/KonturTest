using System.IO;

namespace KonturTest.Models;

public sealed record PacketRecord(
    uint   Size,
    uint   Type,
    uint   PacketNo,
    uint   TimeMs,
    double Channel1,
    double Channel2,
    double Channel3,
    double Channel4,
    double Channel5,
    double Channel6)
{
    private const int BlockCount   = 60;
    private const int ChannelCount = 6;
    public  const int PacketSize   = 16 + BlockCount * ChannelCount * sizeof(int);

    public static PacketRecord Read(BinaryReader reader)
    {
        uint size     = reader.ReadUInt32();
        uint type     = reader.ReadUInt32();
        uint packetNo = reader.ReadUInt32();
        uint timeMs   = reader.ReadUInt32();

        long[] sums = new long[ChannelCount];
        for (int b = 0; b < BlockCount; b++)
            for (int ch = 0; ch < ChannelCount; ch++)
                sums[ch] += reader.ReadInt32();

        return new PacketRecord(
            size, type, packetNo, timeMs,
            sums[0] / (double)BlockCount,
            sums[1] / (double)BlockCount,
            sums[2] / (double)BlockCount,
            sums[3] / (double)BlockCount,
            sums[4] / (double)BlockCount,
            sums[5] / (double)BlockCount);
    }
}
