using System;
using System.IO;
using CookieClicker.Saving;
using UnityEditor;
using UnityEngine;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Editor window to inspect and reset the local save (Cookie Clicker → Game Data).
    /// </summary>
    public class GameDataWindow : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Cookie Clicker/Game Data")]
        private static void Open()
        {
            GetWindow<GameDataWindow>("Game Data");
        }

        [MenuItem("Cookie Clicker/Reset Game Data")]
        private static void ResetFromMenu()
        {
            ConfirmAndReset();
        }

        private void OnGUI()
        {
            var storage = FileSaveStorage.CreateDefault();
            EditorGUILayout.LabelField("Save file", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(storage.MainPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            DrawSummary(storage);
            EditorGUILayout.Space();
            DrawButtons(storage);
        }

        private void DrawSummary(FileSaveStorage storage)
        {
            if (!storage.TryRead(out string json) || !SaveSerializer.TryDeserialize(json, out SaveData data))
            {
                EditorGUILayout.HelpBox("No save yet. The game starts fresh.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Saved at (local)", new DateTime(data.savedAtUtcTicks, DateTimeKind.Utc).ToLocalTime().ToString("g"));
            EditorGUILayout.LabelField("Money", data.money.ToString("N0"));
            EditorGUILayout.LabelField("Cookies on cart", data.cookiesOnCart.ToString());
            foreach (SaveData.UpgradeLevel upgrade in data.upgrades)
            {
                EditorGUILayout.LabelField($"Upgrade: {upgrade.id}", $"Lv {upgrade.level}");
            }

            foreach (string unlock in data.unlocks)
            {
                EditorGUILayout.LabelField($"Unlocked: {unlock}", "Owned");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Raw JSON", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinHeight(120));
            EditorGUILayout.TextArea(json);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawButtons(FileSaveStorage storage)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Reveal in Finder"))
                {
                    EditorUtility.RevealInFinder(File.Exists(storage.MainPath) ? storage.MainPath : Application.persistentDataPath);
                }

                if (GUILayout.Button("Reset Game Data"))
                {
                    ConfirmAndReset();
                }
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Resetting while playing deletes the save and stops play mode, so the running game can't save over it.", MessageType.Info);
            }
        }

        private static void ConfirmAndReset()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Game Data",
                "This deletes the save file: money, upgrade levels, shop unlocks and cookies on the cart.\n\nIt cannot be undone.",
                "Delete Save",
                "Cancel");
            if (!confirmed)
            {
                return;
            }

            if (Application.isPlaying)
            {
                // The game saves while quitting, so delete only once play mode has fully stopped.
                EditorApplication.playModeStateChanged += DeleteAfterPlayModeStops;
                EditorApplication.ExitPlaymode();
                return;
            }

            DeleteSave();
        }

        private static void DeleteAfterPlayModeStops(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            EditorApplication.playModeStateChanged -= DeleteAfterPlayModeStops;
            DeleteSave();
        }

        private static void DeleteSave()
        {
            FileSaveStorage.CreateDefault().Delete();
            Debug.Log("Game data reset.");
        }
    }
}
