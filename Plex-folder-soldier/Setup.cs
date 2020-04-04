using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Plex_folder_soldier
{
    internal class Setup
    {
        private static readonly string Exe = Process.GetCurrentProcess().MainModule?.FileName;
        private static readonly string Command = $"{Exe} %1";

        private static readonly char Sep = Path.DirectorySeparatorChar;

        public static void Run()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    SetRegistryKeys();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                var newAdminStart = new ProcessStartInfo(Exe) { Verb = "runas", UseShellExecute = true };
                Process.Start(newAdminStart);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        private static void SetRegistryKeys()
        {
            Console.WriteLine("Do you want to add windows registry keys for explorer access? (Y/N)");
            var choice = Console.ReadLine()?.ToUpper();
            if (choice != "Y" && choice != "OK" && choice != "YES") return;

            var fileKey = Registry.ClassesRoot.CreateSubKey($"*{Sep}shell{Sep}Plex soldier", RegistryKeyPermissionCheck.ReadWriteSubTree);
            fileKey?.SetValue("Icon", Exe);
            NoticeIfNull(fileKey, "Cannot set file key icon, skipping.");

            var fileCommandKey = fileKey?.CreateSubKey("command");
            fileCommandKey?.SetValue(string.Empty, Command);
            NoticeIfNull(fileCommandKey, "Cannot set file key command, skipping.");


            var directoryRightKey = Registry.ClassesRoot.CreateSubKey($"Directory{Sep}shell{Sep}Plex soldier");
            directoryRightKey?.SetValue("Icon", Exe);
            NoticeIfNull(directoryRightKey, "Cannot set directory right key icon, skipping.");

            var rightCommandKey = directoryRightKey?.CreateSubKey("command");
            rightCommandKey?.SetValue(string.Empty, Command);
            NoticeIfNull(rightCommandKey, "Cannot set directory right key command, skipping.");


            var directoryLeftKey = Registry.ClassesRoot.CreateSubKey($"Directory{Sep}Background{Sep}shell{Sep}Plex soldier");
            directoryLeftKey?.SetValue("Icon", Exe);
            NoticeIfNull(directoryLeftKey, "Cannot set directory left key icon, skipping.");

            var leftCommandKey = directoryLeftKey?.CreateSubKey("command");
            leftCommandKey?.SetValue(string.Empty, Command);
            NoticeIfNull(leftCommandKey, "Cannot set directory left key command, skipping.");


            Console.WriteLine($"Finished setting keys for exe: {Exe}. \n Press a key to close.");
            Console.ReadKey();
        }

        private static void NoticeIfNull(object obj, string message)
        {
            if (obj == null)
            {
                Console.WriteLine(message);
            }
        }
    }
}
