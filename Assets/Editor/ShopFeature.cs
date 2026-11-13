using CookieClicker.Common;
using CookieClicker.Economy;
using CookieClicker.Shopping;
using CookieClicker.Shopping.Effects;
using CookieClicker.Table;
using CookieClicker.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// The shop: a round button on the HUD that opens a window with a "Stand" tab for one-time unlocks
    /// (first one: a bigger cookie holder) and a "Staff" tab with hireable characters (one locked sample for now).
    /// </summary>
    internal class ShopFeature : ISceneFeature
    {
        public const string RowPrefabPath = PrefabsFolder + "/ShopRow.prefab";

        private const string UnlocksFolder = "Assets/Data/Shop";
        private const string StaffFolder = "Assets/Data/Staff";
        private const string SafeAreaName = "TopSafeArea";
        private const float RowHeight = 210f;

        private static readonly Color Backdrop = new Color32(0x3B, 0x24, 0x19, 0x8C);
        private static readonly Color DetailInk = new Color32(0x9A, 0x62, 0x3E, 0xFF);

        public string Name => "Shop";

        public bool IsInstalled()
        {
            return Object.FindAnyObjectByType<ShopPanelView>() != null;
        }

        public void Install()
        {
            var wallet = FindRequired<Wallet>();
            var table = FindRequired<CookieTable>();
            Canvas hud = FindRequired<MoneyCounterView>().GetComponentInParent<Canvas>();
            EnsureEventSystem();
            EnsureRaycaster(hud);

            var shopObject = new GameObject("Shop");
            var shop = shopObject.AddComponent<Shop>();
            SetReference(shop, "wallet", wallet);
            Unlockable biggerHolder = CreateBiggerHolder(shopObject.transform, table);

            ShopRowView rowPrefab = LoadOrBuildRowPrefab();
            Button openButton = CreateShopButton(FindTopArea(hud.transform));
            CreatePanel(hud.transform, openButton, rowPrefab, shop, biggerHolder);
        }

        private static Unlockable CreateBiggerHolder(Transform parent, CookieTable table)
        {
            UnlockDefinition definition = LoadOrCreateAsset<UnlockDefinition>(UnlocksFolder, "BiggerHolder", created => created.Configure(
                "Bigger Holder", LoadSprite("icon_holder_big"), 250, "Holds 15 cookies"));

            var unlockObject = CreateChild(parent, definition.name, Vector3.zero);
            var effect = unlockObject.AddComponent<HolderCapacityEffect>();
            SetReference(effect, "table", table);
            SetInt(effect, "capacity", 15);

            var unlockable = unlockObject.AddComponent<Unlockable>();
            SetReference(unlockable, "definition", definition);
            SetReference(unlockable, "effect", effect);
            return unlockable;
        }

        private static EmployeeDefinition SampleEmployee()
        {
            return LoadOrCreateAsset<EmployeeDefinition>(StaffFolder, "Mia", created => created.Configure(
                "Mia", LoadSprite("staff_mia"), 500, "Helper baker", 3, EmployeeDefinition.WageUnit.Minute));
        }

        // ---------- HUD button ----------

        private static Transform FindTopArea(Transform canvas)
        {
            Transform safeArea = canvas.Find(SafeAreaName);
            return safeArea != null ? safeArea : canvas;
        }

        private static Button CreateShopButton(Transform parent)
        {
            RectTransform rect = CreateUiChild(parent, "ShopButton", new Vector2(170f, 170f));
            Pin(rect, new Vector2(1f, 1f), new Vector2(-36f, -230f));
            Image image = AddImage(rect, "shop_button", true);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            TMP_Text label = CreateText(rect, "Label", "Shop", 40f, Palette.Ink, TextAlignmentOptions.Center);
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0.5f, 0f);
            labelRect.anchorMax = new Vector2(0.5f, 0f);
            labelRect.sizeDelta = new Vector2(200f, 50f);
            labelRect.anchoredPosition = new Vector2(0f, -18f);
            return button;
        }

        // ---------- Window ----------

        private static void CreatePanel(Transform canvas, Button openButton, ShopRowView rowPrefab, Shop shop, Unlockable biggerHolder)
        {
            RectTransform panel = CreateUiChild(canvas, "ShopPanel", Vector2.zero);
            StretchFull(panel);
            panel.SetAsLastSibling();

            RectTransform content = CreateUiChild(panel, "Content", Vector2.zero);
            StretchFull(content);

            RectTransform backdropRect = CreateUiChild(content, "Backdrop", Vector2.zero);
            StretchFull(backdropRect);
            var backdropImage = backdropRect.gameObject.AddComponent<Image>();
            backdropImage.color = Backdrop;
            var backdrop = backdropRect.gameObject.AddComponent<Button>();
            backdrop.transition = Selectable.Transition.None;

            RectTransform window = CreateUiChild(content, "Window", Vector2.zero);
            window.anchorMin = Vector2.zero;
            window.anchorMax = Vector2.one;
            window.offsetMin = new Vector2(48f, 220f);
            window.offsetMax = new Vector2(-48f, -220f);
            AddSlicedImage(window, "shop_window", true);
            var windowPop = window.gameObject.AddComponent<PopAnimation>();
            SetFloat(windowPop, "strength", 0.05f);

            TMP_Text title = CreateText(window, "Title", "Shop", 76f, Palette.Ink, TextAlignmentOptions.MidlineLeft);
            title.rectTransform.sizeDelta = new Vector2(400f, 100f);
            Pin(title.rectTransform, new Vector2(0f, 1f), new Vector2(60f, -36f));

            RectTransform closeRect = CreateUiChild(window, "CloseButton", new Vector2(110f, 110f));
            Pin(closeRect, new Vector2(1f, 1f), new Vector2(-28f, -28f));
            var closeButton = closeRect.gameObject.AddComponent<Button>();
            closeButton.targetGraphic = AddImage(closeRect, "shop_close", true);

            RectTransform tabRow = CreateTabRow(window);
            RectTransform pages = CreateUiChild(window, "Pages", Vector2.zero);
            pages.anchorMin = Vector2.zero;
            pages.anchorMax = Vector2.one;
            pages.offsetMin = new Vector2(44f, 48f);
            pages.offsetMax = new Vector2(-44f, -296f);

            RectTransform standPage = CreatePage(pages, "StandPage");
            AddUnlockRow(standPage, rowPrefab, shop, biggerHolder);
            RectTransform staffPage = CreatePage(pages, "StaffPage");
            AddEmployeeRow(staffPage, rowPrefab, SampleEmployee());

            var view = panel.gameObject.AddComponent<ShopPanelView>();
            SetReference(view, "openButton", openButton);
            SetReference(view, "closeButton", closeButton);
            SetReference(view, "backdrop", backdrop);
            SetReference(view, "content", content.gameObject);
            SetReference(view, "windowPop", windowPop);
            SetTabs(view,
                CreateTab(tabRow, "StandTab", "Stand", "icon_tab_stand", standPage),
                CreateTab(tabRow, "StaffTab", "Staff", "icon_tab_staff", staffPage));

            content.gameObject.SetActive(false);
        }

        private static RectTransform CreateTabRow(Transform window)
        {
            RectTransform row = CreateUiChild(window, "Tabs", new Vector2(0f, 110f));
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = new Vector2(1f, 1f);
            row.pivot = new Vector2(0.5f, 1f);
            row.anchoredPosition = new Vector2(0f, -160f);

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(44, 44, 0, 0);
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            return row;
        }

        private struct TabParts
        {
            public Button Button;
            public Image Background;
            public GameObject Page;
        }

        private static TabParts CreateTab(Transform row, string name, string text, string iconName, RectTransform page)
        {
            RectTransform tab = CreateUiChild(row, name, Vector2.zero);
            tab.gameObject.AddComponent<LayoutElement>().preferredWidth = 300f;
            Image background = AddSlicedImage(tab, "shop_tab", true);
            var button = tab.gameObject.AddComponent<Button>();
            button.targetGraphic = background;

            RectTransform icon = CreateUiChild(tab, "Icon", new Vector2(80f, 80f));
            Pin(icon, new Vector2(0f, 0.5f), new Vector2(22f, 2f));
            AddImage(icon, iconName, false).preserveAspect = true;

            TMP_Text label = CreateText(tab, "Label", text, 46f, Palette.Ink, TextAlignmentOptions.MidlineLeft);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(114f, 0f);
            label.rectTransform.offsetMax = new Vector2(-12f, 0f);

            return new TabParts { Button = button, Background = background, Page = page.gameObject };
        }

        private static void SetTabs(ShopPanelView view, params TabParts[] tabs)
        {
            var serialized = new SerializedObject(view);
            SerializedProperty property = serialized.FindProperty("tabs");
            property.arraySize = tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("button").objectReferenceValue = tabs[i].Button;
                element.FindPropertyRelative("background").objectReferenceValue = tabs[i].Background;
                element.FindPropertyRelative("page").objectReferenceValue = tabs[i].Page;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static RectTransform CreatePage(Transform pages, string name)
        {
            RectTransform page = CreateUiChild(pages, name, Vector2.zero);
            StretchFull(page);
            var layout = page.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return page;
        }

        private static void AddUnlockRow(Transform page, ShopRowView rowPrefab, Shop shop, Unlockable unlockable)
        {
            ShopRowView row = InstantiateRow(page, rowPrefab, unlockable.Definition.DisplayName);
            var offer = row.gameObject.AddComponent<UnlockOfferView>();
            SetReference(offer, "unlockable", unlockable);
            SetReference(offer, "shop", shop);
            SetReference(offer, "row", row);
            SetReference(offer, "pop", row.GetComponent<PopAnimation>());
        }

        private static void AddEmployeeRow(Transform page, ShopRowView rowPrefab, EmployeeDefinition employee)
        {
            ShopRowView row = InstantiateRow(page, rowPrefab, employee.DisplayName);
            var offer = row.gameObject.AddComponent<EmployeeOfferView>();
            SetReference(offer, "employee", employee);
            SetReference(offer, "row", row);
        }

        private static ShopRowView InstantiateRow(Transform page, ShopRowView rowPrefab, string itemName)
        {
            var rowObject = (GameObject)PrefabUtility.InstantiatePrefab(rowPrefab.gameObject, page);
            rowObject.name = $"{itemName} Row";
            return rowObject.GetComponent<ShopRowView>();
        }

        // ---------- Row prefab ----------

        /// <summary>One prefab for every shop card, so their look can be changed in one place.</summary>
        private static ShopRowView LoadOrBuildRowPrefab()
        {
            var existing = AssetDatabase.LoadAssetAtPath<ShopRowView>(RowPrefabPath);
            if (existing != null)
            {
                return existing;
            }

            var root = new GameObject("ShopRow", typeof(RectTransform));
            var rect = (RectTransform)root.transform;
            rect.sizeDelta = new Vector2(900f, RowHeight);
            root.AddComponent<LayoutElement>().preferredHeight = RowHeight;
            AddSlicedImage(rect, "shop_row", false);

            RectTransform icon = CreateUiChild(rect, "Icon", new Vector2(170f, 150f));
            Pin(icon, new Vector2(0f, 0.5f), new Vector2(26f, 0f));
            Image iconImage = AddImage(icon, "icon_holder_big", false);
            iconImage.preserveAspect = true;

            TMP_Text title = CreateText(rect, "Title", "Item", 50f, Palette.Ink, TextAlignmentOptions.MidlineLeft);
            PlaceTextLine(title.rectTransform, 34f, 64f);
            TMP_Text detail = CreateText(rect, "Detail", "Detail", 32f, DetailInk, TextAlignmentOptions.MidlineLeft);
            PlaceTextLine(detail.rectTransform, -30f, 48f);
            detail.overflowMode = TextOverflowModes.Ellipsis;

            RectTransform priceRect = CreateUiChild(rect, "PriceButton", new Vector2(240f, 84f));
            Pin(priceRect, new Vector2(1f, 0.5f), new Vector2(-28f, 0f));
            var priceButton = priceRect.gameObject.AddComponent<Button>();
            priceButton.targetGraphic = AddImage(priceRect, "upgrade_cost", true);
            ColorBlock colors = priceButton.colors;
            colors.pressedColor = new Color(0.9f, 0.85f, 0.8f);
            colors.disabledColor = new Color(0.85f, 0.82f, 0.78f);
            priceButton.colors = colors;
            var priceGroup = priceRect.gameObject.AddComponent<CanvasGroup>();

            RectTransform lockRect = CreateUiChild(priceRect, "Lock", new Vector2(40f, 46f));
            Pin(lockRect, new Vector2(0f, 0.5f), new Vector2(24f, 2f));
            AddImage(lockRect, "icon_lock", false).preserveAspect = true;
            lockRect.gameObject.SetActive(false);

            TMP_Text priceLabel = CreateText(priceRect, "PriceLabel", "0 $", 38f, Palette.MoneyInk, TextAlignmentOptions.Center);
            priceLabel.rectTransform.anchorMin = Vector2.zero;
            priceLabel.rectTransform.anchorMax = Vector2.one;
            priceLabel.rectTransform.offsetMin = new Vector2(0f, 4f);
            priceLabel.rectTransform.offsetMax = Vector2.zero;

            var pop = root.AddComponent<PopAnimation>();
            SetFloat(pop, "strength", 0.06f);

            var view = root.AddComponent<ShopRowView>();
            SetReference(view, "icon", iconImage);
            SetReference(view, "titleLabel", title);
            SetReference(view, "detailLabel", detail);
            SetReference(view, "button", priceButton);
            SetReference(view, "priceLabel", priceLabel);
            SetReference(view, "priceGroup", priceGroup);
            SetReference(view, "lockIcon", lockRect.gameObject);

            EnsureFolder(PrefabsFolder);
            PrefabUtility.SaveAsPrefabAsset(root, RowPrefabPath);
            Object.DestroyImmediate(root);
            return AssetDatabase.LoadAssetAtPath<ShopRowView>(RowPrefabPath);
        }

        /// <summary>A text line between the row's icon and its price button.</summary>
        private static void PlaceTextLine(RectTransform rect, float y, float height)
        {
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.offsetMin = new Vector2(220f, y - height * 0.5f);
            rect.offsetMax = new Vector2(-290f, y + height * 0.5f);
        }

        // ---------- Small UI helpers ----------

        /// <summary>Anchors and pivots a rect to one point of its parent (for example the top-right corner).</summary>
        private static void Pin(RectTransform rect, Vector2 anchor, Vector2 position)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Image AddImage(RectTransform target, string spriteName, bool raycastTarget)
        {
            var image = target.gameObject.AddComponent<Image>();
            image.sprite = LoadSprite(spriteName);
            image.raycastTarget = raycastTarget;
            return image;
        }

        private static Image AddSlicedImage(RectTransform target, string spriteName, bool raycastTarget)
        {
            SpriteImportSetup.EnsureBorder(spriteName);
            Image image = AddImage(target, spriteName, raycastTarget);
            image.type = Image.Type.Sliced;
            return image;
        }

        private static TMP_Text CreateText(Transform parent, string name, string text, float size, Color color, TextAlignmentOptions alignment)
        {
            RectTransform rect = CreateUiChild(parent, name, Vector2.zero);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = FontAssets.LoadOrCreateDisplayFont();
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            return label;
        }
    }
}
