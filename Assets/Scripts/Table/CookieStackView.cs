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
        [SerializeField] private int cookiesPerStack = 5;
        [SerializeField] private float stackSpacing = 0.55f;
        [SerializeField] private float cookieHeight = 0.12f;
        [SerializeField] private float cookieScale = 0.55f;
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
                cookies[i].gameObject.SetActive(i < count);
            }
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
            int stack = index / cookiesPerStack;
            int level = index % cookiesPerStack;

            var cookie = new GameObject($"Cookie {index + 1}");
            cookie.transform.SetParent(transform, false);
            cookie.transform.localPosition = new Vector3(stack * stackSpacing, level * cookieHeight, 0f);
            cookie.transform.localScale = new Vector3(cookieScale, cookieScale * 0.6f, 1f);

            var spriteRenderer = cookie.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cookieSprite;
            spriteRenderer.sortingOrder = sortingOrder + index;
            return spriteRenderer;
        }
    }
}
