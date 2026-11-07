using CookieClicker.Common;
using CookieClicker.Economy;
using CookieClicker.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CookieClicker.EditorTools.BuildHelpers;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Builds the HUD canvas: the money counter at the top and the layer flying money moves on.
    /// </summary>
    internal static class HudBuilder
    {
        private static readonly Vector2 ReferenceResolution = new Vector2(1080f, 1920f);

        public static void Build(Camera worldCamera, Wallet wallet, CashRegister cashRegister)
        {
            Canvas canvas = CreateCanvas(worldCamera);
            MoneyCounterView counter = CreateMoneyCounter(canvas.transform, wallet);
            CreatePopupLayer(canvas.transform, worldCamera, cashRegister, counter);
        }

        private static Canvas CreateCanvas(Camera worldCamera)
        {
            var canvasObject = new GameObject("HUD", typeof(RectTransform));
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = worldCamera;
            canvas.planeDistance = 5f;
            canvas.sortingOrder = SortingOrder.Hud;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static MoneyCounterView CreateMoneyCounter(Transform canvas, Wallet wallet)
        {
            RectTransform panel = CreateUiChild(canvas, "MoneyCounter", new Vector2(420f, 130f));
            panel.anchorMin = new Vector2(0.5f, 1f);
            panel.anchorMax = new Vector2(0.5f, 1f);
            panel.pivot = new Vector2(0.5f, 1f);
            panel.anchoredPosition = new Vector2(0f, -60f);
            AddImage(panel, "hud_pill");

            RectTransform icon = CreateUiChild(panel, "MoneyIcon", new Vector2(124f, 80f));
            icon.anchoredPosition = new Vector2(-120f, 4f);
            AddImage(icon, "money");
            var iconPop = icon.gameObject.AddComponent<PopAnimation>();
            SetFloat(iconPop, "strength", 0.3f);

            RectTransform labelRect = CreateUiChild(panel, "AmountLabel", new Vector2(240f, 100f));
            labelRect.anchoredPosition = new Vector2(70f, 4f);
            var label = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = "0";
            label.fontSize = 68f;
            label.fontStyle = FontStyles.Bold;
            label.color = Palette.Ink;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;

            var counter = panel.gameObject.AddComponent<MoneyCounterView>();
            SetReference(counter, "wallet", wallet);
            SetReference(counter, "amountLabel", label);
            SetReference(counter, "icon", icon);
            SetReference(counter, "iconPop", iconPop);
            return counter;
        }

        private static void CreatePopupLayer(Transform canvas, Camera worldCamera, CashRegister cashRegister, MoneyCounterView counter)
        {
            RectTransform layer = CreateUiChild(canvas, "MoneyPopups", Vector2.zero);
            layer.anchorMin = Vector2.zero;
            layer.anchorMax = Vector2.one;
            layer.SetAsFirstSibling();

            var spawner = layer.gameObject.AddComponent<MoneyPopupSpawner>();
            SetReference(spawner, "cashRegister", cashRegister);
            SetReference(spawner, "counter", counter);
            SetReference(spawner, "popupPrefab", AssetDatabase.LoadAssetAtPath<MoneyPopup>(PrefabBuilder.MoneyPopupPrefabPath));
            SetReference(spawner, "container", layer);
            SetReference(spawner, "worldCamera", worldCamera);
        }

        private static void AddImage(RectTransform target, string spriteName)
        {
            var image = target.gameObject.AddComponent<Image>();
            image.sprite = LoadSprite(spriteName);
            image.raycastTarget = false;
        }
    }
}
