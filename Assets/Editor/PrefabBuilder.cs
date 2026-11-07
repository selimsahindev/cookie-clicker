using CookieClicker.Common;
using CookieClicker.Customers;
using CookieClicker.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CookieClicker.EditorTools.BuildHelpers;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Builds the prefabs that are spawned at runtime: customers and flying money.
    /// </summary>
    internal static class PrefabBuilder
    {
        public const string CustomerPrefabPath = PrefabsFolder + "/Customer.prefab";
        public const string MoneyPopupPrefabPath = PrefabsFolder + "/MoneyPopup.prefab";

        private const string OutlinedFontMaterialPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat";

        public static readonly string[] CustomerLooks = { "customer_mint", "customer_berry", "customer_sunny", "customer_sky" };

        public static void BuildAll()
        {
            EnsureFolder(PrefabsFolder);
            BuildCustomer();
            BuildMoneyPopup();
        }

        /// <summary>Builds only the prefabs that do not exist yet, so manual edits are kept.</summary>
        public static void BuildMissing()
        {
            EnsureFolder(PrefabsFolder);
            if (!Exists(CustomerPrefabPath))
            {
                BuildCustomer();
            }

            if (!Exists(MoneyPopupPrefabPath))
            {
                BuildMoneyPopup();
            }
        }

        private static bool Exists(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) != null;
        }

        private static void BuildCustomer()
        {
            var root = new GameObject("Customer");
            var visual = CreateChild(root.transform, "Visual", Vector3.zero);
            var body = AddSprite(visual, LoadSprite(CustomerLooks[0]), SortingOrder.Customer);
            var shadow = CreateChild(root.transform, "Shadow", new Vector3(0f, 0.05f, 0f));
            shadow.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            AddSprite(shadow, LoadSprite("shadow"), SortingOrder.CustomerShadow);

            SpeechBubble bubble = BuildSpeechBubble(root.transform);

            var walker = root.AddComponent<CharacterWalker>();
            SetReference(walker, "visual", visual.transform);

            var customer = root.AddComponent<Customer>();
            SetReference(customer, "body", body);
            SetReference(customer, "bubble", bubble);

            SaveAndDestroy(root, CustomerPrefabPath);
        }

        private static SpeechBubble BuildSpeechBubble(Transform parent)
        {
            var bubbleObject = CreateChild(parent, "SpeechBubble", new Vector3(0.15f, 2.55f, 0f));
            bubbleObject.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            AddSprite(bubbleObject, LoadSprite("bubble"), SortingOrder.Bubble);

            var icon = CreateChild(bubbleObject.transform, "CookieIcon", new Vector3(-0.45f, 0.62f, 0f));
            icon.transform.localScale = new Vector3(0.65f, 0.65f, 1f);
            AddSprite(icon, LoadSprite("cookie"), SortingOrder.Bubble + 1);

            var labelObject = CreateChild(bubbleObject.transform, "CountLabel", new Vector3(0.3f, 0.62f, 0f));
            var label = labelObject.AddComponent<TextMeshPro>();
            label.rectTransform.sizeDelta = new Vector2(1.1f, 0.7f);
            label.text = "0/5";
            label.fontSize = 5.5f;
            label.fontStyle = FontStyles.Bold;
            label.color = Palette.Ink;
            label.alignment = TextAlignmentOptions.Center;
            label.sortingOrder = SortingOrder.Bubble + 2;

            var pop = bubbleObject.AddComponent<PopAnimation>();
            var bubble = bubbleObject.AddComponent<SpeechBubble>();
            SetReference(bubble, "countLabel", label);
            SetReference(bubble, "pop", pop);
            return bubble;
        }

        private static void BuildMoneyPopup()
        {
            var root = new GameObject("MoneyPopup", typeof(RectTransform));
            ((RectTransform)root.transform).sizeDelta = new Vector2(260f, 90f);
            var canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            RectTransform bill = CreateUiChild(root.transform, "Bill", new Vector2(110f, 71f));
            bill.anchoredPosition = new Vector2(-70f, 0f);
            var billImage = bill.gameObject.AddComponent<Image>();
            billImage.sprite = LoadSprite("money");
            billImage.raycastTarget = false;

            RectTransform labelRect = CreateUiChild(root.transform, "AmountLabel", new Vector2(150f, 80f));
            labelRect.anchoredPosition = new Vector2(60f, 0f);
            var label = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = "+1 $";
            label.fontSize = 54f;
            label.fontStyle = FontStyles.Bold;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            ApplyOutlinedFont(label);

            var popup = root.AddComponent<MoneyPopup>();
            SetReference(popup, "amountLabel", label);
            SetReference(popup, "canvasGroup", canvasGroup);

            SaveAndDestroy(root, MoneyPopupPrefabPath);
        }

        private static void ApplyOutlinedFont(TMP_Text label)
        {
            var outlined = AssetDatabase.LoadAssetAtPath<Material>(OutlinedFontMaterialPath);
            if (outlined != null)
            {
                label.fontSharedMaterial = outlined;
            }
        }

        private static void SaveAndDestroy(GameObject root, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
    }
}
