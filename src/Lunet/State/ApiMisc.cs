namespace Lunet.State;

public partial struct LuaState
{
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
}