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
        public const string UpgradeButtonPrefabPath = PrefabsFolder + "/UpgradeButton.prefab";

        private const string OutlinedFontMaterialPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat";

        public static readonly string[] CustomerLooks = { "customer_mint", "customer_berry", "customer_sunny", "customer_sky" };

        public static void BuildAll()
        {
            EnsureFolder(PrefabsFolder);
            BuildCustomer();
            BuildMoneyPopup();
            BuildUpgradeButton();
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

            if (!Exists(UpgradeButtonPrefabPath))
            {
                BuildUpgradeButton();
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

        private static void BuildUpgradeButton()
        {
            var root = new GameObject("UpgradeButton", typeof(RectTransform));
            ((RectTransform)root.transform).sizeDelta = new Vector2(240f, 320f);
            var card = root.AddComponent<Image>();
            card.sprite = LoadSprite("upgrade_card");
            var button = root.AddComponent<Button>();
            button.targetGraphic = card;
            ColorBlock colors = button.colors;
            colors.pressedColor = new Color(0.9f, 0.85f, 0.8f);
            colors.disabledColor = new Color(0.85f, 0.82f, 0.78f);
            button.colors = colors;

            TMP_Text nameLabel = CreateLabel(root.transform, "NameLabel", new Vector2(0f, -44f), new Vector2(220f, 44f), 28f, Palette.Ink);

            RectTransform icon = CreateTopAnchored(root.transform, "Icon", new Vector2(0f, -122f), new Vector2(112f, 112f));
            var iconImage = icon.gameObject.AddComponent<Image>();
            iconImage.sprite = LoadSprite("icon_bake_speed");
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            TMP_Text levelLabel = CreateLabel(root.transform, "LevelLabel", new Vector2(0f, -196f), new Vector2(220f, 34f), 24f, Palette.Caramel);
            TMP_Text valueLabel = CreateLabel(root.transform, "ValueLabel", new Vector2(0f, -230f), new Vector2(220f, 40f), 34f, Palette.Ink);

            RectTransform costPill = CreateTopAnchored(root.transform, "Cost", new Vector2(0f, -280f), new Vector2(200f, 70f));
            var costImage = costPill.gameObject.AddComponent<Image>();
            costImage.sprite = LoadSprite("upgrade_cost");
            costImage.raycastTarget = false;
            var costGroup = costPill.gameObject.AddComponent<CanvasGroup>();
            costGroup.blocksRaycasts = false;
            TMP_Text costLabel = CreateLabel(costPill, "CostLabel", new Vector2(0f, -32f), new Vector2(190f, 50f), 32f, Palette.MoneyInk);

            var pop = root.AddComponent<PopAnimation>();
            SetFloat(pop, "strength", 0.08f);

            var view = root.AddComponent<UpgradeButtonView>();
            SetReference(view, "button", button);
            SetReference(view, "icon", iconImage);
            SetReference(view, "nameLabel", nameLabel);
            SetReference(view, "levelLabel", levelLabel);
            SetReference(view, "valueLabel", valueLabel);
            SetReference(view, "costLabel", costLabel);
            SetReference(view, "costGroup", costGroup);
            SetReference(view, "pop", pop);

            SaveAndDestroy(root, UpgradeButtonPrefabPath);
        }

        private static RectTransform CreateTopAnchored(Transform parent, string name, Vector2 position, Vector2 size)
        {
            RectTransform rect = CreateUiChild(parent, name, size);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.anchoredPosition = position;
            return rect;
        }

        private static TMP_Text CreateLabel(Transform parent, string name, Vector2 position, Vector2 size, float fontSize, Color color)
        {
            RectTransform rect = CreateTopAnchored(parent, name, position, size);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.fontSize = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.color = color;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            return label;
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
