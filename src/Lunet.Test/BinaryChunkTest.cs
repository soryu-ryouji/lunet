using Xunit.Abstractions;
using BinaryReader = Lunet.BinChunk.BinaryReader;

namespace Lunet.Test;

public class BinaryChunkTest
{
    [Fact]
    public void ReadByte_ShouldReturnCorrectByte()
    {
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var reader = new BinaryReader(data);
        Assert.Equal(0x01, reader.ReadByte());
    }

    [Fact]
    public void ReadInt32_ShouldReturnCorrectInt()
    {
        var data = BitConverter.GetBytes(123456789);
        var reader = new BinaryReader(data);
        Assert.Equal(123456789, reader.ReadInt32());
    }

    [Fact]
    public void ReadUint32_ShouldReturnCorrectUint()
    {
        var data = BitConverter.GetBytes(123456789U);
        var reader = new BinaryReader(data);
        Assert.Equal(123456789U, reader.ReadUint32());
    }

    [Fact]
    public void ReadInt64_ShouldReturnCorrectLong()
    {
        var data = BitConverter.GetBytes(1234567890123456789L);
        var reader = new BinaryReader(data);
        Assert.Equal(1234567890123456789L, reader.ReadInt64());
    }

    [Fact]
    public void ReadUint64_ShouldReturnCorrectUlong()
    {
        var data = BitConverter.GetBytes(1234567890123456789UL);
        var reader = new BinaryReader(data);
        Assert.Equal(1234567890123456789UL, reader.ReadUint64());
    }

    [Fact]
    public void ReadString_ShouldReturnCorrectString()
    {
        var data = new byte[] { 0x05, (byte)'H', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
        var reader = new BinaryReader(data);
        Assert.Equal("Hello", reader.ReadString());
    }

    [Fact]
    public void CheckHeader_ShouldThrowExceptionForInvalidHeader()
    {
        var data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var reader = new BinaryReader(data);
        Assert.Throws<Exception>(() => reader.CheckHeader());
    }

    [Fact]
    public void CheckHeader_Test()
    {
        // lua binary chunk directory
        var path = "../../../lua/luac.out";

        if (!File.Exists(path))
        {
            throw new Exception("Lua Binary Chunk not found: " + path);
        }

        var data = File.ReadAllBytes("../../../lua/luac.out");
        var reader = new BinaryReader(data);
        reader.CheckHeader();
        Assert.True(true);
    }
}
