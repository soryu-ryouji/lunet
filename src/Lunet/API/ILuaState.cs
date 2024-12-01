namespace Lunet.API;

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
}