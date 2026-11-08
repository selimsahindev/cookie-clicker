using System;
using CookieClicker.Baking;
using UnityEngine;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Adds a glow layer to the existing oven and the <see cref="OvenView"/> that animates it.
    /// </summary>
    internal class OvenAnimationFeature : ISceneFeature
    {
        private const string OvenSpriteName = "oven";
        private const string GlowSpriteName = "oven_glow";

        public string Name => "Oven animation";

        public bool IsInstalled()
        {
            return Object.FindAnyObjectByType<OvenView>() != null;
        }

        public void Install()
        {
            SpriteRenderer oven = FindOven();
            var baker = Object.FindAnyObjectByType<Baker>();
            if (baker == null)
            {
                throw new InvalidOperationException("The bakery scene has no Baker.");
            }

            var glowObject = CreateChild(oven.transform, "Glow", Vector3.zero);
            SpriteRenderer glow = AddSprite(glowObject, LoadSprite(GlowSpriteName), oven.sortingOrder + 1);
            glow.sortingLayerID = oven.sortingLayerID;
            glow.color = new Color(1f, 1f, 1f, 0f);

            var view = oven.gameObject.AddComponent<OvenView>();
            SetReference(view, "baker", baker);
            SetReference(view, "glow", glow);
        }

        private static SpriteRenderer FindOven()
        {
            foreach (SpriteRenderer spriteRenderer in Object.FindObjectsByType<SpriteRenderer>())
            {
                if (spriteRenderer.sprite != null && spriteRenderer.sprite.name == OvenSpriteName)
                {
                    return spriteRenderer;
                }
            }

            throw new InvalidOperationException("No oven sprite found in the bakery scene.");
        }
    }
}
