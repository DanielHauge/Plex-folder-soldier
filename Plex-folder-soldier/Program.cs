using System;
using System.IO;

namespace Plex_folder_soldier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up with args: "+ string.Join(" ", args));
            if (args.Length <= 0)
            {
                Setup.Run();
            }
            else
            {
                var arg = string.Join(" ", args);
                if (string.IsNullOrWhiteSpace(arg)) return;
                if (Path.HasExtension(arg))
                {
                    FolderSoldier.HandleFile(new FileInfo(arg));
                }
                else
                {
                    FolderSoldier.HandleDirectory(new DirectoryInfo(arg));
                }
            }
        }


        
    }
}
