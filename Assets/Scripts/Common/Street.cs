using UnityEngine;

namespace CookieClicker.Common
{
    /// <summary>A walk across the street from one edge to the other.</summary>
    public readonly struct StreetCrossing
    {
        public StreetCrossing(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        public Vector3 Start { get; }
        public Vector3 End { get; }

        /// <summary>The point on this crossing's walking line at <paramref name="x"/>.</summary>
        public Vector3 PointAt(float x)
        {
            return new Vector3(x, Start.y, Start.z);
        }
    }

    /// <summary>
    /// The walking lane in front of the cart. Passers-by and arriving customers cross it in both directions.
    /// </summary>
    public class Street : MonoBehaviour
    {
        [Tooltip("People walk between these two x positions, starting at either side.")]
        [SerializeField] private float leftX = -7f;
        [SerializeField] private float rightX = 7f;
        [Tooltip("Random y range of the walking line, in world units.")]
        [SerializeField] private Vector2 laneY = new Vector2(0.65f, 0.8f);
        [Tooltip("Negative z draws people on the street in front of the customer line.")]
        [SerializeField] private float laneZ = -0.1f;
        [SerializeField] private Vector2 walkSpeedRange = new Vector2(1.8f, 2.6f);

        public StreetCrossing RandomCrossing()
        {
            bool walksRight = Random.value < 0.5f;
            float y = Random.Range(laneY.x, laneY.y);
            var left = new Vector3(leftX, y, laneZ);
            var right = new Vector3(rightX, y, laneZ);
            return walksRight ? new StreetCrossing(left, right) : new StreetCrossing(right, left);
        }

        public float RandomWalkSpeed()
        {
            return Random.Range(walkSpeedRange.x, walkSpeedRange.y);
        }
    }
}
