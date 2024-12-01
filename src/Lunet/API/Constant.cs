using Lunet.VM;

namespace Lunet.API;

public static class Constant
{
    internal const byte LUA_OK = 0;
    internal const byte LUA_YIELD = 1;
    internal const byte LUA_ERRRUN = 2;
    internal const byte LUA_ERRSYNTAX = 3;
    internal const byte LUA_ERRMEM = 4;
    internal const byte LUA_ERRGCMM = 5;
    internal const byte LUA_ERRERR = 6;
    internal const byte LUA_ERRFILE = 7;

    public const string NULL_ALIAS = "null";

    internal const int LFIELDS_PER_FLUSH = 50;

    public const int LUA_MINSTACK = 20;
    public const int LUAI_MAXSTACK = 1000000;
    public const int LUA_REGISTRYINDEX = -LUAI_MAXSTACK - 1000;
    public const long LUA_RIDX_GLOBALS = 2;

    internal const int LUA_TNONE = -1;
    public const int LUA_TNIL = 0;
    public const int LUA_TBOOLEAN = 1;
    public const int LUA_TLIGHTUSERDATA = 2;
    public const int LUA_TNUMBER = 3;
    public const int LUA_TSTRING = 4;
    public const int LUA_TTABLE = 5;
    public const int LUA_TFUNCTION = 6;
    public const int LUA_TUSERDATA = 7;
    public const int LUA_TTHREAD = 8;

    public const int LUA_OPADD = 0;
    public const int LUA_OPSUB = 1;
    public const int LUA_OPMUL = 2;
    public const int LUA_OPMOD = 3;
    public const int LUA_OPPOW = 4;
    public const int LUA_OPDIV = 5;
    public const int LUA_OPIDIV = 6;
    public const int LUA_OPBAND = 7;
    public const int LUA_OPBOR = 8;
    public const int LUA_OPBXOR = 9;
    public const int LUA_OPSHL = 10;
    public const int LUA_OPSHR = 11;
    public const int LUA_OPUNM = 12;
    public const int LUA_OPBNOT = 13;


    public const int LUA_OPEQ = 0;
    public const int LUA_OPLT = 1;
    public const int LUA_OPLE = 2;

    private const byte OpArgN = 0;
    private const byte OpArgU = 1;
    private const byte OpArgR = 2;
    private const byte OpArgK = 3;

    private const byte IABC = 0;
    private const byte IABx = 1;
    private const byte IAsBx = 2;
    private const byte IAx = 3;

    internal const byte OP_MOVE = 0;
    internal const byte OP_LOADK = 1;
    internal const byte OP_LOADKX = 2;
    internal const byte OP_LOADBOOL = 3;
    internal const byte OP_LOADNIL = 4;
    internal const byte OP_GETUPVAL = 5;
    internal const byte OP_GETTABUP = 6;
    internal const byte OP_GETTABLE = 7;
    internal const byte OP_SETTABUP = 8;
    internal const byte OP_SETUPVAL = 9;
    internal const byte OP_SETTABLE = 10;
    internal const byte OP_NEWTABLE = 11;
    internal const byte OP_SELF = 12;
    internal const byte OP_ADD = 13;
    internal const byte OP_SUB = 14;
    internal const byte OP_MUL = 15;
    internal const byte OP_MOD = 16;
    internal const byte OP_POW = 17;
    internal const byte OP_DIV = 18;
    internal const byte OP_IDIV = 19;
    internal const byte OP_BAND = 20;
    internal const byte OP_BOR = 21;
    internal const byte OP_BXOR = 22;
    internal const byte OP_SHL = 23;
    internal const byte OP_SHR = 24;
    internal const byte OP_UNM = 25;
    internal const byte OP_BNOT = 26;
    internal const byte OP_NOT = 27;
    internal const byte OP_LEN = 28;
    internal const byte OP_CONCAT = 29;
    internal const byte OP_JMP = 30;
    internal const byte OP_EQ = 31;
    internal const byte OP_LT = 32;
    internal const byte OP_LE = 33;
    internal const byte OP_TEST = 34;
    internal const byte OP_TESTSET = 35;
    internal const byte OP_CALL = 36;
    internal const byte OP_TAILCALL = 37;
    internal const byte OP_RETURN = 38;
    internal const byte OP_FORLOOP = 39;
    internal const byte OP_FORPREP = 40;
    internal const byte OP_TFORCALL = 41;
    internal const byte OP_TFORLOOP = 42;
    internal const byte OP_SETLIST = 43;
    internal const byte OP_CLOSURE = 44;
    internal const byte OP_VARARG = 45;
    internal const byte OP_EXTRAARG = 46;

