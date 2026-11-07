using UnityEditor;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Gives newly added sprites in Assets/Art/Sprites the project's import settings.
    /// Only the first import is touched, so settings changed later in the Inspector are kept.
    /// </summary>
    internal class SpriteImportPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetImporter.importSettingsMissing || !SpriteImportSetup.IsGameSprite(assetPath))
            {
                return;
            }

            SpriteImportSetup.Apply((TextureImporter)assetImporter, assetPath);
        }
    }
}
