using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plex_folder_soldier
{
    class FolderSoldier
    {
        private const string SeasonFolderFormat = "Season {0:D2}";
        public static void HandleDirectory(DirectoryInfo directory)
        {
            var directoryName = directory.Name;
            var directoryParentPath = directory.Parent?.FullName;
            if (string.IsNullOrWhiteSpace(directoryParentPath)) throw new DirectoryNotFoundException("The folder specified did not have a parent folder");
            Directory.Move(directory.FullName, $"{directory.Parent.FullName}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}");
            var newMainDirectory = Directory.CreateDirectory($"{directoryParentPath}{Path.DirectorySeparatorChar}{directoryName}");
            Directory.Move($"{directoryParentPath}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}", $"{newMainDirectory.FullName}{Path.DirectorySeparatorChar}{string.Format(SeasonFolderFormat, 1)}");
        }

        public static void HandleFile(FileInfo file)
        {
            if (!file.Exists) throw new FileNotFoundException("File was not found");
            if (!file.Directory?.Exists ?? true) throw new FileNotFoundException("File did not have a directory yet");

            Console.WriteLine("Enter folder name:");
            var newFolderName = Console.ReadLine();
            var newDirectory = Directory.CreateDirectory($"{file.DirectoryName}{Path.DirectorySeparatorChar}{newFolderName ?? Path.GetRandomFileName()}");

            MoveAssociatedFiles(file.Directory.EnumerateFiles().Where(siblingFile => StringDistance.LevenshteinDistance(siblingFile.Name, file.Name) < 4), newDirectory);
        }

        public static void MoveAssociatedFiles(IEnumerable<FileInfo> files, FileSystemInfo folder)
        {
            foreach (var associatedFiles in files)
            {
                associatedFiles.MoveTo($"{folder.FullName}{Path.DirectorySeparatorChar}{associatedFiles.Name}");
            }
        }
    }
}
