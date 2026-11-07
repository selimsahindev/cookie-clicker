using System.IO;
using UnityEditor;
using UnityEngine;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Shared constants and small helpers used by the scene and prefab builders.
    /// </summary>
    internal static class BuildHelpers
    {
        public const string SpritesFolder = "Assets/Art/Sprites";
        public const string PrefabsFolder = "Assets/Prefabs";

        public static class Palette
        {
            public static readonly Color Ink = new Color32(0x4A, 0x2E, 0x22, 0xFF);
            public static readonly Color RingTrack = new Color32(0x4A, 0x2E, 0x22, 0x55);
            public static readonly Color RingFill = new Color32(0xE8, 0x6F, 0x6F, 0xFF);
            public static readonly Color Sky = new Color32(0x9F, 0xD3, 0xEC, 0xFF);
        }

        public static class SortingOrder
        {
            public const int Background = -100;
            public const int Oven = 0;
            public const int Baker = 5;
            public const int Umbrella = 8;
            public const int Table = 10;
            public const int TableCookies = 20;
            public const int CustomerShadow = 25;
            public const int Customer = 30;
            public const int Ring = 50;
            public const int Bubble = 60;
            public const int Hud = 100;
        }

        public static GameObject CreateChild(Transform parent, string name, Vector3 localPosition)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPosition;
            return child;
        }

        public static RectTransform CreateUiChild(Transform parent, string name, Vector2 size)
        {
            var child = new GameObject(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            var rect = (RectTransform)child.transform;
            rect.sizeDelta = size;
            return rect;
        }

        public static SpriteRenderer AddSprite(GameObject target, Sprite sprite, int sortingOrder)
        {
            var spriteRenderer = target.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            return spriteRenderer;
        }

        public static Sprite LoadSprite(string spriteName)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SpritesFolder}/{spriteName}.png");
            if (sprite == null)
            {
                throw new FileNotFoundException($"Sprite '{spriteName}' not found in {SpritesFolder}. Run Tools/svg-rasterizer first.");
            }

            return sprite;
        }

        public static void SetReference(Object target, string field, Object value)
        {
            Apply(target, field, property => property.objectReferenceValue = value);
        }

        public static void SetInt(Object target, string field, int value)
        {
            Apply(target, field, property => property.intValue = value);
        }

        public static void SetFloat(Object target, string field, float value)
        {
            Apply(target, field, property => property.floatValue = value);
        }

        public static void SetVector3(Object target, string field, Vector3 value)
        {
            Apply(target, field, property => property.vector3Value = value);
        }

        public static void SetSpriteArray(Object target, string field, string[] spriteNames)
        {
            Apply(target, field, property =>
            {
                property.arraySize = spriteNames.Length;
                for (int i = 0; i < spriteNames.Length; i++)
                {
                    property.GetArrayElementAtIndex(i).objectReferenceValue = LoadSprite(spriteNames[i]);
                }
            });
        }

        public static void EnsureFolder(string folder)
        {
            folder = folder.Replace('\\', '/');
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(folder));
        }

        private static void Apply(Object target, string field, System.Action<SerializedProperty> assign)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            if (property == null)
            {
                throw new System.ArgumentException($"{target.GetType().Name} has no serialized field '{field}'.");
            }

            assign(property);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