    internal static readonly OpCode[] OpCodes =
    [
        new(0, 1, OpArgR, OpArgN, IABC, "MOVE    "), // R(A) := R(B)
        new(0, 1, OpArgK, OpArgN, IABx, "LOADK   "), // R(A) := Kst(Bx)
        new(0, 1, OpArgN, OpArgN, IABx, "LOADKX  "), // R(A) := Kst(extra arg)
        new(0, 1, OpArgU, OpArgU, IABC, "LOADBOOL"), // R(A) := (bool)B; if (C) pc++
        new(0, 1, OpArgU, OpArgN, IABC, "LOADNIL "), // R(A), R(A+1), ..., R(A+B) := nil
        new(0, 1, OpArgU, OpArgN, IABC, "GETUPVAL"), // R(A) := UpValue[B]
        new(0, 1, OpArgU, OpArgK, IABC, "GETTABUP"), // R(A) := UpValue[B][RK(C)]
        new(0, 1, OpArgR, OpArgK, IABC, "GETTABLE"), // R(A) := R(B)[RK(C)]
        new(0, 0, OpArgK, OpArgK, IABC, "SETTABUP"), // UpValue[A][RK(B)] := RK(C)
        new(0, 0, OpArgU, OpArgN, IABC, "SETUPVAL"), // UpValue[B] := R(A)
        new(0, 0, OpArgK, OpArgK, IABC, "SETTABLE"), // R(A)[RK(B)] := RK(C)
        new(0, 1, OpArgU, OpArgU, IABC, "NEWTABLE"), // R(A) := {} (size = B,C)
        new(0, 1, OpArgR, OpArgK, IABC, "SELF    "), // R(A+1) := R(B); R(A) := R(B)[RK(C)]
        new(0, 1, OpArgK, OpArgK, IABC, "ADD     "), // R(A) := RK(B) + RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "SUB     "), // R(A) := RK(B) - RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "MUL     "), // R(A) := RK(B) * RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "MOD     "), // R(A) := RK(B) % RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "POW     "), // R(A) := RK(B) ^ RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "DIV     "), // R(A) := RK(B) / RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "IDIV    "), // R(A) := RK(B) // RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "BAND    "), // R(A) := RK(B) & RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "BOR     "), // R(A) := RK(B) | RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "BXOR    "), // R(A) := RK(B) ~ RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "SHL     "), // R(A) := RK(B) << RK(C)
        new(0, 1, OpArgK, OpArgK, IABC, "SHR     "), // R(A) := RK(B) >> RK(C)
        new(0, 1, OpArgR, OpArgN, IABC, "UNM     "), // R(A) := -R(B)
        new(0, 1, OpArgR, OpArgN, IABC, "BNOT    "), // R(A) := ~R(B)
        new(0, 1, OpArgR, OpArgN, IABC, "NOT     "), // R(A) := not R(B)
        new(0, 1, OpArgR, OpArgN, IABC, "LEN     "), // R(A) := length of R(B)
        new(0, 1, OpArgR, OpArgR, IABC, "CONCAT  "), // R(A) := R(B).. ... ..R(C)
        new(0, 0, OpArgR, OpArgN, IAsBx, "JMP     "),
        new(1, 0, OpArgK, OpArgK, IABC, "EQ      "),
        new(1, 0, OpArgK, OpArgK, IABC, "LT      "),
        new(1, 0, OpArgK, OpArgK, IABC, "LE      "),
        new(1, 0, OpArgN, OpArgU, IABC, "TEST    "),
        new(1, 1, OpArgR, OpArgU, IABC, "TESTSET "),
        new(0, 1, OpArgU, OpArgU, IABC, "CALL    "),
        new(0, 1, OpArgU, OpArgU, IABC, "TAILCALL"), // return R(A)(R(A+1), ... ,R(A+B-1))
        new(0, 0, OpArgU, OpArgN, IABC, "RETURN  "), // return R(A), ... ,R(A+B-2)
        new(0, 1, OpArgR, OpArgN, IAsBx, "FORLOOP "),
        new(0, 1, OpArgR, OpArgN, IAsBx, "FORPREP "), // R(A)-=R(A+2); pc+=sBx
        new(0, 0, OpArgN, OpArgU, IABC, "TFORCALL"),
        new(0, 1, OpArgR, OpArgN, IAsBx, "TFORLOOP"),
        new(0, 0, OpArgU, OpArgU, IABC, "SETLIST "),
        new(0, 1, OpArgU, OpArgN, IABx, "CLOSURE "),
        new(0, 1, OpArgU, OpArgN, IABC, "VARARG  "),
        new(0, 0, OpArgU, OpArgU, IAx, "EXTRAARG")
    ];
}
