using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HadesEditor.Linq
{
    [DebuggerDisplay("{Value}")]
    public class LuaValue : LuaToken, IEquatable<LuaValue>
    {
        public object Value { get; set; }

        public override LuaTokenType TokenType { get; }

        public override LuaToken this[LuaValue key] 
        {   
            get => throw new NotSupportedException("Can't do that, not a table."); 
            set => throw new NotSupportedException("Can't do that either, not a table.");
        }

        public LuaValue(string value)
            => (Value, TokenType) = (value, LuaTokenType.String);

        public LuaValue(bool value)
            => (Value, TokenType) = (value, LuaTokenType.Boolean);

        public LuaValue(double value)
            => (Value, TokenType) = (value, LuaTokenType.Double);

        public bool Equals(LuaValue other)
        {
            if (TokenType != other.TokenType)
                return false;

            switch (other.TokenType)
            {
                case LuaTokenType.String:
                    {
                        var cv = Value as string;
                        var ov = other.Value as string;
                        return cv == ov;
                    }

                case LuaTokenType.Double:
                    {
                        var cv = (double)Value;
                        var ov = (double)other.Value;
                        return cv == ov;
                    }
            }

            return false;
        }

        public override int GetHashCode()
            => Value.GetHashCode();


        public static implicit operator LuaValue(string rhs)
            => new LuaValue(rhs);

        public static implicit operator LuaValue(bool rhs)
            => new LuaValue(rhs);

        public static implicit operator LuaValue(double rhs)
            => new LuaValue(rhs);

        public static explicit operator bool(LuaValue o)
           => (bool)o.Value;

        public static explicit operator string(LuaValue o)
            => o.Value as string;

        public static explicit operator double(LuaValue o)
            => (double)o.Value;
    }
}
