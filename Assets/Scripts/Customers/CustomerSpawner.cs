using CookieClicker.Common;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// Sends new customers into the queue every few seconds while there is room.
    /// With a <see cref="Street"/> assigned, customers walk in along the street from either side,
    /// like passers-by who decide to stop. Without one they walk in from the spawn point.
    /// </summary>
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField] private Customer customerPrefab;
        [SerializeField] private CustomerQueue queue;
        [SerializeField] private Transform spawnPoint;
        [Tooltip("Optional. Customers walk in along this street from both sides instead of from the spawn point.")]
        [SerializeField] private Street street;
        [SerializeField] private Sprite[] looks;
        [SerializeField, Min(1)] private int minCookiesWanted = 1;
        [SerializeField, Min(1)] private int maxCookiesWanted = 5;
        [SerializeField] private Vector2 spawnIntervalRange = new Vector2(1.5f, 3.5f);

        private float timeUntilNextSpawn;

        /// <summary>Upgrades or difficulty can tune how often customers arrive.</summary>
        public Vector2 SpawnIntervalRange
        {
            get
            {
                return spawnIntervalRange;
            }
            set
            {
                spawnIntervalRange = value;
            }
        }

        private void Update()
        {
            timeUntilNextSpawn -= Time.deltaTime;
            if (timeUntilNextSpawn > 0f || !queue.HasRoom)
            {
                return;
            }

            Spawn();
            timeUntilNextSpawn = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
        }

        private void Spawn()
        {
            Customer customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity, transform);
            Sprite look = looks[Random.Range(0, looks.Length)];
            int cookiesWanted = Random.Range(minCookiesWanted, maxCookiesWanted + 1);
            customer.Initialize(look, cookiesWanted);
            if (street != null)
            {
                customer.EnterFromStreet(street.RandomCrossing(), street.RandomWalkSpeed());
            }

            queue.Join(customer);
        }
    }
}
