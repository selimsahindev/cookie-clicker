using UnityEditor;
using UnityEngine;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Locks the game to portrait on mobile and opens a portrait window on desktop.
    /// </summary>
    public static class PlayerSetup
    {
        private const int DesktopWindowWidth = 540;
        private const int DesktopWindowHeight = 960;

        public static void ConfigurePortrait()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = DesktopWindowWidth;
            PlayerSettings.defaultScreenHeight = DesktopWindowHeight;
            PlayerSettings.resizableWindow = true;
        }
    }
}
