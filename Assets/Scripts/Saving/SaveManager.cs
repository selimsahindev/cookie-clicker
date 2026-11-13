using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CookieClicker.Saving
{
    /// <summary>
    /// Loads the game state on start and saves it periodically, when the app goes to the background
    /// (the moment mobile OSes may kill it) and when it quits.
    /// Created automatically in every scene, so nothing has to be added to the scene by hand.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float autosaveInterval = 10f;

        private ISaveStorage storage;
        private float timeSinceSave;

        /// <summary>Lets tests use their own storage instead of the real save file.</summary>
        internal static ISaveStorage StorageOverride { get; set; }

        public static SaveManager Instance { get; private set; }

        public event Action Saved;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateIfMissing()
        {
            if (FindAnyObjectByType<SaveManager>() == null)
            {
                new GameObject(nameof(SaveManager)).AddComponent<SaveManager>();
            }
        }

        private void Awake()
        {
            Instance = this;
            storage = StorageOverride ?? FileSaveStorage.CreateDefault();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            Load();
        }

        private void Update()
        {
            timeSinceSave += Time.unscaledDeltaTime;
            if (timeSinceSave >= autosaveInterval)
            {
                Save();
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                Save();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            timeSinceSave = 0f;
            var data = new SaveData { savedAtUtcTicks = DateTime.UtcNow.Ticks };
            foreach (ISaveable saveable in FindSaveables())
            {
                saveable.Capture(data);
            }

            try
            {
                storage.Write(SaveSerializer.Serialize(data));
                Saved?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Saving failed: {exception}");
            }
        }

        private void Load()
        {
            SaveData data = ReadSave();
            if (data == null)
            {
                return;
            }

            foreach (ISaveable saveable in FindSaveables().OrderBy(saveable => saveable.RestoreOrder))
            {
                saveable.Restore(data);
            }
        }

        private SaveData ReadSave()
        {
            if (storage.TryRead(out string json) && SaveSerializer.TryDeserialize(json, out SaveData data))
            {
                return data;
            }

            if (storage is FileSaveStorage fileStorage
                && fileStorage.TryReadBackup(out string backupJson)
                && SaveSerializer.TryDeserialize(backupJson, out SaveData backup))
            {
                Debug.LogWarning("Main save was unreadable, loaded the backup instead.");
                return backup;
            }

            return null;
        }

        private static IEnumerable<ISaveable> FindSaveables()
        {
            return FindObjectsByType<MonoBehaviour>().OfType<ISaveable>();
        }
    }
}
