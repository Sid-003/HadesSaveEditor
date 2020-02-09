using System;
using System.Collections.Generic;
using System.Text;

namespace HadesEditor.Linq
{
    public abstract class LuaToken
    {
        public abstract LuaTokenType TokenType { get; }


        public abstract LuaToken this[LuaValue key] { get; set; }


        public static implicit operator LuaToken(string rhs)
            => new LuaValue(rhs);

        public static implicit operator LuaToken(bool rhs)
            => new LuaValue(rhs);

        public static implicit operator LuaToken(double rhs)
            => new LuaValue(rhs);

        public static explicit operator bool(LuaToken token)
            => (bool)(token as LuaValue);

        public static explicit operator double(LuaToken token)
            => (double)(token as LuaValue);

        public static explicit operator string(LuaToken token)
            => (string)(token as LuaValue);

    }
}
