using CookieClicker.Common;
using CookieClicker.Progression;
using TMPro;
using UnityEngine;

namespace CookieClicker.UI
{
    /// <summary>
    /// Thin bar across the top of the screen that fills toward the level goal, with a "Goal" title and a percentage.
    /// </summary>
    public class LevelGoalView : MonoBehaviour
    {
        [SerializeField] private LevelGoal goal;
        [SerializeField] private RectTransform fill;
        [SerializeField] private TMP_Text percentLabel;
        [SerializeField] private PopAnimation titlePop;
        [SerializeField] private float fillSpeed = 1.5f;

        private float shownProgress;

        private void OnEnable()
        {
            goal.Changed += Refresh;
            goal.Completed += Celebrate;
            shownProgress = goal.Progress;
            Refresh();
            ApplyFill();
        }

        private void OnDisable()
        {
            goal.Changed -= Refresh;
            goal.Completed -= Celebrate;
        }

        private void Update()
        {
            shownProgress = Mathf.MoveTowards(shownProgress, goal.Progress, fillSpeed * Time.deltaTime);
            ApplyFill();
        }

        private void Refresh()
        {
            percentLabel.text = $"{Mathf.FloorToInt(goal.Progress * 100f)}%";
        }

        private void Celebrate()
        {
            Refresh();
            titlePop.Play();
        }

        private void ApplyFill()
        {
            fill.anchorMax = new Vector2(shownProgress, 1f);
        }
    }
}
