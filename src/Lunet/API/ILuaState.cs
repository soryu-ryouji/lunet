namespace Lunet.API;

using LuaType = System.Int32;
using ArithOp = System.Int32;
using CompareOp = System.Int32;

public interface ILuaState
{
    public int GetTop();
    public int AbsIndex(int idx);
    public bool CheckStack(int n);
    public void Pop(int n);
    public void Copy(int fromIdx, int toIdx);
    public void PushValue(int idx);
    public void Replace(int idx);
    public void Insert(int idx);
    public void Remove(int idx);
    public void Rotate(int idx, int n);
    public void SetTop(int idx);

    string TypeName(LuaType tp);
    LuaType Type(int idx);
    bool IsNone(int idx);
    bool IsNil(int idx);
    bool IsNoneOrNil(int idx);
    bool IsBoolean(int idx);
    bool IsInteger(int idx);
    bool IsNumber(int idx);
    bool IsString(int idx);
    bool ToBoolean(int idx);
    long ToInteger(int idx);
    Tuple<long, bool> ToIntegerX(int idx);
    double ToNumber(int idx);
    Tuple<double, bool> ToNumberX(int idx);
    string ToString(int idx);
    Tuple<string, bool> ToStringX(int idx);

    void Arith(ArithOp op);
    bool Compare(int idx1, int idx2, CompareOp op);
    void Len(int idx);
    void Concat(int n);
}