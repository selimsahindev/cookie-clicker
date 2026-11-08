using System.IO;
using UnityEngine;

namespace CookieClicker.Saving
{
    /// <summary>
    /// Stores the save as a file in a writable folder (Application.persistentDataPath on device).
    /// Writes are crash-safe: the new save goes to a temp file first, the previous save is kept as a backup,
    /// and reading falls back to the backup if the main file is missing or unreadable.
    /// </summary>
    public class FileSaveStorage : ISaveStorage
    {
        public const string DefaultFileName = "save.json";

        private readonly string mainPath;
        private readonly string tempPath;
        private readonly string backupPath;

        public FileSaveStorage(string directory, string fileName = DefaultFileName)
        {
            Directory.CreateDirectory(directory);
            mainPath = Path.Combine(directory, fileName);
            tempPath = mainPath + ".tmp";
            backupPath = mainPath + ".bak";
        }

        public static FileSaveStorage CreateDefault()
        {
            return new FileSaveStorage(Application.persistentDataPath);
        }

        public string MainPath => mainPath;

        public bool TryRead(out string contents)
        {
            return TryReadFile(mainPath, out contents) || TryReadFile(backupPath, out contents);
        }

        /// <summary>Reads the backup only. Used when the main file exists but holds broken data.</summary>
        public bool TryReadBackup(out string contents)
        {
            return TryReadFile(backupPath, out contents);
        }

        public void Write(string contents)
        {
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(contents);
                writer.Flush();
                stream.Flush(true);
            }

            if (File.Exists(mainPath))
            {
                File.Copy(mainPath, backupPath, true);
                File.Delete(mainPath);
            }

            File.Move(tempPath, mainPath);
        }

        public void Delete()
        {
            DeleteIfExists(mainPath);
            DeleteIfExists(tempPath);
            DeleteIfExists(backupPath);
        }

        private static bool TryReadFile(string path, out string contents)
        {
            contents = null;
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                contents = File.ReadAllText(path);
                return !string.IsNullOrWhiteSpace(contents);
            }
            catch (IOException exception)
            {
                Debug.LogWarning($"Could not read save file {path}: {exception.Message}");
                return false;
            }
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
