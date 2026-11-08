using System;
using System.IO;
using CookieClicker.Saving;
using UnityEngine;

namespace CookieClicker.Tests
{
    /// <summary>
    /// Points the game at an empty, throwaway save folder so tests never read or overwrite the real save.
    /// </summary>
    internal sealed class IsolatedSaveStorage : IDisposable
    {
        private readonly string directory;

        public IsolatedSaveStorage()
        {
            directory = Path.Combine(Application.temporaryCachePath, "save-tests", Guid.NewGuid().ToString("N"));
            Storage = new FileSaveStorage(directory);
            SaveManager.StorageOverride = Storage;
        }

        public FileSaveStorage Storage { get; }

        public void Dispose()
        {
            SaveManager.StorageOverride = null;
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
