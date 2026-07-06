using PlServer.Protocol;

namespace PlServer.Protocol.Tests
{

    public sealed class PacketReaderWriterTests
    {
        [Fact]
        public void PacketReader_reads_byte_arrays_and_remaining_count()
        {
            var reader = new PacketReader(new byte[]
            {
                0x01,
                0x02,
                0x03
            });

            Assert.Equal(0x01, reader.ReadByte());
            Assert.Equal(
                new byte[]
                {
                    0x02,
                    0x03
                },
                reader.ReadBytes(2));
            Assert.Equal(0, reader.Remaining);
        }

        [Fact]
        public void PacketWriter_writes_bytes()
        {
            var writer = new PacketWriter();

            writer.WriteByte(0x01);
            writer.WriteBytes(new byte[]
            {
                0x02,
                0x03
            });

            Assert.Equal(
                new byte[]
                {
                    0x01,
                    0x02,
                    0x03
                },
                writer.ToArray());
        }
    }

}
