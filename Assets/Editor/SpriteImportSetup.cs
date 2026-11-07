using System.IO;
using UnityEditor;
using UnityEngine;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Applies sprite import settings and pivots to every rasterized SVG.
    /// </summary>
    internal static class SpriteImportSetup
    {
        private const float PixelsPerUnit = 200f;

        public static void ConfigureAll()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { BuildHelpers.SpritesFolder }))
            {
                Configure(AssetDatabase.GUIDToAssetPath(guid));
            }
        }

        private static void Configure(string path)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            importer.SetTextureSettings(settings);
            importer.spritePivot = PivotFor(Path.GetFileNameWithoutExtension(path));
            importer.SaveAndReimport();
        }

        private static Vector2 PivotFor(string spriteName)
        {
            switch (spriteName)
            {
                case "background":
                case "cookie":
                case "shadow":
                case "money":
                case "hud_pill":
                    return new Vector2(0.5f, 0.5f);
                case "bubble":
                    return new Vector2(0.485f, 0.05f);
                case "vendor_arm_left":
                case "vendor_arm_right":
                    return new Vector2(0.5f, 1f - 12f / 130f);
                default:
                    return new Vector2(0.5f, 0f);
            }
        }
    }
}
