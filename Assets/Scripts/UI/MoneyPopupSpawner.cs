using CookieClicker.Economy;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// Spawns a <see cref="MoneyPopup"/> on the canvas, at the screen position of each sale.
    /// </summary>
    public class MoneyPopupSpawner : MonoBehaviour
    {
        [SerializeField] private CashRegister cashRegister;
        [SerializeField] private MoneyCounterView counter;
        [SerializeField] private MoneyPopup popupPrefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private float spawnJitter = 30f;

        private void OnEnable()
        {
            cashRegister.Earned += Spawn;
        }

        private void OnDisable()
        {
            cashRegister.Earned -= Spawn;
        }

        private void Spawn(long amount, Vector3 worldPosition)
        {
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);
            screenPoint += Random.insideUnitCircle * spawnJitter;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(container, screenPoint, worldCamera, out Vector3 spawnPosition);

            MoneyPopup popup = Instantiate(popupPrefab, container);
            popup.transform.position = spawnPosition;
            popup.Play(amount, counter.Icon, counter.PlayCollect);
        }
    }
}
