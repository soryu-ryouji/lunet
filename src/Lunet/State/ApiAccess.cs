using System;
using Lunet.API;
using LuaType = System.Int32;

namespace Lunet.State
{
    public partial struct LuaState
    {
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

            switch (val.GetType().Name)
            {
                case "Boolean": return (bool) val;
                default: return true;
            }
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
                case "String": return Tuple.Create((string) val, true);
                case "Int64":
                case "Double":
                    var s = val;
                    mStack.Set(idx, s);
                    return Tuple.Create(Convert.ToString(s), true);
                default: return Tuple.Create("", false);
            }
        }
    }
}