using HadesEditor.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HadesEditor.UI
{
    class Program
    {

        static void Main(string[] args)
        {
            var saveEditor = new HadesSaveEditor(@"Profile1.sav");
            saveEditor.LoadFile();
            Console.WriteLine("loaaded file");
            saveEditor.EditFile<LuaValue>("GameState.Resources.SuperLockKeys", x => 
            {
                x.Value = 420.0;
            });
            saveEditor.EditFile<LuaTable>("GameState.WeaponUnlocks", x =>
            {
                x.Remove("GunWeapon");
            });
            saveEditor.SaveFile("Profile1.sav");
            Console.WriteLine("saved file");
            Console.Read();
        }

    }
}
