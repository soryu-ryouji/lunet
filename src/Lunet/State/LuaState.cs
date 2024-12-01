using Lunet.API;
using Lunet.Number;

using LuaType = System.Int32;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

namespace Lunet.State;

public partial struct LuaState : ILuaState
{
    private LuaStack mStack = new();

    public LuaState()
    {
    }

    public int LuaType { get; set; }

    public int GetTop()
    {
        return mStack.Top;
    }

    public int AbsIndex(int idx)
    {
        return mStack.Top;
    }

    public bool CheckStack(int n)
    {
        return true;
    }

    public void Pop(int n)
    {
        for (var i = 0; i < n; i++)
        {
            mStack.Pop();
        }
    }

    public void Copy(int fromIdx, int toIdx)
    {
        var val = mStack.Get(fromIdx);
        mStack.Set(toIdx, val);
    }

    public void PushValue(int idx)
    {
        var val = mStack.Get(idx);
        mStack.Push(val);
    }

    public void Replace(int idx)
    {
        mStack.Set(idx, mStack.Pop());
    }

    public void Insert(int idx)
    {
        Rotate(idx, 1);
    }

    public void Remove(int idx)
    {
        Rotate(idx, -1);
        Pop(1);
    }

    public void Rotate(int idx, int n)
    {
        var t = mStack.Top - 1; /* end of stack segment being rotated */
        var p = mStack.AbsIndex(idx) - 1; /* start of segment */
        var m = n >= 0 ? t - n : p - n - 1; /* end of prefix */

        mStack.Reverse(p, m); /* reverse the prefix with length 'n' */
        mStack.Reverse(m + 1, t); /* reverse the suffix */
        mStack.Reverse(p, t);
    }

    public void SetTop(int idx)
    {
        var newTop = mStack.AbsIndex(idx);
        if (newTop < 0)
        {
            throw new Exception("stack underflow!");
        }

        var n = mStack.Top - newTop;
        if (n > 0)
        {
            for (var i = 0; i < n; i++)
            {
                mStack.Pop();
            }
        }
        else if (n < 0)
        {
            for (var i = 0; i > n; i--)
            {
                mStack.Push(null);
            }
        }
    }

    # region Arith
    private static Operator[] Operators =
    [
        new() { integerFunc = iadd, floatFunc = fadd },
        new() { integerFunc = isub, floatFunc = fsub },
        new() { integerFunc = imul, floatFunc = fmul },
        new() { integerFunc = imod, floatFunc = fmod },
        new() { integerFunc = null, floatFunc = pow },
        new() { integerFunc = null, floatFunc = div },
        new() { integerFunc = iidiv, floatFunc = fidiv },
        new() { integerFunc = band, floatFunc = null },
        new() { integerFunc = bor, floatFunc = null },
        new() { integerFunc = bxor, floatFunc = null },
        new() { integerFunc = shl, floatFunc = null },
        new() { integerFunc = shr, floatFunc = null },
        new() { integerFunc = inum, floatFunc = fnum },
        new() { integerFunc = bnot, floatFunc = null }
    ];

    private static long iadd(long a, long b)
    {
        return a + b;
    }

    private static double fadd(double a, double b)
    {
        return a + b;
    }

    private static long isub(long a, long b)
    {
        return a - b;
    }

    private static double fsub(double a, double b)
    {
        return a - b;
    }

    private static long imul(long a, long b)
    {
        return a * b;
    }

    private static double fmul(double a, double b)
    {
        return a * b;
    }

    private static long imod(long a, long b)
    {
        return LuaMath.IMod(a, b);
    }

    private static double fmod(double a, double b)
    {
        return LuaMath.FMod(a, b);
    }

    private static double pow(double a, double b)
    {
        return System.Math.Pow(a, b);
    }

    private static double div(double a, double b)
    {
        return a / b;
    }

    private static long iidiv(long a, long b)
    {
        return LuaMath.IFloorDiv(a, b);
    }

    private static double fidiv(double a, double b)
    {
        return LuaMath.FFloorDiv(a, b);
    }

    private static long band(long a, long b)
    {
        return a & b;
    }

    private static long bor(long a, long b)
    {
        return a | b;
    }

    private static long bxor(long a, long b)
    {
        return a ^ b;
    }

