using CookieClicker.Progression;
using CookieClicker.Saving;
using NUnit.Framework;
using UnityEngine;

namespace CookieClicker.Tests
{
    public class LevelGoalTests
    {
        private GameObject host;
        private LevelGoal goal;

        [SetUp]
        public void SetUp()
        {
            host = new GameObject("LevelGoal");
            goal = host.AddComponent<LevelGoal>();
            goal.TargetEarnings = 100;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(host);
        }

        [Test]
        public void CompletesOnceWhenEarningsReachTheTarget()
        {
            int completions = 0;
            goal.Completed += () => completions++;

            goal.AddEarnings(60);
            Assert.AreEqual(0.6f, goal.Progress, 0.0001f);
            Assert.IsFalse(goal.IsCompleted);

            goal.AddEarnings(60);
            goal.AddEarnings(10);

            Assert.AreEqual(1f, goal.Progress);
            Assert.IsTrue(goal.IsCompleted);
            Assert.AreEqual(1, completions);
        }

        [Test]
        public void ProgressSurvivesSaveAndLoad()
        {
            goal.AddEarnings(42);
            var data = new SaveData();
            goal.Capture(data);

            var otherHost = new GameObject("Restored");
            var restored = otherHost.AddComponent<LevelGoal>();
            restored.TargetEarnings = 100;
            restored.Restore(data);

            Assert.AreEqual(42, restored.Earned);
            Object.DestroyImmediate(otherHost);
        }
    }
}
