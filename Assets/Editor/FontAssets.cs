using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Creates TextMeshPro font assets from the TTF files in Assets/Art/Fonts.
    /// </summary>
    internal static class FontAssets
    {
        public const string DisplayFontPath = "Assets/Art/Fonts/LilitaOne-Regular.ttf";
        private const string PreloadedCharacters = "GoalDrem Shp!ready0123456789%$,./ ";

        /// <summary>The rounded display font used for headings, loaded or created on first use.</summary>
        public static TMP_FontAsset LoadOrCreateDisplayFont()
        {
            string assetPath = DisplayFontPath.Replace(".ttf", " SDF.asset");
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset != null)
            {
                return fontAsset;
            }

            var font = AssetDatabase.LoadAssetAtPath<Font>(DisplayFontPath);
            fontAsset = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
            fontAsset.TryAddCharacters(PreloadedCharacters);

            AssetDatabase.CreateAsset(fontAsset, assetPath);
            fontAsset.material.name = $"{fontAsset.name} Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            foreach (Texture2D atlas in fontAsset.atlasTextures)
            {
                atlas.name = $"{fontAsset.name} Atlas";
                AssetDatabase.AddObjectToAsset(atlas, fontAsset);
            }

            AssetDatabase.SaveAssets();
            return fontAsset;
        }
    }
}
