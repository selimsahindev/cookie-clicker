using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CookieClicker.EditorTools.BuildHelpers;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Adds every feature the bakery scene is missing, keeping the existing layout as it is.
    /// Run it after pulling new features. Running it again does nothing.
    /// Command line:
    /// Unity -batchmode -quit -projectPath . -executeMethod CookieClicker.EditorTools.SceneFeatureInstaller.InstallMissing
    /// </summary>
    public static class SceneFeatureInstaller
    {
        internal static readonly ISceneFeature[] Features =
        {
            new UpgradeFeature(),
            new OvenAnimationFeature(),
            new LevelGoalFeature(),
            new PedestrianFeature(),
            new CookieHolderFeature(),
        };

        [MenuItem("Cookie Clicker/Install Missing Features")]
        public static void InstallMissing()
        {
            Scene scene = OpenBakeryScene();
            SpriteImportSetup.ConfigureUnconfigured();
            PrefabBuilder.BuildMissing();

            int installed = InstallMissingIntoOpenScene();
            if (installed == 0)
            {
                Debug.Log("The bakery scene already has every feature.");
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        /// <summary>Installs missing features into whatever scene is open and returns how many were added.</summary>
        internal static int InstallMissingIntoOpenScene()
        {
            int installed = 0;
            foreach (ISceneFeature feature in Features)
            {
                if (feature.IsInstalled())
                {
                    continue;
                }

                feature.Install();
                installed++;
                Debug.Log($"Installed feature: {feature.Name}");
            }

            return installed;
        }

        private static Scene OpenBakeryScene()
        {
            Scene active = SceneManager.GetActiveScene();
            if (active.path == ScenePath)
            {
                return active;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                throw new OperationCanceledException("Install cancelled: the open scene has unsaved changes.");
            }

            return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }
    }
}
