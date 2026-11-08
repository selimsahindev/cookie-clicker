using System;
using System.Collections;
using System.IO;
using CookieClicker.Baking;
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
