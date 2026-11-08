using System;
using CookieClicker.Common;
using CookieClicker.Saving;
using CookieClicker.Table;
using UnityEngine;

namespace CookieClicker.Baking
{
    /// <summary>
    /// Bakes cookies nonstop and puts each one on the table. Taps speed up the current bake.
    /// Waits with a full ring while the table is full.
    /// </summary>
    public class Baker : MonoBehaviour, IProgressSource, ISaveable
    {
        [SerializeField] private CookieTable table;
        [SerializeField, Min(0.05f)] private float bakeDuration = 1.5f;
        [SerializeField, Range(0f, 1f)] private float clickBoost = 0.2f;

        private float progress;

        public event Action CookieBaked;
        public event Action Boosted;

        public float Progress => progress;
        public bool IsWaitingForSpace => progress >= 1f && !table.HasSpace;
        public bool IsBaking => !IsWaitingForSpace;

        /// <summary>Seconds per cookie. Upgrades can lower this at runtime.</summary>
        public float BakeDuration
        {
            get
            {
                return bakeDuration;
            }
            set
            {
                bakeDuration = Mathf.Max(0.05f, value);
            }
        }

        /// <summary>Share of a full bake added by one tap. Upgrades can raise this at runtime.</summary>
        public float ClickBoost
        {
            get
            {
                return clickBoost;
            }
            set
            {
                clickBoost = Mathf.Clamp01(value);
            }
        }

        /// <summary>Instantly advances the current bake by <see cref="ClickBoost"/>.</summary>
        public void Boost()
        {
            progress = Mathf.Min(1f, progress + clickBoost);
            Boosted?.Invoke();
        }

        public void Capture(SaveData data)
        {
            data.bakeProgress = progress;
        }

        public void Restore(SaveData data)
        {
            progress = Mathf.Clamp01(data.bakeProgress);
        }

        private void Update()
        {
            if (progress < 1f)
            {
                progress = Mathf.Min(1f, progress + Time.deltaTime / bakeDuration);
            }

            if (progress >= 1f && table.TryAddCookie())
            {
                progress = 0f;
                CookieBaked?.Invoke();
            }
        }
    }
}
