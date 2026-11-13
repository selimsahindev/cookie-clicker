using System;
using System.Collections;
using System.IO;
using CookieClicker.Baking;
using CookieClicker.Common;
using CookieClicker.Customers;
using CookieClicker.Economy;
using CookieClicker.Table;
using CookieClicker.Upgrades;
using CookieClicker.Upgrades.Effects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CookieClicker.Tests
{
    public class BakeryLoopTests
    {
        private const string SceneName = "Bakery";
        private const string ScreenshotEnvironmentVariable = "COOKIE_SCREENSHOT";

        private IsolatedSaveStorage isolatedSave;

        [SetUp]
        public void SetUp()
        {
            isolatedSave = new IsolatedSaveStorage();
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;
            isolatedSave.Dispose();
        }

        [UnityTest]
        public IEnumerator BakerFillsTableAndCustomersGetServed()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            var table = UnityEngine.Object.FindAnyObjectByType<CookieTable>();
            var server = UnityEngine.Object.FindAnyObjectByType<CookieServer>();
            Assert.IsNotNull(baker);
            Assert.IsNotNull(table);
            Assert.IsNotNull(server);

            int cookiesBaked = 0;
            int customersSatisfied = 0;
            baker.CookieBaked += () => cookiesBaked++;
            server.CustomerSatisfied += _ => customersSatisfied++;

            Time.timeScale = 4f;
            yield return new WaitForSeconds(40f);
            Time.timeScale = 1f;

            Assert.Greater(cookiesBaked, 0, "Baker should bake cookies.");
            Assert.Greater(customersSatisfied, 0, "At least one customer should get all their cookies and leave.");
            Assert.LessOrEqual(table.Count, table.Capacity);

            var wallet = UnityEngine.Object.FindAnyObjectByType<Wallet>();
            Assert.Greater(wallet.Balance, 0, "Every sold cookie should earn money.");

            // Pause serving so the table fills up before the screenshot.
            server.enabled = false;
            yield return new WaitForSeconds(6f);
            CaptureScreenshotIfRequested();
        }

        [UnityTest]
        public IEnumerator BakerWaitsWhenTableIsFull()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            UnityEngine.Object.FindAnyObjectByType<CustomerSpawner>().enabled = false;
            var table = UnityEngine.Object.FindAnyObjectByType<CookieTable>();
            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            table.Capacity = 2;
            baker.BakeDuration = 0.1f;

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(2, table.Count);
            Assert.IsTrue(baker.IsWaitingForSpace);
            Assert.IsFalse(baker.IsBaking, "The baker should rest while the table is full.");
        }

        [UnityTest]
        public IEnumerator TapsAreIgnoredWhileTableIsFull()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            UnityEngine.Object.FindAnyObjectByType<CustomerSpawner>().enabled = false;
            var table = UnityEngine.Object.FindAnyObjectByType<CookieTable>();
            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            table.Capacity = 1;
            baker.BakeDuration = 0.1f;
            yield return new WaitForSeconds(0.5f);

            int boosts = 0;
            baker.Boosted += () => boosts++;
            baker.BakeDuration = 1000f;
            float before = baker.Progress;
            baker.Boost();

            Assert.AreEqual(0, boosts, "A tap on a full table should not trigger the bake animation.");
            Assert.AreEqual(before, baker.Progress, 0.001f);
        }

        [UnityTest]
        public IEnumerator FullTableHidesRingAndShowsCount()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            UnityEngine.Object.FindAnyObjectByType<CustomerSpawner>().enabled = false;
            var table = UnityEngine.Object.FindAnyObjectByType<CookieTable>();
            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            var ring = UnityEngine.Object.FindAnyObjectByType<ProgressRing>();
            var countLabel = UnityEngine.Object.FindAnyObjectByType<CookieCountLabel>();
            Assert.IsNotNull(countLabel, "Run Cookie Clicker > Install Missing Features to add the cookie holder.");
            table.TakeCookies(table.Count);
            table.Capacity = 2;
            baker.BakeDuration = 0.1f;

            yield return new WaitForSeconds(1f);

            Assert.IsFalse(baker.IsProgressVisible);
            foreach (LineRenderer line in ring.GetComponentsInChildren<LineRenderer>())
            {
                Assert.IsFalse(line.enabled, "The ring should hide while the table is full.");
            }

            Assert.AreEqual("2/2", countLabel.Text);

            table.TakeCookies(1);
            yield return null;
            Assert.AreEqual("1/2", countLabel.Text);
        }

        [UnityTest]
        public IEnumerator BoostAdvancesBakeInstantly()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            baker.BakeDuration = 1000f;
            yield return null;

            float before = baker.Progress;
            baker.Boost();

            Assert.AreEqual(before + baker.ClickBoost, baker.Progress, 0.001f);
        }

        [UnityTest]
        public IEnumerator BuyingBakeSpeedSpendsMoneyAndSpeedsUpBaking()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var shop = UnityEngine.Object.FindAnyObjectByType<UpgradeShop>();
            var wallet = UnityEngine.Object.FindAnyObjectByType<Wallet>();
            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            Upgrade bakeSpeed = UnityEngine.Object.FindAnyObjectByType<BakeSpeedEffect>().GetComponent<Upgrade>();
            wallet.TrySpend(wallet.Balance);

            float durationBefore = baker.BakeDuration;
            long cost = bakeSpeed.Cost;
            Assert.IsFalse(shop.TryBuy(bakeSpeed), "Should not be able to buy without money.");

            wallet.Add(cost);
            Assert.IsTrue(shop.TryBuy(bakeSpeed));

            Assert.AreEqual(0, wallet.Balance);
            Assert.AreEqual(UpgradeDefinition.StartingLevel + 1, bakeSpeed.Level);
            Assert.Less(baker.BakeDuration, durationBefore);
            Assert.Greater(bakeSpeed.Cost, cost, "Next level should cost more.");
        }

        [UnityTest]
        public IEnumerator CustomerTakesWholeOrderAtOnceWhenEnoughCookiesAreReady()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);

            var baker = UnityEngine.Object.FindAnyObjectByType<Baker>();
            var table = UnityEngine.Object.FindAnyObjectByType<CookieTable>();
            var queue = UnityEngine.Object.FindAnyObjectByType<CustomerQueue>();
            var server = UnityEngine.Object.FindAnyObjectByType<CookieServer>();
            baker.enabled = false;
            server.enabled = false;

            Time.timeScale = 4f;
            float timeout = 20f;
            while ((queue.Front == null || !queue.Front.IsReadyToBeServed) && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            Time.timeScale = 1f;
            Assert.IsNotNull(queue.Front, "A customer should reach the front of the line.");

            Customer front = queue.Front;
            int wanted = front.CookiesWanted;
            table.Capacity = Mathf.Max(table.Capacity, wanted + 2);
            for (int i = 0; i < wanted + 2; i++)
            {
                table.TryAddCookie();
            }

            int handOvers = 0;
            int servedCount = 0;
            server.CookiesServed += (_, count) =>
            {
                handOvers++;
                servedCount += count;
            };
            server.enabled = true;
            yield return null;

            Assert.AreEqual(1, handOvers, "The whole order should be handed over in one go.");
            Assert.AreEqual(wanted, servedCount);
            Assert.AreEqual(2, table.Count);
        }

        [Test]
        public void UpgradeCostGrowsBy15PercentPerLevel()
        {
            var definition = ScriptableObject.CreateInstance<UpgradeDefinition>();
            definition.Configure("Test", null, 100, 1f, UpgradeDefinition.ValueGrowth.Multiply, 0.9f, "{0}");

            Assert.AreEqual(100, definition.GetCost(1));
            Assert.AreEqual(115, definition.GetCost(2));
            Assert.AreEqual(132, definition.GetCost(3));
            Assert.AreEqual(0.81f, definition.GetValue(3), 0.0001f);
        }

        private static void CaptureScreenshotIfRequested()
        {
            string path = Environment.GetEnvironmentVariable(ScreenshotEnvironmentVariable);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            Camera camera = Camera.main;
            var target = new RenderTexture(1080, 1920, 24);
            camera.targetTexture = target;
            camera.Render();

            RenderTexture.active = target;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            image.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            File.WriteAllBytes(path, image.EncodeToPNG());
            Debug.Log($"Screenshot saved to {path}");
        }
    }
}
