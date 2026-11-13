using System.Collections;
using CookieClicker.Economy;
using CookieClicker.Saving;
using CookieClicker.Shopping;
using CookieClicker.Shopping.Effects;
using CookieClicker.Table;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CookieClicker.Tests
{
    public class ShopTests
    {
        private const string SceneName = "Bakery";

        private IsolatedSaveStorage isolated;

        [SetUp]
        public void SetUp()
        {
            isolated = new IsolatedSaveStorage();
        }

        [TearDown]
        public void TearDown()
        {
            isolated.Dispose();
        }

        [UnityTest]
        public IEnumerator BiggerHolderRaisesCapacityAndSurvivesReload()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var effect = Object.FindAnyObjectByType<HolderCapacityEffect>();
            if (effect == null)
            {
                Assert.Ignore("The shop isn't installed yet. Run Cookie Clicker → Install Missing Features.");
            }

            var holder = effect.GetComponent<Unlockable>();
            var shop = Object.FindAnyObjectByType<Shop>();
            var wallet = Object.FindAnyObjectByType<Wallet>();
            var table = Object.FindAnyObjectByType<CookieTable>();
            int oldCapacity = table.Capacity;

            Assert.IsFalse(shop.TryBuy(holder), "Buying without money should fail.");
            wallet.Add(holder.Price);
            Assert.IsTrue(shop.TryBuy(holder));
            Assert.IsFalse(shop.TryBuy(holder), "An unlock is bought only once.");
            Assert.Greater(table.Capacity, oldCapacity);

            while (table.TryAddCookie())
            {
            }

            int fullCount = table.Count;
            SaveManager.Instance.Save();

            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var reloaded = Object.FindAnyObjectByType<CookieTable>();
            Assert.IsTrue(Object.FindAnyObjectByType<HolderCapacityEffect>().GetComponent<Unlockable>().IsUnlocked);
            Assert.AreEqual(fullCount, reloaded.Capacity);
            Assert.AreEqual(fullCount, reloaded.Count, "Cookies above the old capacity must not be lost on load.");
        }
    }
}
