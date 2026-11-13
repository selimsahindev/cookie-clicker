using System;
using CookieClicker.Table;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Puts a wooden holder around the existing cookie stacks, with a "count/capacity" label on its front board.
    /// The stacks get a SortingGroup, so the front board can cover every cookie but never a customer.
    /// </summary>
    internal class CookieHolderFeature : ISceneFeature
    {
        private const string BackSpriteName = "cookie_holder_back";
        private const string FrontSpriteName = "cookie_holder_front";
        private const float FrontBoardHeight = 0.26f;
        private static readonly Color LabelColor = new Color32(0xFF, 0xF6, 0xE6, 0xFF);

        public string Name => "Cookie holder and count";

        public bool IsInstalled()
        {
            return Object.FindAnyObjectByType<CookieHolderView>() != null;
        }

        public void Install()
        {
            CookieStackView stacks = FindStacks();
            var table = (CookieTable)new SerializedObject(stacks).FindProperty("table").objectReferenceValue;
            int cookieOrder = new SerializedObject(stacks).FindProperty("sortingOrder").intValue;

            var group = stacks.GetComponent<SortingGroup>();
            if (group == null)
            {
                group = stacks.gameObject.AddComponent<SortingGroup>();
            }

            group.sortingOrder = cookieOrder;

            var holderObject = CreateChild(stacks.transform, "CookieHolder", Vector3.zero);
            SpriteRenderer back = CreateBoard(holderObject.transform, "Back", BackSpriteName, cookieOrder - 1);
            SpriteRenderer front = CreateBoard(holderObject.transform, "Front", FrontSpriteName, cookieOrder + 100);
            TextMeshPro label = CreateCountLabel(front.transform, table);

            var holder = holderObject.AddComponent<CookieHolderView>();
            SetReference(holder, "stacks", stacks);
            SetReference(holder, "back", back);
            SetReference(holder, "front", front);
            SetRendererArray(holder, "onFront", label.GetComponent<Renderer>());
            holder.Fit();
        }

        private static SpriteRenderer CreateBoard(Transform parent, string name, string spriteName, int sortingOrder)
        {
            SpriteImportSetup.EnsureBorder(spriteName);
            var board = CreateChild(parent, name, Vector3.zero);
            SpriteRenderer spriteRenderer = AddSprite(board, LoadSprite(spriteName), sortingOrder);
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = spriteRenderer.sprite.bounds.size;
            return spriteRenderer;
        }

        private static TextMeshPro CreateCountLabel(Transform front, CookieTable table)
        {
            var labelObject = CreateChild(front, "CountLabel", new Vector3(0f, FrontBoardHeight * 0.5f, 0f));
            var label = labelObject.AddComponent<TextMeshPro>();
            label.font = FontAssets.LoadOrCreateDisplayFont();
            label.rectTransform.sizeDelta = new Vector2(1.4f, FrontBoardHeight);
            label.text = $"0/{table.Capacity}";
            label.fontSize = 2.3f;
            label.color = LabelColor;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;

            var countLabel = labelObject.AddComponent<CookieCountLabel>();
            SetReference(countLabel, "table", table);
            SetReference(countLabel, "label", label);
            return label;
        }

        private static void SetRendererArray(Object target, string field, params Renderer[] renderers)
        {
            var serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(field);
            property.arraySize = renderers.Length;
            for (int i = 0; i < renderers.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static CookieStackView FindStacks()
        {
            var stacks = Object.FindAnyObjectByType<CookieStackView>();
            if (stacks == null)
            {
                throw new InvalidOperationException("The bakery scene has no CookieStackView.");
            }

            return stacks;
        }
    }
}
