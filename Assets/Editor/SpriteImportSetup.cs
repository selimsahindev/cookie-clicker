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

        /// <summary>Configures only sprites that never got the project's settings, leaving tuned ones alone.</summary>
        public static void ConfigureUnconfigured()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { BuildHelpers.SpritesFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                if (!Mathf.Approximately(importer.spritePixelsPerUnit, PixelsPerUnit))
                {
                    Configure(path);
                }
            }
        }

        private static void Configure(string path)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Apply(importer, path);
            importer.SaveAndReimport();
        }

        public static bool IsGameSprite(string path)
        {
            return path.StartsWith(BuildHelpers.SpritesFolder + "/");
        }

        /// <summary>Writes the sprite settings into the importer without reimporting.</summary>
        public static void Apply(TextureImporter importer, string path)
        {
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
            importer.spriteBorder = BorderFor(Path.GetFileNameWithoutExtension(path));
        }

        /// <summary>
        /// Re-applies the slice border of a sprite that was imported before its border was known.
        /// Sliced sprites need it to stretch without distorting their ends.
        /// </summary>
        public static void EnsureBorder(string spriteName)
        {
            string path = $"{BuildHelpers.SpritesFolder}/{spriteName}.png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Vector4 border = BorderFor(spriteName);
            if (importer == null || importer.spriteBorder == border)
            {
                return;
            }

            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        /// <summary>Slice borders in texture pixels (left, bottom, right, top). Zero means the sprite isn't sliced.</summary>
        private static Vector4 BorderFor(string spriteName)
        {
            switch (spriteName)
            {
                case "cookie_holder_back":
                case "cookie_holder_front":
                    return new Vector4(40f, 0f, 40f, 0f);
                case "shop_window":
                    return new Vector4(120f, 120f, 120f, 120f);
                case "shop_row":
                    return new Vector4(80f, 80f, 80f, 80f);
                case "shop_tab":
                    return new Vector4(72f, 72f, 72f, 72f);
                default:
                    return Vector4.zero;
            }
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
                case "upgrade_card":
                case "upgrade_cost":
                case "icon_bake_speed":
                case "icon_price":
                case "icon_tap":
                case "icon_customers":
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
