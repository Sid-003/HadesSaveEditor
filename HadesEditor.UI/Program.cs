using System;

namespace HadesEditor.UI
{
    class Program
    {

        static void Main(string[] args)
        {
            var saveEditor = new HadesSaveEditor(@"Profile1.sav");
            saveEditor.LoadFile();
            saveEditor.EditFile("GameState.Resources.SuperLockKeys", 69.0);
            saveEditor.SaveFile("Profile1.sav");
            Console.Read();
        }

    }
}
