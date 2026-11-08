using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// Shrinks a full-screen RectTransform to the device's safe area so UI stays clear of notches and rounded corners.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect appliedSafeArea;
        private Vector2Int appliedScreenSize;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Update()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (Screen.safeArea == appliedSafeArea && screenSize == appliedScreenSize)
            {
                return;
            }

            Apply(Screen.safeArea, screenSize);
        }

        private void Apply(Rect safeArea, Vector2Int screenSize)
        {
            appliedSafeArea = safeArea;
            appliedScreenSize = screenSize;
            if (screenSize.x <= 0 || screenSize.y <= 0)
            {
                return;
            }

            rectTransform.anchorMin = new Vector2(safeArea.xMin / screenSize.x, safeArea.yMin / screenSize.y);
            rectTransform.anchorMax = new Vector2(safeArea.xMax / screenSize.x, safeArea.yMax / screenSize.y);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
