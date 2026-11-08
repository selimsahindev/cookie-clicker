using System;
using CookieClicker.Baking;
using CookieClicker.Customers;
using CookieClicker.Economy;
using CookieClicker.UI;
using CookieClicker.Upgrades;
using CookieClicker.Upgrades.Effects;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// The upgrade shop and the bottom upgrade bar.
    /// </summary>
    internal class UpgradeFeature : ISceneFeature
    {
        private const string DefinitionsFolder = "Assets/Data/Upgrades";
        private const float BarHeight = 320f;
        private const float BarBottomMargin = 36f;

        public string Name => "Upgrades";

        public bool IsInstalled()
        {
            return Object.FindAnyObjectByType<UpgradeShop>() != null;
        }

        public void Install()
        {
            var baker = FindRequired<Baker>();
            var wallet = FindRequired<Wallet>();
            var cashRegister = FindRequired<CashRegister>();
            var spawner = FindRequired<CustomerSpawner>();
            Canvas hud = FindRequired<MoneyCounterView>().GetComponentInParent<Canvas>();

            var root = new GameObject("Upgrades");
            var shop = root.AddComponent<UpgradeShop>();
            SetReference(shop, "wallet", wallet);

            Upgrade[] upgrades =
            {
                CreateUpgrade<BakeSpeedEffect>(root.transform, BakeSpeedDefinition(), effect => SetReference(effect, "baker", baker)),
                CreateUpgrade<CookiePriceEffect>(root.transform, CookiePriceDefinition(), effect => SetReference(effect, "cashRegister", cashRegister)),
                CreateUpgrade<TapPowerEffect>(root.transform, TapPowerDefinition(), effect => SetReference(effect, "baker", baker)),
                CreateUpgrade<CustomerAttractionEffect>(root.transform, CustomerAttractionDefinition(), effect => SetReference(effect, "spawner", spawner)),
            };

            EnsureEventSystem();
            EnsureRaycaster(hud);
            CreateUpgradeBar(hud.transform, shop, upgrades);
        }

        private static Upgrade CreateUpgrade<TEffect>(Transform parent, UpgradeDefinition definition, Action<TEffect> wireEffect)
            where TEffect : UpgradeEffect
        {
            var upgradeObject = CreateChild(parent, definition.name, Vector3.zero);
            var effect = upgradeObject.AddComponent<TEffect>();
            wireEffect(effect);

            var upgrade = upgradeObject.AddComponent<Upgrade>();
            SetReference(upgrade, "definition", definition);
            SetReference(upgrade, "effect", effect);
            return upgrade;
        }

        private static UpgradeDefinition BakeSpeedDefinition()
        {
            return LoadOrCreateDefinition("BakeSpeed", definition => definition.Configure(
                "Bake Speed", LoadSprite("icon_bake_speed"), 10, 1.5f, UpgradeDefinition.ValueGrowth.Multiply, 0.9f, "{0:0.00}s"));
        }

        private static UpgradeDefinition CookiePriceDefinition()
        {
            return LoadOrCreateDefinition("CookiePrice", definition => definition.Configure(
                "Cookie Price", LoadSprite("icon_price"), 25, 1f, UpgradeDefinition.ValueGrowth.Add, 1f, "{0:0} $"));
        }

        private static UpgradeDefinition TapPowerDefinition()
        {
            return LoadOrCreateDefinition("TapPower", definition => definition.Configure(
                "Tap Power", LoadSprite("icon_tap"), 15, 0.2f, UpgradeDefinition.ValueGrowth.Add, 0.05f, "+{0:0%}", 17));
        }

        private static UpgradeDefinition CustomerAttractionDefinition()
        {
            return LoadOrCreateDefinition("Customers", definition => definition.Configure(
                "Customers", LoadSprite("icon_customers"), 20, 2.5f, UpgradeDefinition.ValueGrowth.Multiply, 0.9f, "{0:0.0}s"));
        }

        /// <summary>Existing definitions are reused as they are, so balancing done in the Inspector is kept.</summary>
        private static UpgradeDefinition LoadOrCreateDefinition(string assetName, Action<UpgradeDefinition> configure)
        {
            string path = $"{DefinitionsFolder}/{assetName}.asset";
            var definition = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(path);
            if (definition != null)
            {
                return definition;
            }

            EnsureFolder(DefinitionsFolder);
            definition = ScriptableObject.CreateInstance<UpgradeDefinition>();
            configure(definition);
            AssetDatabase.CreateAsset(definition, path);
            return definition;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static void EnsureRaycaster(Canvas canvas)
        {
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private static void CreateUpgradeBar(Transform canvas, UpgradeShop shop, Upgrade[] upgrades)
        {
            RectTransform bar = CreateUiChild(canvas, "UpgradeBar", new Vector2(0f, BarHeight));
            bar.anchorMin = new Vector2(0f, 0f);
            bar.anchorMax = new Vector2(1f, 0f);
            bar.pivot = new Vector2(0.5f, 0f);
            bar.anchoredPosition = new Vector2(0f, BarBottomMargin);

            var layout = bar.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 0, 0);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabBuilder.UpgradeButtonPrefabPath);
            foreach (Upgrade upgrade in upgrades)
            {
                var buttonObject = (GameObject)PrefabUtility.InstantiatePrefab(buttonPrefab, bar);
                buttonObject.name = $"{upgrade.Definition.DisplayName} Button";
                var button = buttonObject.GetComponent<UpgradeButtonView>();
                SetReference(button, "upgrade", upgrade);
                SetReference(button, "shop", shop);
            }
        }

        private static T FindRequired<T>() where T : Object
        {
            T found = Object.FindAnyObjectByType<T>();
            if (found == null)
            {
                throw new InvalidOperationException($"The bakery scene has no {typeof(T).Name}.");
            }

            return found;
        }
    }
}
