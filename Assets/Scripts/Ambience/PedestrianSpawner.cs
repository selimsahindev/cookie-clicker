using CookieClicker.Common;
using UnityEngine;

namespace CookieClicker.Ambience
{
    /// <summary>
    /// Sends passers-by across the street in both directions at random intervals.
    /// Customers come from the same street, so the passers-by who never stop make the buyers feel chosen.
    /// </summary>
    public class PedestrianSpawner : MonoBehaviour
    {
        [SerializeField] private Pedestrian pedestrianPrefab;
        [SerializeField] private Street street;
        [SerializeField] private Sprite[] looks;

        [SerializeField] private Vector2 spawnIntervalRange = new Vector2(1.5f, 4f);
        [SerializeField] private Vector2 scaleRange = new Vector2(0.95f, 1.05f);
        [SerializeField, Min(1)] private int maxPedestrians = 4;

        private float timeUntilNextSpawn;

        private void Update()
        {
            timeUntilNextSpawn -= Time.deltaTime;
            if (timeUntilNextSpawn > 0f || transform.childCount >= maxPedestrians)
            {
                return;
            }

            Spawn();
            timeUntilNextSpawn = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
        }

        private void Spawn()
        {
            StreetCrossing crossing = street.RandomCrossing();
            Pedestrian pedestrian = Instantiate(pedestrianPrefab, crossing.Start, Quaternion.identity, transform);
            pedestrian.transform.localScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);
            Sprite look = looks[Random.Range(0, looks.Length)];
            pedestrian.Cross(look, crossing, street.RandomWalkSpeed());
        }
    }
}
