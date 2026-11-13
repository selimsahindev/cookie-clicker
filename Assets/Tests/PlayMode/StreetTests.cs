using System.Collections;
using CookieClicker.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CookieClicker.Tests
{
    public class StreetTests
    {
        private GameObject host;

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(host);
        }

        [Test]
        public void CrossingsGoFromOneEdgeToTheOtherInBothDirections()
        {
            host = new GameObject("Street");
            var street = host.AddComponent<Street>();
            bool sawRight = false;
            bool sawLeft = false;

            for (int i = 0; i < 100; i++)
            {
                StreetCrossing crossing = street.RandomCrossing();
                Assert.AreEqual(14f, Mathf.Abs(crossing.End.x - crossing.Start.x), 0.0001f);
                Assert.AreEqual(crossing.Start.y, crossing.End.y);
                sawRight |= crossing.End.x > crossing.Start.x;
                sawLeft |= crossing.End.x < crossing.Start.x;
            }

            Assert.IsTrue(sawRight, "Some people should walk to the right.");
            Assert.IsTrue(sawLeft, "Some people should walk to the left.");
        }

        [UnityTest]
        public IEnumerator WalkerFollowsEveryPointInOrder()
        {
            host = new GameObject("Walker");
            var walker = host.AddComponent<CharacterWalker>();
            walker.Speed = 10f;
            var corner = new Vector3(2f, 0f, 0f);
            var end = new Vector3(2f, 1f, 0f);

            walker.WalkAlong(corner, end);
            bool passedCorner = false;
            float timeLeft = 2f;
            while (!walker.IsAtTarget && timeLeft > 0f)
            {
                passedCorner |= host.transform.position == corner;
                Assert.IsFalse(host.transform.position.y > 0f && host.transform.position.x < 2f, "The walker cut the corner.");
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            Assert.IsTrue(walker.IsAtTarget);
            Assert.AreEqual(end, host.transform.position);
            Assert.IsTrue(passedCorner);
        }
    }
}
