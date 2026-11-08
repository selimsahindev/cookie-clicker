using System;
using CookieClicker.Common;
using CookieClicker.Economy;
using CookieClicker.Progression;
using CookieClicker.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// The level goal and its thin goal bar along the top of the screen, inside the safe area:
    /// a "Goal" title, a flat bar that fills green, and the percentage.
    /// </summary>
    internal class LevelGoalFeature : ISceneFeature
    {
        private const string BarName = "GoalBar";
        private const string SafeAreaName = "TopSafeArea";
        private const float RowHeight = 56f;
        private const float TopMargin = 28f;
        private const float SideMargin = 32f;
        private const float TitleWidth = 110f;
        private const float PercentWidth = 96f;
        private const float BarThickness = 18f;
        private const float BorderThickness = 3f;

        private static readonly Color Track = new Color32(0xFF, 0xF4, 0xE4, 0xFF);
        private static readonly Color Fill = new Color32(0x6D, 0xBE, 0x6A, 0xFF);

        public string Name => "Level goal bar";

        public bool IsInstalled()
        {
            LevelGoalView view = Object.FindAnyObjectByType<LevelGoalView>();
            return Object.FindAnyObjectByType<LevelGoal>() != null && view != null && view.name == BarName;
        }

        public void Install()
        {
            var counter = Object.FindAnyObjectByType<MoneyCounterView>();
            if (counter == null)
            {
                throw new InvalidOperationException("The bakery scene needs a MoneyCounterView first.");
            }

            LevelGoal goal = FindOrCreateGoal();
            Canvas hud = counter.GetComponentInParent<Canvas>();
            RemoveOldBar(hud.transform);
            CreateBar(FindOrCreateSafeArea(hud.transform), goal);
        }

        private static LevelGoal FindOrCreateGoal()
        {
            var goal = Object.FindAnyObjectByType<LevelGoal>();
            if (goal != null)
            {
                return goal;
            }

            var cashRegister = Object.FindAnyObjectByType<CashRegister>();
            if (cashRegister == null)
            {
                throw new InvalidOperationException("The bakery scene needs a CashRegister first.");
            }

            goal = new GameObject("LevelGoal").AddComponent<LevelGoal>();
            SetReference(goal, "cashRegister", cashRegister);
            return goal;
        }

        /// <summary>Replaces the first version of the bar, which used stretched sprites.</summary>
        private static void RemoveOldBar(Transform canvas)
        {
            foreach (LevelGoalView oldView in canvas.GetComponentsInChildren<LevelGoalView>(true))
            {
                Object.DestroyImmediate(oldView.gameObject);
            }
        }

        private static RectTransform FindOrCreateSafeArea(Transform canvas)
        {
            Transform existing = canvas.Find(SafeAreaName);
            if (existing != null)
            {
                return (RectTransform)existing;
            }

            RectTransform safeArea = CreateUiChild(canvas, SafeAreaName, Vector2.zero);
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.gameObject.AddComponent<SafeAreaFitter>();
            return safeArea;
        }

        private static void CreateBar(RectTransform parent, LevelGoal goal)
        {
            TMP_FontAsset font = FontAssets.LoadOrCreateDisplayFont();

            RectTransform row = CreateUiChild(parent, BarName, new Vector2(-2f * SideMargin, RowHeight));
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = new Vector2(1f, 1f);
            row.pivot = new Vector2(0.5f, 1f);
            row.anchoredPosition = new Vector2(0f, -TopMargin);

            TMP_Text title = CreateText(row, "Title", font, "Goal", 44f, TextAlignmentOptions.MidlineLeft);
            Stretch((RectTransform)title.transform, 0f, TitleWidth, 0f);
            var titlePop = title.gameObject.AddComponent<PopAnimation>();
            SetFloat(titlePop, "strength", 0.3f);

            TMP_Text percent = CreateText(row, "Percent", font, "0%", 32f, TextAlignmentOptions.MidlineRight);
            Stretch((RectTransform)percent.transform, 1f, PercentWidth, 0f);

            RectTransform border = CreateUiChild(row, "Bar", Vector2.zero);
            border.anchorMin = new Vector2(0f, 0.5f);
            border.anchorMax = new Vector2(1f, 0.5f);
            border.offsetMin = new Vector2(TitleWidth, -BarThickness * 0.5f);
            border.offsetMax = new Vector2(-PercentWidth - 12f, BarThickness * 0.5f);
            AddColor(border, Palette.Ink);

            RectTransform track = CreateUiChild(border, "Track", Vector2.zero);
            track.anchorMin = Vector2.zero;
            track.anchorMax = Vector2.one;
            track.offsetMin = new Vector2(BorderThickness, BorderThickness);
            track.offsetMax = new Vector2(-BorderThickness, -BorderThickness);
            AddColor(track, Track);

            RectTransform fill = CreateUiChild(track, "Fill", Vector2.zero);
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = new Vector2(0f, 1f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            AddColor(fill, Fill);

            var view = row.gameObject.AddComponent<LevelGoalView>();
            SetReference(view, "goal", goal);
            SetReference(view, "fill", fill);
            SetReference(view, "percentLabel", percent);
            SetReference(view, "titlePop", titlePop);
        }

        /// <summary>Pins a full-height child to the left (side 0) or right (side 1) edge with a fixed width.</summary>
        private static void Stretch(RectTransform rect, float side, float width, float inset)
        {
            rect.anchorMin = new Vector2(side, 0f);
            rect.anchorMax = new Vector2(side, 1f);
            rect.pivot = new Vector2(side, 0.5f);
            rect.sizeDelta = new Vector2(width, 0f);
            rect.anchoredPosition = new Vector2(side < 0.5f ? inset : -inset, 0f);
        }

        private static TMP_Text CreateText(Transform parent, string name, TMP_FontAsset font, string text, float size, TextAlignmentOptions alignment)
        {
            RectTransform rect = CreateUiChild(parent, name, Vector2.zero);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = font;
            label.text = text;
            label.fontSize = size;
            label.color = Palette.Ink;
            label.alignment = alignment;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            return label;
        }

        private static void AddColor(RectTransform target, Color color)
        {
            var image = target.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
        }
    }
}
