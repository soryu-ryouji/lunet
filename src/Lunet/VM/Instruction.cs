using Lunet.API;

namespace Lunet.VM;

public class Instruction(uint b)
{
    private readonly uint mSelf = b;

    public int Opcode()
    {
        return (int)(mSelf & 0x3F);
    }

    public (int, int, int) ABC()
    {
        return (
            (int)(mSelf >> 6) & 0xFF,
            (int)(mSelf >> 23) & 0x1FF,
            (int)(mSelf >> 14) & 0x1FF
        );
    }

    public (int, int) ABx()
    {
        return ((int)((mSelf >> 6) & 0xFF),
            (int)(mSelf >> 14));
    }

    private const int MAXARG_Bx = (1 << 18) - 1;
    private const int MAXARG_sBx = MAXARG_Bx >> 1;

    public (int, int) AsBx()
    {
        var (a, sBx) = ABx();
        return (a, sBx - MAXARG_sBx);
    }

    public int Ax()
    {
        return (int)(mSelf >> 6);
    }

    private string OpName()
    {
        return Constant.OpCodes[Opcode()].Name;
    }

    public byte OpMode()
    {
        return Constant.OpCodes[Opcode()].OpMode;
    }

    public byte BMode()
    {
        return Constant.OpCodes[Opcode()].ArgBMode;
    }

    public byte CMode()
    {
        return Constant.OpCodes[Opcode()].ArgCMode;
    }
}