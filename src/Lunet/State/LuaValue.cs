using Lunet.API;
using Lunet.Number;

using LuaType = System.Int32;

namespace Lunet.State
{
    public class LuaValue(object value)
    {
        internal readonly object value = value;

        internal static LuaType TypeOf(object val)
        {
            if (val == null)
            {
                return Constant.LUA_TNIL;
            }

            //Console.WriteLine(val.GetType().Name);
            switch (val.GetType().Name)
            {
                case "Boolean": return Constant.LUA_TBOOLEAN;
                case "Double": return Constant.LUA_TNUMBER;
                case "Int64": return Constant.LUA_TNUMBER;
                case "String": return Constant.LUA_TSTRING;
                default: throw new Exception("todo!");
            }
        }

        internal static Tuple<double, bool> ConvertToFloat(object val)
        {
            switch (val.GetType().Name)
            {
                case "Double": return Tuple.Create((double) val, true);
                case "Int64": return Tuple.Create(Convert.ToDouble(val), true);
                case "String": return NumberParser.ParseFloat((string) val);
                default: return Tuple.Create(0d, false);
            }
        }

        internal static Tuple<long, bool> ConvertToInteger(object val)
        {
            switch (val.GetType().Name)
            {
                case "Int64": return Tuple.Create<long, bool>((long) val, true);
                case "Double": return LuaMath.FloatToInteger((double) val);
                case "String": return Tuple.Create(Convert.ToInt64(val), true);
                default: return Tuple.Create(0L, false);
            }
        }

        private Tuple<long, bool> _stringToInteger(string s)
        {
            var v = NumberParser.ParseInteger(s);
            if (v.Item2)
            {
                return Tuple.Create(v.Item1, true);
            }

            var v2 = NumberParser.ParseFloat(s);
            if (v2.Item2)
            {
                return LuaMath.FloatToInteger(v2.Item1);
            }

            return Tuple.Create<long, bool>(0L, false);
        }
    }
}