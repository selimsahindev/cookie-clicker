using CookieClicker.Common;
using UnityEngine;

namespace CookieClicker.Ambience
{
    /// <summary>
    /// A passer-by who crosses the street once and disappears. Never joins the line.
    /// </summary>
    [RequireComponent(typeof(CharacterWalker))]
    public class Pedestrian : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer body;

        private CharacterWalker walker;
        private bool isWalking;

        private void Awake()
        {
            walker = GetComponent<CharacterWalker>();
        }

        public void Cross(Sprite look, StreetCrossing crossing, float speed)
        {
            body.sprite = look;
            transform.position = crossing.Start;
            walker.Speed = speed;
            walker.WalkTo(crossing.End);
            isWalking = true;
        }

        private void Update()
        {
            if (isWalking && walker.IsAtTarget)
            {
                Destroy(gameObject);
            }
        }
    }
}