    private static long shl(long a, long b)
    {
        return LuaMath.ShiftLeft(a, b);
    }

    private static long shr(long a, long b)
    {
        return LuaMath.ShiftRight(a, b);
    }

    private static long inum(long a, long _)
    {
        return -a;
    }

    private static double fnum(double a, double _)
    {
        return -a;
    }

    private static long bnot(long a, long _)
    {
        return ~a;
    }


    public void Arith(ArithOp op)
    {
        LuaValue a, b;
        b = new LuaValue(mStack.Pop());
        if (op != Constant.LUA_OPUNM && op != Constant.LUA_OPBNOT)
        {
            a = new LuaValue(mStack.Pop());
        }
        else
        {
            a = b;
        }

        var opr = Operators[op];
        var result = _arith(a, b, opr);
        if (result != null)
        {
            mStack.Push(result);
        }
        else
        {
            throw new Exception("arithmetic error!");
        }
    }

    object _arith(LuaValue a, LuaValue b, Operator op)
    {
        if (op.floatFunc == null)
        {
            Tuple<long, bool> v = LuaValue.ConvertToInteger(a.value);
            if (v.Item2)
            {
                Tuple<long, bool> v2 = LuaValue.ConvertToInteger(b.value);
                if (v2.Item2)
                {
                    return op.integerFunc(v.Item1, v2.Item1);
                }
            }
        }
        else
        {
            if (op.integerFunc != null)
            {
                if (a.value.GetType().Name.Equals("Int64") && b.value.GetType().Name.Equals("Int64"))
                {
                    var x = long.Parse(a.value.ToString());
                    var y = long.Parse(b.value.ToString());
                    return op.integerFunc(x, y);
                }
            }

            var v = LuaValue.ConvertToFloat(a.value);
            if (v.Item2)
            {
                var v2 = LuaValue.ConvertToFloat(b.value);
                if (v2.Item2)
                {
                    return op.floatFunc(v.Item1, v2.Item1);
                }
            }
        }

        return null;
    }
    # endregion

    # region Access
    public string TypeName(LuaType tp)
    {
        return tp switch
        {
            Constant.LUA_TNONE => "no value",
            Constant.LUA_TNIL => "nil",
            Constant.LUA_TBOOLEAN => "boolean",
            Constant.LUA_TNUMBER => "number",
            Constant.LUA_TSTRING => "string",
            Constant.LUA_TTABLE => "table",
            Constant.LUA_TFUNCTION => "function",
            Constant.LUA_TTHREAD => "thread",
            _ => "userdata",
        };
    }

    public LuaType Type(int idx)
    {
        if (!mStack.IsValid(idx)) return Constant.LUA_TNONE;
        var val = mStack.Get(idx);
        return LuaValue.TypeOf(val);
    }

    public bool IsNone(int idx)
    {
        return Type(idx) == Constant.LUA_TNONE;
    }

    public bool IsNil(int idx)
    {
        return Type(idx) == Constant.LUA_TNIL;
    }

    public bool IsNoneOrNil(int idx)
    {
        return Type(idx) <= Constant.LUA_TNIL;
    }

    public bool IsBoolean(int idx)
    {
        return Type(idx) == Constant.LUA_TBOOLEAN;
    }

    public bool IsString(int idx)
    {
        var t = Type(idx);
        return t == Constant.LUA_TSTRING || t == Constant.LUA_TNUMBER;
    }

    public bool IsNumber(int idx)
    {
        return ToNumberX(idx).Item2;
    }

    public bool IsInteger(int idx)
    {
        var val = mStack.Get(idx);
        return val.GetType().Name.Equals("Int64");
    }

    public bool ToBoolean(int idx)
    {
        var val = mStack.Get(idx);
        return ConvertToBoolean(val);
    }


    private static bool ConvertToBoolean(object val)
    {
        if (val == null)
        {
            return false;
        }

        return val.GetType().Name switch
        {
            "Boolean" => (bool)val,
            _ => true,
        };
    }

    public double ToNumber(int idx)
    {
        return ToNumberX(idx).Item1;
    }

    public Tuple<double, bool> ToNumberX(int idx)
    {
        var val = mStack.Get(idx);
        return LuaValue.ConvertToFloat(val);
    }

