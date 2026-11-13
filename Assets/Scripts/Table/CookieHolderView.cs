using UnityEngine;

namespace CookieClicker.Table
{
    /// <summary>
    /// The wooden holder the cookie stacks stand in. Its back and front boards are sliced sprites
    /// that stretch to fit every stack, so a bigger capacity simply makes the holder wider.
    /// Lives under the stacks' SortingGroup, so its front board can sit above every cookie without covering customers.
    /// </summary>
    public class CookieHolderView : MonoBehaviour
    {
        [SerializeField] private CookieStackView stacks;
        [SerializeField] private SpriteRenderer back;
        [SerializeField] private SpriteRenderer front;
        [Tooltip("Things drawn on the front board, such as the count label. They stay above it in this order.")]
        [SerializeField] private Renderer[] onFront = new Renderer[0];
        [Tooltip("Extra room between the outer stacks and the holder's side walls.")]
        [SerializeField] private float sidePadding = 0.1f;
        [Tooltip("Local y of the holder's floor, just below the bottom cookies.")]
        [SerializeField] private float floorY = -0.28f;

        private void OnEnable()
        {
            stacks.LayoutChanged += Fit;
            Fit();
        }

        private void OnDisable()
        {
            stacks.LayoutChanged -= Fit;
        }

        /// <summary>Re-applies layout changes made in the Inspector while the game is running.</summary>
        private void OnValidate()
        {
            if (!Application.isPlaying || stacks == null)
            {
                return;
            }

            Fit();
        }

        /// <summary>Sizes and centers the holder around the stacks. The editor installer calls it once too.</summary>
        public void Fit()
        {
            Vector2 row = stacks.RowExtents;
            float width = row.y - row.x + sidePadding * 2f;
            transform.localPosition = new Vector3((row.x + row.y) * 0.5f, floorY, transform.localPosition.z);
            Stretch(back, width);
            Stretch(front, width);
            KeepFrontOverCookies();
        }

        private void KeepFrontOverCookies()
        {
            int order = stacks.TopSortingOrder + 1;
            front.sortingOrder = order;
            foreach (Renderer layer in onFront)
            {
                order++;
                layer.sortingOrder = order;
            }
        }

        private static void Stretch(SpriteRenderer board, float width)
        {
            board.size = new Vector2(width, board.size.y);
        }
    }
}
