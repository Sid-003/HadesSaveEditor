using HadesEditor.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HadesEditor
{
    // Lua simple Encoder/Decoder
    // Adapted (more like stole) from https://github.com/zsennenga/luabins_py

    public class LuaEncoder : IDisposable
    {
        private const byte LUABINS_NIL = 0x2D;
        private const byte LUABINS_FALSE = 0x30;
        private const byte LUABINS_TRUE = 0x31;
        private const byte LUABINS_NUMBER = 0x4E;
        private const byte LUABINS_STRING = 0x53;
        private const byte LUABINS_TABLE = 0x54;
        private readonly BinaryReader _luaBinStream;
        private BinaryWriter _luaBinWriter;


        public LuaEncoder(Stream stream)
        {
            _luaBinStream = new BinaryReader(stream);
           
        }


        public byte[] Encode(List<LuaTable> values)
        {
            using var ms = new MemoryStream();
            _luaBinWriter = new BinaryWriter(ms);
            _luaBinWriter.Write((byte)values.Count);

            foreach (var value in values)
                SaveValue(value);

            return ms.ToArray();
        }

        public List<LuaTable> Decode()
        {
            var list = new List<LuaTable>();
            int n = _luaBinStream.ReadByte();
            for (int i = 0; i < n; i++)
                list.Add(LoadValue() as LuaTable);
            return list;
        }

        public void SaveValue(LuaToken value)
        {
            if(value == null)
            {
                _luaBinWriter.Write(LUABINS_NIL);
                return;
            }
            switch (value.TokenType)
            {
                case LuaTokenType.Table:
                    _luaBinWriter.Write(LUABINS_TABLE);
                    WriteTable(value as LuaTable);
                    return;
                case LuaTokenType.Double:
                    _luaBinWriter.Write(LUABINS_NUMBER);
                    _luaBinWriter.Write((double)value);
                    return;
                case LuaTokenType.String:
                    _luaBinWriter.Write(LUABINS_STRING);
                    var s = (string) value;
                    _luaBinWriter.Write(s.Length);
                    _luaBinWriter.Write(Encoding.ASCII.GetBytes(s));
                    return;
                case LuaTokenType.Boolean:
                    if((bool)value)
                        _luaBinWriter.Write(LUABINS_TRUE);
                    else
                        _luaBinWriter.Write(LUABINS_FALSE);
                    return;
            };
        }


        public LuaToken LoadValue()
        {
            byte type = _luaBinStream.ReadByte();

            return type switch
            {
                LUABINS_TRUE => true,
                LUABINS_FALSE => false,
                LUABINS_NIL => null,
                LUABINS_NUMBER => _luaBinStream.ReadDouble(),
                LUABINS_STRING  => Encoding.ASCII.GetString(_luaBinStream.ReadBytes(_luaBinStream.ReadInt32())),
                LUABINS_TABLE => ReadTable(),
                _ => throw new ArgumentException("bruh")
            };
        }


        private LuaTable ReadTable()
        {
            var dict = new LuaTable();

            int arrSize = _luaBinStream.ReadInt32();
            int hashSize = _luaBinStream.ReadInt32();

            int totalSize = arrSize + hashSize;

            for (int i = 0; i < totalSize; i++)
            {
                var key = LoadValue() as LuaValue;
                var val = LoadValue();
                dict[key] = val;
            }
            return dict;
        }

        private void WriteTable(LuaTable table)
        {
            //ahhhhhhhhhhhhhhhhhhhhhh
            var arraySize = 0;
            var hashSize = table.Count - arraySize;

            _luaBinWriter.Write(arraySize);
            _luaBinWriter.Write(hashSize);

            foreach (var (k, v) in table)
            {
                SaveValue(k);
                SaveValue(v);
            }     
        }

        public void Dispose()
        {
            _luaBinStream.Dispose();
            _luaBinWriter.Dispose();
        }
    }
}
