using System;

namespace SimpleDecoder
{
    class Program
    {

        static void Main(string[] args)
        {
            var saveEditor = new HadesSaveEditor(@"Profile1.sav");
            saveEditor.LoadFile();
            saveEditor.EditFile("GameState.Resources.SuperLockKeys", 69.0);
            saveEditor.SaveFile("editedfile.sav");
            Console.Read();
        }

    }
}
