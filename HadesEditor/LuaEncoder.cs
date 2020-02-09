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


        public byte[] Encode(JArray values)
        {
            using var ms = new MemoryStream();
            _luaBinWriter = new BinaryWriter(ms);
            _luaBinWriter.Write((byte)values.Count);

            foreach (var value in values)
                SaveValue(value);

            return ms.ToArray();
        }

        public JArray Decode()
        {
            var list = new JArray();
            int n = _luaBinStream.ReadByte();
            for (int i = 0; i < n; i++)
                list.Add(LoadValue());
            return list;
        }

        public void SaveValue(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.Object:
                    _luaBinWriter.Write(LUABINS_TABLE);
                    WriteTable(value.ToObject<JObject>());
                    return;
                case JTokenType.Float:
                    _luaBinWriter.Write(LUABINS_NUMBER);
                    _luaBinWriter.Write(value.ToObject<double>());
                    return;
                case JTokenType.String:
                    _luaBinWriter.Write(LUABINS_STRING);
                    var s = value.ToObject<string>();
                    _luaBinWriter.Write(s.Length);
                    _luaBinWriter.Write(Encoding.ASCII.GetBytes(s));
                    return;
                case JTokenType.Null:
                    _luaBinWriter.Write(LUABINS_NIL);
                    return;
                case JTokenType.Boolean:
                    if(value.ToObject<bool>())
                        _luaBinWriter.Write(LUABINS_TRUE);
                    else
                        _luaBinWriter.Write(LUABINS_FALSE);
                    return;
            };
        }


        public JToken LoadValue()
        {
            byte type = _luaBinStream.ReadByte();

            return type switch
            {
                LUABINS_TRUE => new JValue(true),
                LUABINS_FALSE => new JValue(false),
                LUABINS_NIL => null,
                LUABINS_NUMBER => new JValue(_luaBinStream.ReadDouble()),
                LUABINS_STRING  => new JValue(Encoding.ASCII.GetString(_luaBinStream.ReadBytes(_luaBinStream.ReadInt32()))),
                LUABINS_TABLE => ReadTable(),
                _ => throw new ArgumentException("bruh")
            };
        }


        private JObject ReadTable()
        {
            var dict = new JObject();

            int arrSize = _luaBinStream.ReadInt32();
            int hashSize = _luaBinStream.ReadInt32();

            int totalSize = arrSize + hashSize;

            for (int i = 0; i < totalSize; i++)
            {
                var key = LoadValue();
                var val = LoadValue();
                dict[key.ToObject<string>()] = val;
            }
            return dict;
        }

        private void WriteTable(JObject table)
        {
            var arraySize = table.Children().Count(x => x.Type == JTokenType.Integer);
            var hashSize = table.Count - arraySize;

            _luaBinWriter.Write(arraySize);
            _luaBinWriter.Write(hashSize);

            foreach (var (k, v) in table)
            {
                if (double.TryParse(k, out double res))
                    SaveValue(res);
                else
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
