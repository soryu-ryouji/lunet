namespace Lunet.VM;

public struct OpCode(byte t, byte a, byte argBMode, byte argCMode, byte opMode, string name)
{
    private byte mTestFlag = t;
    private byte mSetAFlag = a;
    internal readonly byte ArgBMode = argBMode;
    internal readonly byte ArgCMode = argCMode;
    internal readonly byte OpMode = opMode;
    internal readonly string Name = name;
}