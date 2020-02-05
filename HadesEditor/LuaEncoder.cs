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
        const byte LUABINS_NIL = 0x2D;
        const byte LUABINS_FALSE = 0x30;
        const byte LUABINS_TRUE = 0x31;
        const byte LUABINS_NUMBER = 0x4E;
        const byte LUABINS_STRING = 0x53;
        const byte LUABINS_TABLE = 0x54;
        private BinaryReader _luaBinStream;
        private BinaryWriter _luaBinWriter;


        public LuaEncoder(Stream stream)
        {
            _luaBinStream = new BinaryReader(stream);
           
        }


        public byte[] Encode(List<object> values)
        {
            using var ms = new MemoryStream();
            _luaBinWriter = new BinaryWriter(ms);
            _luaBinWriter.Write((byte)values.Count);

            foreach (var value in values)
                SaveValue(value);

            return ms.ToArray();
        }

        public List<object> Decode()
        {
            var list = new List<object>();
            int n = _luaBinStream.ReadByte();
            for (int i = 0; i < n; i++)
                list.Add(LoadValue());
            return list;
        }

        public void SaveValue(object value)
        {
            switch (value)
            {
                case Dictionary<object, object> dict:
                    _luaBinWriter.Write(LUABINS_TABLE);
                    WriteTable(dict);
                    return;
                case double d:
                    _luaBinWriter.Write(LUABINS_NUMBER);
                    _luaBinWriter.Write(d);
                    return;
                case string s:
                    _luaBinWriter.Write(LUABINS_STRING);
                    _luaBinWriter.Write(s.Length);
                    _luaBinWriter.Write(Encoding.ASCII.GetBytes(s));
                    return;
                case null:
                    _luaBinWriter.Write(LUABINS_NIL);
                    return;
                case true:
                    _luaBinWriter.Write(LUABINS_TRUE);
                    return;
                case false:
                    _luaBinWriter.Write(LUABINS_FALSE);
                    return;
            };
        }


        public object LoadValue()
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


        private Dictionary<object, object> ReadTable()
        {
            var dict = new Dictionary<object, object>();

            int arrSize = _luaBinStream.ReadInt32();
            int hashSize = _luaBinStream.ReadInt32();

            int totalSize = arrSize + hashSize;

            for (int i = 0; i < totalSize; i++)
            {
                var key = LoadValue();
                var val = LoadValue();
                dict[key] = val;
            }
            return dict;
        }

        private void WriteTable(Dictionary<object, object> table)
        {
            var arraySize = table.Count(x => x.Key is int);
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
