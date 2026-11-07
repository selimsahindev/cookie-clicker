using UnityEngine;

namespace CookieClicker.Common
{
    /// <summary>
    /// Draws a circular progress ring with two LineRenderers: a full track and a fill arc.
    /// </summary>
    public class ProgressRing : MonoBehaviour
    {
        private const int SegmentCount = 64;

        [SerializeField] private MonoBehaviour progressSource;
        [SerializeField] private LineRenderer track;
        [SerializeField] private LineRenderer fill;
        [SerializeField] private float radius = 0.32f;

        private IProgressSource source;
        private float lastDrawnProgress = -1f;

        private void Awake()
        {
            source = progressSource as IProgressSource;
            if (source == null)
            {
                Debug.LogError($"{name}: progress source must implement {nameof(IProgressSource)}.", this);
            }

            DrawArc(track, 1f);
        }

        private void LateUpdate()
        {
            if (source == null)
            {
                return;
            }

            float progress = Mathf.Clamp01(source.Progress);
            if (Mathf.Approximately(progress, lastDrawnProgress))
            {
                return;
            }

            lastDrawnProgress = progress;
            DrawArc(fill, progress);
        }

        private void DrawArc(LineRenderer line, float amount)
        {
            int pointCount = Mathf.Max(2, Mathf.CeilToInt(SegmentCount * amount) + 1);
            line.positionCount = pointCount;
            line.enabled = amount > 0f;

            for (int i = 0; i < pointCount; i++)
            {
                float t = amount * i / (pointCount - 1);
                float angle = Mathf.PI * 0.5f - t * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
            }
        }

        private void OnValidate()
        {
            if (progressSource != null && !(progressSource is IProgressSource))
            {
                Debug.LogWarning($"{name}: {progressSource.GetType().Name} does not implement {nameof(IProgressSource)}.", this);
            }
        }
    }
}
