using System.Collections;
using System.IO;
using CookieClicker.Economy;
using CookieClicker.Saving;
using CookieClicker.Table;
using CookieClicker.Upgrades;
using CookieClicker.Upgrades.Effects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CookieClicker.Tests
{
    public class SaveSystemTests
    {
        private const string SceneName = "Bakery";

        private IsolatedSaveStorage isolated;
        private FileSaveStorage storage;

        [SetUp]
        public void SetUp()
        {
            isolated = new IsolatedSaveStorage();
            storage = isolated.Storage;
        }

        [TearDown]
        public void TearDown()
        {
            isolated.Dispose();
        }

        [Test]
        public void StorageRoundTripsAndKeepsPreviousSaveAsBackup()
        {
            storage.Write("first");
            storage.Write("second");

            Assert.IsTrue(storage.TryRead(out string contents));
            Assert.AreEqual("second", contents);
            Assert.IsTrue(storage.TryReadBackup(out string backup));
            Assert.AreEqual("first", backup);
            Assert.IsFalse(File.Exists(storage.MainPath + ".tmp"));
        }

        [Test]
        public void StorageFallsBackToBackupWhenMainFileIsMissing()
        {
            storage.Write("first");
            storage.Write("second");
            File.Delete(storage.MainPath);

            Assert.IsTrue(storage.TryRead(out string contents));
            Assert.AreEqual("first", contents);
        }

        [Test]
        public void SerializerRejectsCorruptAndTooNewSaves()
        {
            Assert.IsFalse(SaveSerializer.TryDeserialize("{ not json", out _));

            var tooNew = new SaveData { version = SaveData.CurrentVersion + 1 };
            Assert.IsFalse(SaveSerializer.TryDeserialize(SaveSerializer.Serialize(tooNew), out _));
        }

        [UnityTest]
        public IEnumerator GameStateSurvivesReload()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var wallet = Object.FindAnyObjectByType<Wallet>();
            var table = Object.FindAnyObjectByType<CookieTable>();
            var shop = Object.FindAnyObjectByType<UpgradeShop>();
            Upgrade price = Object.FindAnyObjectByType<CookiePriceEffect>().GetComponent<Upgrade>();

            wallet.Add(price.Cost);
            Assert.IsTrue(shop.TryBuy(price));
            wallet.Add(1234);
            long money = wallet.Balance;
            int level = price.Level;
            SaveManager.Instance.Save();
            int cookies = table.Count;

            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            Assert.AreEqual(money, Object.FindAnyObjectByType<Wallet>().Balance);
            Assert.AreEqual(level, Object.FindAnyObjectByType<CookiePriceEffect>().GetComponent<Upgrade>().Level);
            Assert.AreEqual(cookies, Object.FindAnyObjectByType<CookieTable>().Count);
        }
    }
}
