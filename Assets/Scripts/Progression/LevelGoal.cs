using System;
using CookieClicker.Economy;
using CookieClicker.Saving;
using UnityEngine;

namespace CookieClicker.Progression
{
    /// <summary>
    /// The big earnings goal for the current level ("Dream Shop").
    /// Counts money earned from sales, so spending on upgrades never moves the bar back.
    /// Raises <see cref="Completed"/> once when the goal is reached; moving to the next level hooks in there.
    /// </summary>
    public class LevelGoal : MonoBehaviour, ISaveable
    {
        [SerializeField] private CashRegister cashRegister;
        [SerializeField, Min(1)] private long targetEarnings = 10000;

        private long earned;
        private bool completed;

        public event Action Changed;
        public event Action Completed;

        public long Earned => earned;
        public bool IsCompleted => completed;
        public float Progress => Mathf.Clamp01((float)((double)earned / targetEarnings));

        public long TargetEarnings
        {
            get
            {
                return targetEarnings;
            }
            set
            {
                targetEarnings = Math.Max(1, value);
                completed = earned >= targetEarnings;
                Changed?.Invoke();
            }
        }

        private void OnEnable()
        {
            if (cashRegister != null)
            {
                cashRegister.Earned += HandleEarned;
            }
        }

        private void OnDisable()
        {
            if (cashRegister != null)
            {
                cashRegister.Earned -= HandleEarned;
            }
        }

        public void AddEarnings(long amount)
        {
            if (amount <= 0)
            {
                return;
            }

            earned += amount;
            Changed?.Invoke();

            if (!completed && earned >= targetEarnings)
            {
                completed = true;
                Completed?.Invoke();
            }
        }

        public void Capture(SaveData data)
        {
            data.goalEarnings = earned;
        }

        public void Restore(SaveData data)
        {
            earned = Math.Max(0, data.goalEarnings);
            completed = earned >= targetEarnings;
            Changed?.Invoke();
        }

        private void HandleEarned(long amount, Vector3 position)
        {
            AddEarnings(amount);
        }
    }
}
