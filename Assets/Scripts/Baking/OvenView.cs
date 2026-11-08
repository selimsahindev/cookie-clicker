using UnityEngine;

namespace CookieClicker.Baking
{
    /// <summary>
    /// Lights up the oven window while the baker is baking: fades in, flickers like a real oven,
    /// flashes when a cookie comes out and fades out when baking stops (for example when the cart is full).
    /// </summary>
    public class OvenView : MonoBehaviour
    {
        [SerializeField] private Baker baker;
        [SerializeField] private SpriteRenderer glow;

        [SerializeField] private float fadeSpeed = 2.5f;
        [SerializeField, Range(0f, 0.5f)] private float flickerAmount = 0.12f;
        [SerializeField] private float flickerSpeed = 6f;
        [SerializeField, Range(0f, 1f)] private float bakedFlash = 0.35f;
        [SerializeField] private float flashFadeSpeed = 2f;

        private float intensity;
        private float flash;

        private void OnEnable()
        {
            baker.CookieBaked += Flash;
        }

        private void OnDisable()
        {
            baker.CookieBaked -= Flash;
        }

        private void Flash()
        {
            flash = bakedFlash;
        }

        private void Update()
        {
            float target = baker.IsBaking ? 1f : 0f;
            intensity = Mathf.MoveTowards(intensity, target, fadeSpeed * Time.deltaTime);
            flash = Mathf.MoveTowards(flash, 0f, flashFadeSpeed * Time.deltaTime);

            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) - 0.5f;
            float flicker = noise * 2f * flickerAmount * intensity;
            float alpha = Mathf.Clamp01(intensity * (1f - flickerAmount) + flicker + flash);

            Color color = glow.color;
            color.a = alpha;
            glow.color = color;
        }
    }
}
