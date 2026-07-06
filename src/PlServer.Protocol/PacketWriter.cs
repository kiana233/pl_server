namespace PlServer.Protocol;

public sealed class PacketWriter
{
    private readonly List<byte> _bytes = new();

    public void WriteByte(byte value)
    {
        _bytes.Add(value);
    }

    public void WriteUInt16LittleEndian(ushort value)
    {
        _bytes.Add((byte)value);
        _bytes.Add((byte)(value >> 8));
    }

    public void WriteUInt32LittleEndian(uint value)
    {
        _bytes.Add((byte)value);
        _bytes.Add((byte)(value >> 8));
        _bytes.Add((byte)(value >> 16));
        _bytes.Add((byte)(value >> 24));
    }

    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        _bytes.AddRange(bytes.ToArray());
    }

    public byte[] ToArray()
    {
        return _bytes.ToArray();
    }
}
