using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Plex_folder_soldier
{
    internal class SetupHandler
    {
        private static string _exe = Process.GetCurrentProcess().MainModule?.FileName;
        private static string _command = $"{_exe} %1";

        private static char _sep = Path.DirectorySeparatorChar;

        public static void Setup()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        SetRegistryKeys();
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine(e.Message);
                        var newAdminStart = new ProcessStartInfo(_exe) { Verb = "runas", UseShellExecute = true };
                        Process.Start(newAdminStart);
                    }
                }
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
            Console.WriteLine(choice);
            if (choice != "Y" && choice != "OK" && choice != "YES") return;

            Console.WriteLine("ASSEMBLE: ");
            Console.WriteLine(_exe);
            Console.WriteLine("\n");

            var fileKey = Registry.ClassesRoot.CreateSubKey($"*{_sep}shell{_sep}Plex soldier", RegistryKeyPermissionCheck.ReadWriteSubTree);
            fileKey?.SetValue("Icon", _exe);
            var fileCommandKey = fileKey?.CreateSubKey("command");
            fileCommandKey?.SetValue(string.Empty, _command);

            var directoryRightKey = Registry.ClassesRoot.CreateSubKey($"Directory{_sep}shell{_sep}Plex soldier");
            directoryRightKey?.SetValue("Icon", _exe);
            var rightCommandKey = directoryRightKey?.CreateSubKey("command");
            rightCommandKey?.SetValue(string.Empty, _command);

            var directoryLeftKey = Registry.ClassesRoot.CreateSubKey($"Directory{_sep}Background{_sep}shell{_sep}Plex soldier");
            directoryLeftKey?.SetValue("Icon", _exe);
            var leftCommandKey = directoryLeftKey?.CreateSubKey("command");
            leftCommandKey?.SetValue(string.Empty, _command);

            Console.WriteLine($"Successfully set keys for exe: {_exe}. \n Press a key to close.");
            Console.ReadKey();
        }
    }
}
