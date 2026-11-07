using UnityEngine;
using UnityEngine.EventSystems;

namespace CookieClicker.Baking
{
    /// <summary>
    /// Turns a tap or click anywhere on the game area into a bake boost.
    /// Taps on UI (future upgrade buttons) are ignored.
    /// </summary>
    public class TapToBake : MonoBehaviour
    {
        [SerializeField] private Baker baker;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || IsPointerOverUi())
            {
                return;
            }

            baker.Boost();
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
