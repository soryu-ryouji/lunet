using System.Text;

namespace Lunet.Utils;

public static class ConvertUtil
{
    public static string Bytes2String(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
}