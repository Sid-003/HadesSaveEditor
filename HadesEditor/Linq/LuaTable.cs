using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HadesEditor.Linq
{
    [DebuggerDisplay("(LuaTable, Count = {Count})")]
    public class LuaTable : LuaToken, IEnumerable<KeyValuePair<LuaValue, LuaToken>>
    {
        private readonly Dictionary<LuaValue, LuaToken> _backingDict;

        public override LuaTokenType TokenType => LuaTokenType.Table;


        public int Count => _backingDict.Count;

        public LuaTable()
        {
            _backingDict = new Dictionary<LuaValue, LuaToken>();
        }

        public bool Add(LuaValue key, LuaToken value)
            => _backingDict.TryAdd(key, value);

        public bool Remove(LuaValue key)
            => _backingDict.Remove(key);

        public IEnumerator<KeyValuePair<LuaValue, LuaToken>> GetEnumerator()
            => _backingDict.GetEnumerator();
       
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override LuaToken this[LuaValue key]
        {
            get => _backingDict[key];
            set => _backingDict[key] = value;           
        }
    }
}
