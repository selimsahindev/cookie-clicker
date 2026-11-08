using System;
using UnityEngine;

namespace CookieClicker.Saving
{
    /// <summary>
    /// Turns <see cref="SaveData"/> into text and back, rejecting broken or too-new saves.
    /// </summary>
    public static class SaveSerializer
    {
        public static string Serialize(SaveData data)
        {
            return JsonUtility.ToJson(data, true);
        }

        public static bool TryDeserialize(string json, out SaveData data)
        {
            data = null;
            try
            {
                data = JsonUtility.FromJson<SaveData>(json);
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning($"Save data is corrupted and was ignored: {exception.Message}");
                return false;
            }

            if (data == null)
            {
                return false;
            }

            if (data.version > SaveData.CurrentVersion)
            {
                Debug.LogWarning($"Save version {data.version} is newer than this build supports ({SaveData.CurrentVersion}) and was ignored.");
                data = null;
                return false;
            }

            return true;
        }
    }
}
