namespace Lunet.Number;

public class NumberParser
{
    internal static Tuple<long, bool> ParseInteger(string str)
    {
        try
        {
            var i = Convert.ToInt64(str);
            return Tuple.Create(i, true);
        }
        catch (Exception)
        {
            return Tuple.Create(0L, false);
        }
    }

    internal static Tuple<double, bool> ParseFloat(string str)
    {
        try
        {
            var i = Convert.ToDouble(str);
            return Tuple.Create(i, true);
        }
        catch (Exception)
        {
            return Tuple.Create(0D, false);
        }
    }
}