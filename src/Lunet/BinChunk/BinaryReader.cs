using Lunet.Utils;

namespace Lunet.BinChunk;

public struct BinaryReader(byte[] data)
{
    private byte[] mData = data;
    public readonly byte[] Data => mData;

    public byte ReadByte()
    {
        var value = mData[0];
        mData = mData[1..];
        return value;
    }

    private byte[] ReadBytes(uint n)
    {
        var bytes = new byte[n];
        Array.ConstrainedCopy(mData, 0, bytes, 0, (int)n);
        mData = mData.Skip((int)n).ToArray();
        return bytes;
    }

    public int ReadInt32()
    {
        var value = BitConverter.ToInt32(mData, 0);
        mData = mData[4..];
        return value;
    }

    public uint ReadUint32()
    {
        var value = BitConverter.ToUInt32(mData, 0);
        mData = mData[4..];
        return value;
    }

    public long ReadInt64()
    {
        var value = BitConverter.ToInt64(mData, 0);
        mData = mData[8..];
        return value;
    }

    public ulong ReadUint64()
    {
        var value = BitConverter.ToUInt64(mData, 0);
        mData = mData[8..];
        return value;
    }

    private long ReadLuaInteger()
    {
        return (long)ReadUint64();
    }

    private double ReadLuaNumber()
    {
        return BitConverter.Int64BitsToDouble((long)ReadUint64());
    }

    public string ReadString()
    {
        var size = (uint)ReadByte();

        if (size == 0)
        {
            return "";
        }

        if (size == 0xFF)
        {
            size = (uint)ReadUint64();
        }

        var bytes = ReadBytes(size);
        return ConvertUtil.Bytes2String(bytes);
    }

    private uint[] ReadCode()
    {
        var code = new uint[ReadUint32()];
        for (var i = 0; i < code.Length; i++)
        {
            code[i] = ReadUint32();
        }

        return code;
    }

    public void CheckHeader()
    {
        if (ConvertUtil.Bytes2String(ReadBytes(4)) != BinaryChunk.LUA_SIGNATURE)
        {
            throw new Exception("not a precompiled chunk!");
        }

        if (ReadByte() != BinaryChunk.LUAC_VERSION)
        {
            throw new Exception("version mismatch!");
        }

        if (ReadByte() != BinaryChunk.LUAC_FORMAT)
        {
            throw new Exception("format mismatch!");
        }

        var luacData = ReadBytes(6);
        if (luacData[0] != 0x19 || luacData[1] != 0x93 || luacData[2] != 0x0D ||
            luacData[3] != 0x0A || luacData[4] != 0x1A || luacData[5] != 0x0A)
        {
            throw new Exception("corrupted!");
        }

        var bit = ReadByte();
        var instruction = Convert.ToUInt32(bit);
        if (instruction != BinaryChunk.INSTRUCTION_SIZE)
        {
            throw new Exception("instruction size mismatch!");
        }

        var integerBit = ReadByte();
        var integer = Convert.ToUInt32(integerBit);
        if (integer != BinaryChunk.LUA_INTEGER_SIZE)
        {
            throw new Exception("lua_Integer size mismatch!");
        }

        var numBit = ReadByte();
        var num = Convert.ToUInt32(numBit);
        if (num != BinaryChunk.LUA_NUMBER_SIZE)
        {
            throw new Exception("lua_Number size mismatch!");
        }

        if (ReadLuaInteger() != BinaryChunk.LUAC_INT)
        {
            throw new Exception("endianness mismatch!");
        }

        if (!ReadLuaNumber().Equals(BinaryChunk.LUAC_NUM))
        {
            throw new Exception("float format mismatch!");
        }
    }

    public Prototype ReadProto(string parentSource)
    {
        var source = ReadString();
        if (source == "")
        {
            source = parentSource;
        }

        return new Prototype
        {
            Source = source,
            LineDefined = ReadUint32(),
            LastLineDefined = ReadUint32(),
            NumParams = ReadByte(),
            IsVararg = ReadByte(),
            MaxStackSize = ReadByte(),
            Code = ReadCode(),
            Constants = ReadConstants(),
            Upvalues = ReadUpvalues(),
            Protos = ReadProtos(source),
            LineInfo = ReadLineInfo(),
            LocVars = ReadLocVars(),
            UpvalueNames = ReadUpvalueNames()
        };
    }

    private string[] ReadUpvalueNames()
    {
        var names = new string[ReadUint32()];
        for (var i = 0; i < names.Length; i++)
        {
            names[i] = ReadString();
        }

        return names;
    }

    private LocVar[] ReadLocVars()
    {
        var locVars = new LocVar[ReadUint32()];
        for (var i = 0; i < locVars.Length; i++)
        {
            locVars[i] = new LocVar
            {
                VarName = ReadString(),
                StartPc = ReadUint32(),
                EndPc = ReadUint32()
            };
        }

        return locVars;
    }

    private uint[] ReadLineInfo()
    {
        var lineInfo = new uint[ReadUint32()];
        for (var i = 0; i < lineInfo.Length; i++)
        {
            lineInfo[i] = ReadUint32();
        }

        return lineInfo;
    }

    private Prototype[] ReadProtos(string parentSource)
    {
        var protos = new Prototype[ReadUint32()];
        for (var i = 0; i < protos.Length; i++)
        {
            protos[i] = new Prototype();
            protos[i] = ReadProto(parentSource);
        }

        return protos;
    }

    private Upvalue[] ReadUpvalues()
    {
        var upvalues = new Upvalue[ReadUint32()];
        for (var i = 0; i < upvalues.Length; i++)
        {
            upvalues[i] = new Upvalue
            {
                Instack = ReadByte(),
                Idx = ReadByte()
            };
        }

        return upvalues;
    }

    private object?[] ReadConstants()
    {
        var constants = new object?[ReadUint32()];
        for (var i = 0; i < constants.Length; i++)
        {
            constants[i] = ReadConstant();
        }

        return constants;
    }

    private object? ReadConstant()
    {
        return ReadByte() switch
        {
            BinaryChunk.TAG_NIL => null,
            BinaryChunk.TAG_BOOLEAN => ReadByte() != 0,
            BinaryChunk.TAG_INTEGER => ReadInt32(),
            BinaryChunk.TAG_NUMBER => ReadLuaNumber(),
            BinaryChunk.TAG_SHORT_STR => ReadString(),
            BinaryChunk.TAG_LONG_STR => ReadString(),
            _ => throw new Exception("corrupted!"),
        };
    }
}
