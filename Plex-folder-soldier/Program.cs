using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plex_folder_soldier
{
    class Program
    {
        private const string SeasonFolderFormat = "Season {0:D2}";
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up with args: "+ string.Join(" ", args));
            if (args.Length <= 0)
            {
                SetupHandler.Setup();
            }
            else
            {
                var arg = string.Join(" ", args);
                if (string.IsNullOrWhiteSpace(arg)) return;
                if (Path.HasExtension(arg)) // A single file was chosen.
                {
                    HandleFile(new FileInfo(arg));
                }
                else
                {
                    HandleDirectory(new DirectoryInfo(arg));
                }
            }
            //else // MULTIPLE STRING INPUTS
            //{
            //    HandleMultiple(args);
            //}
        }


        private static void HandleDirectory(DirectoryInfo directory)
        {
            var directoryName = directory.Name;
            var directoryParentPath = directory.Parent?.FullName;
            if (string.IsNullOrWhiteSpace(directoryParentPath)) throw new DirectoryNotFoundException("The folder specified did not have a parent folder");
            Directory.Move(directory.FullName, $"{directory.Parent.FullName}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}");
            var newMainDirectory = Directory.CreateDirectory($"{directoryParentPath}{Path.DirectorySeparatorChar}{directoryName}");
            Directory.Move($"{directoryParentPath}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}", $"{newMainDirectory.FullName}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}");
        }

        private static void HandleFile(FileInfo file)
        {
            if (!file.Exists) throw new FileNotFoundException("File was not found");
            if (!file.Directory?.Exists ?? true) throw new FileNotFoundException("File did not have a directory yet");

            Console.WriteLine("Enter folder name:");
            var newFolderName = Console.ReadLine();
            var newDirectory = Directory.CreateDirectory($"{file.DirectoryName}{Path.DirectorySeparatorChar}{newFolderName ?? Path.GetRandomFileName()}");
            //var seasonOneDirectory = Directory.CreateDirectory( $"{newDirectory.Name}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}");

            MoveAssociatedFiles(file.Directory.EnumerateFiles().Where(siblingFile => StringDistance.LevenshteinDistance(siblingFile.Name, file.Name) < 4), newDirectory);
        }

        private static void MoveAssociatedFiles(IEnumerable<FileInfo> files, FileSystemInfo folder)
        {
            foreach (var associatedFiles in files)
            {
                associatedFiles.MoveTo($"{folder.FullName}{Path.DirectorySeparatorChar}{associatedFiles.Name}");
            }
        }


        //private static void HandleMultiple(string[] args)
        //{
        //    var directoryName = Path.GetDirectoryName(args.First());

        //    var newFolderName = Console.ReadLine();
        //    var newDirectory = Directory.CreateDirectory($"{directoryName}{Path.DirectorySeparatorChar}{newFolderName ?? Path.GetRandomFileName()}");
        //    foreach (var fileOrFolder in args)
        //    {
        //        if (File.Exists(fileOrFolder))
        //        {
        //            var file = new FileInfo(fileOrFolder);
        //            File.Move(fileOrFolder, $"{newDirectory.FullName}{Path.DirectorySeparatorChar}{file.Name}");
        //        }
        //        else if (Directory.Exists(fileOrFolder))
        //        {
        //            var folder = new DirectoryInfo(fileOrFolder);
        //            Directory.Move(fileOrFolder, $"{newDirectory.FullName}{Path.DirectorySeparatorChar}{folder.Name}");
        //        }
        //    }
        //}
    }
}
