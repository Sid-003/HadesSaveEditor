using HadesEditor.Linq;
using LZ4;
using System;
using System.IO;

namespace HadesEditor
{
    public class HadesSaveEditor
    {
        private readonly BinaryLoadData _binaryLoadData;

        private LuaEncoder _luaEncoder;

        private HadesSaveFile _saveFile;

        public HadesSaveEditor(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The file " + filePath + " does not exist.");


            _binaryLoadData = new BinaryLoadData(true);
            using var fs = File.OpenRead(filePath);
            _binaryLoadData.fromStream(fs);
        }


        public void LoadFile()
        {
            _saveFile = new HadesSaveFile()
            {
                 Version = _binaryLoadData.loadInt(),
                 Timestamp = new DateTime(_binaryLoadData.loadLong()),
                 Location = _binaryLoadData.loadString(),
                 CompletedRuns = _binaryLoadData.loadInt(),
                 ActiveMetaPoints= _binaryLoadData.loadInt(), 
                 ActiveShrinePoints= _binaryLoadData.loadInt() ,
                 GodMode = _binaryLoadData.loadBool(),
                 HellMode = _binaryLoadData.loadBool(),
                 LuaKeys= _binaryLoadData.loadStringList(),
                 CurrentMap = _binaryLoadData.loadString(),
                 NextMap = _binaryLoadData.loadString(),                 
            };

            var rawLuaBytes = _binaryLoadData.loadLuaString();


            var lmao = LZ4Codec.Decode(rawLuaBytes, 0, rawLuaBytes.Length, 9388032);
            
            _luaEncoder = new LuaEncoder(new MemoryStream(rawLuaBytes));
             var luaState = _luaEncoder.Decode();
            _saveFile.LuaState = luaState;
        }


        public void SaveFile(string path)
        {
            var bs = new BSaveData(3145736, true);
            bs.saveInt(_saveFile.Version);
            bs.saveLong(_saveFile.Timestamp.ToBinary());
            bs.saveString(_saveFile.Location);
            bs.saveInt(_saveFile.CompletedRuns);
            bs.saveInt(_saveFile.ActiveMetaPoints);
            bs.saveInt(_saveFile.ActiveShrinePoints);
            bs.saveBool(_saveFile.GodMode);
            bs.saveBool(_saveFile.HellMode);
            bs.saveStringList(_saveFile.LuaKeys);
            bs.saveString(_saveFile.CurrentMap);
            bs.saveString(_saveFile.NextMap);
            var encodedLuaState = _luaEncoder.Encode(_saveFile.LuaState);
            bs.saveRawBytes(encodedLuaState);

            var ms = new MemoryStream();
            bs.toStream(ms);
            File.WriteAllBytes(path, ms.ToArray());

            File.WriteAllText("yeet.txt", _saveFile.LuaState.ToString());
        }

        public void EditFile<T>(string property, Action<T> modify) where T : LuaToken
        {
            var keys = property.Split('.');

            //always one element least in the current version (long winter update)
            LuaToken val = _saveFile.LuaState[0];

            for (int i = 0; i < keys.Length; i++)
            {
                val = val[keys[i]];
            }

            modify(val as T);
        }



    }
}
