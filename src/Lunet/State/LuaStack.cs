using System.Collections;
using Lunet.API;

namespace Lunet.State;

public class LuaStack
{
    internal readonly ArrayList Slots = new();
    internal int Top => Slots.Count;

    internal int AbsIndex(int idx)
    {
        return idx >= 0 || idx <= Constant.LUA_REGISTRYINDEX
            ? idx
            : idx + Slots.Count + 1;
    }

    internal void Push(object? val)
    {
        if (Slots.Count > Slots.Capacity)
        {
            throw new StackOverflowException();
        }

        Slots.Add(val);
    }

    internal void PushN(ArrayList vals)
    {
        foreach (var val in vals)
        {
            Push(val);
        }
    }

    internal object? Pop()
    {
        var value = Slots[^1];
        Slots.RemoveAt(Slots.Count - 1);
        return value;
    }

    internal ArrayList PopN(int n)
    {
        var vals = new ArrayList(n);
        for (var i = 0; i < n; i++)
        {
            vals.Add(Pop());
        }

        vals.Reverse();
        return vals;
    }

    internal object Get(int idx)
    {
        throw new System.NotImplementedException();
    }

    internal void Set(int idx, object? val)
    {
        if (idx == 0)
        {
            throw new IndexOutOfRangeException();
        }

        if (idx > 0)
        {
            Slots[idx - 1] = val;
        }
        else
        {
            Slots[Slots.Count + idx] = val;
        }
    }

    internal void Reverse(int from, int to)
    {
        Slots.Reverse(from, to - from + 1);
    }
}