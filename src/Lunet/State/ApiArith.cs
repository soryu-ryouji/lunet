using Lunet.API;
using Lunet.Number;
using ArithOp = System.Int32;
using CompareOp = System.Int32;
using System;

namespace Lunet.State;

// Remove these lines
// internal delegate long IntegerFunc(long a, long b);
// internal delegate double FloatFunc(double a, double b);

struct Operator
{
    public Func<long, long, long> integerFunc;
    public Func<double, double, double> floatFunc;
}

public partial struct LuaState
{
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
}