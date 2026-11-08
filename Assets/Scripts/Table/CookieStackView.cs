using System.Collections.Generic;
using UnityEngine;

namespace CookieClicker.Table
{
    /// <summary>
    /// Shows the table's cookies as little stacks laid out in columns on the tabletop.
    /// </summary>
    public class CookieStackView : MonoBehaviour
    {
        [SerializeField] private CookieTable table;
        [SerializeField] private Sprite cookieSprite;
        [SerializeField, Min(1)] private int cookiesPerStack = 5;
        [Tooltip("Horizontal distance between two stacks.")]
        [SerializeField] private float stackSpacing = 0.75f;
        [Tooltip("Vertical distance between two cookies in a stack.")]
        [SerializeField] private float cookieHeight = 0.17f;
        [SerializeField] private float cookieScale = 0.85f;
        [Tooltip("Height of a cookie relative to its width, so it reads as lying on the cart.")]
        [SerializeField, Range(0.2f, 1f)] private float cookieFlatness = 0.6f;
        [SerializeField] private int sortingOrder = 20;

        private readonly List<SpriteRenderer> cookies = new List<SpriteRenderer>();

        private void OnEnable()
        {
            table.Changed += Refresh;
            Refresh(table.Count, table.Capacity);
        }

        private void OnDisable()
        {
            table.Changed -= Refresh;
        }

        private void Refresh(int count, int capacity)
        {
            EnsureCookieCount(capacity);

            for (int i = 0; i < cookies.Count; i++)
            {
                Place(cookies[i].transform, i);
                cookies[i].gameObject.SetActive(i < count);
            }
        }

        /// <summary>Re-applies layout changes made in the Inspector while the game is running.</summary>
        private void OnValidate()
        {
            if (!Application.isPlaying || table == null)
            {
                return;
            }

            Refresh(table.Count, table.Capacity);
        }

        private void Place(Transform cookie, int index)
        {
            int stack = index / cookiesPerStack;
            int level = index % cookiesPerStack;
            cookie.localPosition = new Vector3(stack * stackSpacing, level * cookieHeight, 0f);
            cookie.localScale = new Vector3(cookieScale, cookieScale * cookieFlatness, 1f);
        }

        private void EnsureCookieCount(int amount)
        {
            while (cookies.Count < amount)
            {
                cookies.Add(CreateCookie(cookies.Count));
            }
        }

        private SpriteRenderer CreateCookie(int index)
        {
            var cookie = new GameObject($"Cookie {index + 1}");
            cookie.transform.SetParent(transform, false);

            var spriteRenderer = cookie.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cookieSprite;
            spriteRenderer.sortingOrder = sortingOrder + index;
            return spriteRenderer;
        }
    }
}
