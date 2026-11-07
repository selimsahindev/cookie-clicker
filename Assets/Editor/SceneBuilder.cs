using System.IO;
using CookieClicker.Baking;
using CookieClicker.Common;
using CookieClicker.Customers;
using CookieClicker.Economy;
using CookieClicker.Table;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static CookieClicker.EditorTools.BuildHelpers;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// Creates the street-vendor scene from the sprites in Assets/Art/Sprites.
    /// The scene is generated only once. After that it belongs to the editor, so manual changes are never overwritten
    /// unless you explicitly reset it.
    /// Command line:
    /// Unity -batchmode -quit -projectPath . -executeMethod CookieClicker.EditorTools.SceneBuilder.Build
    /// </summary>
    public static class SceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Bakery.unity";
        private const string LineMaterialPath = "Assets/Art/Materials/ProgressRing.mat";
        private const float BakerScale = 1.3f;
        private const float CustomerLineY = 1.0f;

        /// <summary>Creates whatever is missing (prefabs, scene) and leaves existing assets untouched.</summary>
        [MenuItem("Cookie Clicker/Create Missing Content")]
        public static void Build()
        {
            ConfigureProject();
            PrefabBuilder.BuildMissing();

            if (File.Exists(ScenePath))
            {
                Debug.Log($"{ScenePath} already exists and was left untouched. Use 'Cookie Clicker/Reset Bakery Scene...' to regenerate it.");
                return;
            }

            BuildScene();
            Debug.Log("Bakery scene created.");
        }

        [MenuItem("Cookie Clicker/Reset Bakery Scene...")]
        private static void ResetFromMenu()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Bakery Scene",
                "This regenerates the scene, the prefabs and the sprite import settings from code.\n\nEvery manual change to them will be lost.",
                "Reset",
                "Cancel");
            if (!confirmed)
            {
                return;
            }

            Reset();
        }

        /// <summary>Regenerates everything from code, discarding manual changes. Also callable from the command line.</summary>
        public static void Reset()
        {
            ConfigureProject();
            SpriteImportSetup.ConfigureAll();
            PrefabBuilder.BuildAll();
            BuildScene();
            Debug.Log("Bakery scene reset.");
        }

        private static void ConfigureProject()
        {
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
            PlayerSetup.ConfigurePortrait();
        }

        private static void BuildScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Camera camera = CreateCamera();
            AddSprite(new GameObject("Background"), LoadSprite("background"), SortingOrder.Background);
            CreateProp("Oven", "oven", new Vector3(-3.6f, -5.1f, 0f), SortingOrder.Oven);
            CreateProp("Umbrella", "umbrella", new Vector3(1.75f, 0.85f, 0f), SortingOrder.Umbrella);

            CookieTable table = CreateCart();
            CreateBaker(table);
            CookieServer server = CreateCustomers(table);
            (Wallet wallet, CashRegister cashRegister) = CreateEconomy(server);
            HudBuilder.Build(camera, wallet, cashRegister);

            EnsureFolder(Path.GetDirectoryName(ScenePath));
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        private static Camera CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 9.6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Palette.Sky;
            return camera;
        }

        private static void CreateProp(string name, string spriteName, Vector3 position, int sortingOrder)
        {
            var prop = new GameObject(name);
            prop.transform.position = position;
            AddSprite(prop, LoadSprite(spriteName), sortingOrder);
        }

        private static CookieTable CreateCart()
        {
            var cart = new GameObject("CookieCart");
            cart.transform.position = new Vector3(0f, -1.2f, 0f);
            AddSprite(cart, LoadSprite("cart"), SortingOrder.Table);
            var table = cart.AddComponent<CookieTable>();

            var stackObject = CreateChild(cart.transform, "CookieStacks", new Vector3(-1.3f, 2.08f, 0f));
            var stackView = stackObject.AddComponent<CookieStackView>();
            SetReference(stackView, "table", table);
            SetReference(stackView, "cookieSprite", LoadSprite("cookie"));
            SetInt(stackView, "sortingOrder", SortingOrder.TableCookies);
            return table;
        }

        private static void CreateBaker(CookieTable table)
        {
            var bakerObject = new GameObject("Baker");
            bakerObject.transform.position = new Vector3(0f, -6.3f, 0f);
            var visual = CreateChild(bakerObject.transform, "Visual", Vector3.zero);
            visual.transform.localScale = new Vector3(BakerScale, BakerScale, 1f);

            var rig = CreateChild(visual.transform, "Rig", Vector3.zero);
            AddSprite(rig, LoadSprite("vendor_body"), SortingOrder.Baker);
            var leftArm = CreateChild(rig.transform, "LeftArm", new Vector3(-0.48f, 1.24f, 0f));
            AddSprite(leftArm, LoadSprite("vendor_arm_left"), SortingOrder.Baker - 1);
            var rightArm = CreateChild(rig.transform, "RightArm", new Vector3(0.48f, 1.24f, 0f));
            AddSprite(rightArm, LoadSprite("vendor_arm_right"), SortingOrder.Baker - 1);

            var baker = bakerObject.AddComponent<Baker>();
            SetReference(baker, "table", table);

            var tapToBake = bakerObject.AddComponent<TapToBake>();
            SetReference(tapToBake, "baker", baker);

            var animator = bakerObject.AddComponent<BakerAnimator>();
            SetReference(animator, "baker", baker);
            SetReference(animator, "rig", rig.transform);
            SetReference(animator, "leftArm", leftArm.transform);
            SetReference(animator, "rightArm", rightArm.transform);

            CreateProgressRing(bakerObject.transform, baker);
        }

        private static void CreateProgressRing(Transform parent, Baker baker)
        {
            Material lineMaterial = LoadOrCreateLineMaterial();
            var ringObject = CreateChild(parent, "ProgressRing", new Vector3(0f, 4.45f, 0f));
            LineRenderer track = CreateLine(ringObject.transform, "Track", lineMaterial, Palette.RingTrack, SortingOrder.Ring);
            LineRenderer fill = CreateLine(ringObject.transform, "Fill", lineMaterial, Palette.RingFill, SortingOrder.Ring + 1);
            track.loop = true;

            var ring = ringObject.AddComponent<ProgressRing>();
            SetReference(ring, "progressSource", baker);
            SetReference(ring, "track", track);
            SetReference(ring, "fill", fill);
            SetFloat(ring, "radius", 0.38f);
        }

        private static CookieServer CreateCustomers(CookieTable table)
        {
            // Customers walk in from the left, line up toward the cart and leave to the right.
            var root = new GameObject("Customers");
            var frontSlot = CreateChild(root.transform, "FrontSlot", new Vector3(0f, CustomerLineY, 0f));
            var spawnPoint = CreateChild(root.transform, "SpawnPoint", new Vector3(-7f, CustomerLineY, 0f));
            var exitPoint = CreateChild(root.transform, "ExitPoint", new Vector3(7f, CustomerLineY, 0f));

            var queue = root.AddComponent<CustomerQueue>();
            SetReference(queue, "frontSlot", frontSlot.transform);
            SetVector3(queue, "spacing", new Vector3(-1.5f, 0f, 0f));
            SetInt(queue, "maxLength", 4);

            var spawner = root.AddComponent<CustomerSpawner>();
            SetReference(spawner, "customerPrefab", AssetDatabase.LoadAssetAtPath<Customer>(PrefabBuilder.CustomerPrefabPath));
            SetReference(spawner, "queue", queue);
            SetReference(spawner, "spawnPoint", spawnPoint.transform);
            SetSpriteArray(spawner, "looks", PrefabBuilder.CustomerLooks);

            var server = root.AddComponent<CookieServer>();
            SetReference(server, "table", table);
            SetReference(server, "queue", queue);
            SetReference(server, "exitPoint", exitPoint.transform);
            return server;
        }

        private static (Wallet, CashRegister) CreateEconomy(CookieServer server)
        {
            var root = new GameObject("Economy");
            var wallet = root.AddComponent<Wallet>();
            var payoutPoint = CreateChild(root.transform, "PayoutPoint", new Vector3(0.5f, 1.6f, 0f));

            var cashRegister = root.AddComponent<CashRegister>();
            SetReference(cashRegister, "server", server);
            SetReference(cashRegister, "wallet", wallet);
            SetReference(cashRegister, "payoutPoint", payoutPoint.transform);
            return (wallet, cashRegister);
        }

        private static Material LoadOrCreateLineMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(LineMaterialPath);
            if (material != null)
            {
                return material;
            }

            EnsureFolder(Path.GetDirectoryName(LineMaterialPath));
            material = new Material(Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(material, LineMaterialPath);
            return material;
        }

        private static LineRenderer CreateLine(Transform parent, string name, Material material, Color color, int sortingOrder)
        {
            var line = CreateChild(parent, name, Vector3.zero).AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.sharedMaterial = material;
            line.startColor = color;
            line.endColor = color;
            line.widthMultiplier = 0.13f;
            line.numCapVertices = 6;
            line.numCornerVertices = 2;
            line.sortingOrder = sortingOrder;
            line.positionCount = 0;
            return line;
        }
    }
}