    public Tuple<long, bool> ToIntegerX(int idx)
    {
        var val = mStack.Get(idx);
        return LuaValue.ConvertToInteger(val);
    }

    public long ToInteger(int idx)
    {
        var val = ToIntegerX(idx);
        return val.Item1;
    }

    public string ToString(int idx)
    {
        return ToStringX(idx).Item1;
    }

    public Tuple<string, bool> ToStringX(int idx)
    {
        var val = mStack.Get(idx);
        switch (val.GetType().Name)
        {
            case "String": return Tuple.Create((string)val, true);
            case "Int64":
            case "Double":
                var s = val;
                mStack.Set(idx, s);
                return Tuple.Create(Convert.ToString(s), true);
            default: return Tuple.Create("", false);
        }
    }
    # endregion

    # region Comparison
    public bool Compare(int idx1, int idx2, CompareOp op)
    {
        var a = mStack.Get(idx1);
        var b = mStack.Get(idx2);
        switch (op)
        {
            case Constant.LUA_OPEQ: return OpEqual(a, b);
            case Constant.LUA_OPLT: return OpLt(a, b);
            case Constant.LUA_OPLE: return OpLe(a, b);
            default: throw new Exception("invalid compare op!");
        }
    }

    bool OpEqual(object a, object b)
    {
        if (a == null)
        {
            return b == null;
        }

        switch (a.GetType().Name)
        {
            case "Boolean":
                if (b.GetType().Name.Equals("Boolean"))
                {
                    return a == b;
                }

                return false;
            case "String":
                if (b.GetType().Name.Equals("String"))
                {
                    return a.Equals(b);
                }

                return false;
            case "Int64":
                return b.GetType().Name switch
                {
                    "Int64" => (long)a == (long)b,
                    "Double" => ((double)b).Equals((double)a),
                    _ => false,
                };
            case "Double":
                return b.GetType().Name switch
                {
                    "Double" => a.Equals(b),
                    "Int64" => a.Equals((double)b),
                    _ => false,
                };
            default: return a == b;
        }
    }

    bool OpLt(object a, object b)
    {
        switch (a.GetType().Name)
        {
            case "String":
                if (b.GetType().Name.Equals("String"))
                {
                    return String.Compare(((string)a), (string)b, StringComparison.Ordinal) == -1;
                }

                break;
            case "Int64":
                switch (b.GetType().Name)
                {
                    case "Int64": return (long)a < (long)b;
                    case "Double": return (double)a < (double)b;
                }

                break;
            case "Double":
                switch (b.GetType().Name)
                {
                    case "Double": return (double)a < (double)b;
                    case "Int64": return (double)a < (double)b;
                }

                break;
        }

        throw new Exception("comparison error!");
    }

    bool OpLe(object a, object b)
    {
        switch (a.GetType().Name)
        {
            case "String":
                if (b.GetType().Name.Equals("String"))
                {
                    return string.CompareOrdinal((string)a, (string)b) <= 0;
                }

                break;
            case "Int64":
                switch (b.GetType().Name)
                {
                    case "Int64": return (long)a <= (long)b;
                    case "Double": return Convert.ToDouble(a) <= Convert.ToDouble(b);
                }

                break;
            case "Double":
                switch (b.GetType().Name)
                {
                    case "Double": return (double)a <= (double)b;
                    case "Int64": return Convert.ToDouble(a) <= Convert.ToDouble(b);
                }

                break;
        }

        throw new Exception("comparison error!");
    }
    # endregion

    # region Misc
    public void Len(int idx)
    {
        var val = mStack.Get(idx);
        if (val.GetType().Name.Equals("String"))
        {
            var s = (string)val;
            mStack.Push((long)s.Length);
        }
        else
        {
            throw new Exception("length error!");
        }
    }

    public void Concat(int n)
    {
        if (n == 0)
        {
            mStack.Push("");
        }
        else if (n >= 2)
        {
            for (var i = 1; i < n; i++)
            {
                if (IsString(-1) && IsString(-2))
                {
                    var s2 = ToString(-1);
                    var s1 = ToString(-2);
                    mStack.Pop();
                    mStack.Pop();
                    mStack.Push(s1 + s2);
                    continue;
                }

                throw new Exception("concatenation error!");
            }
            // n==1, do nothing
        }
    }
    # endregion
}