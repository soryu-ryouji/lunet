using Lunet.API;

namespace Lunet.State;

public partial class LuaState : ILuaState
{
    private LuaStack mStack = new();

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
}