using System;
using CookieClicker.Ambience;
using CookieClicker.Common;
using CookieClicker.Customers;
using UnityEditor;
using UnityEngine;
using static CookieClicker.EditorTools.BuildHelpers;
using Object = UnityEngine.Object;

namespace CookieClicker.EditorTools
{
    /// <summary>
    /// A street in front of the cart. Passers-by cross it both ways and new customers walk in along it,
    /// so only some of the people on the street stop to buy.
    /// </summary>
    internal class PedestrianFeature : ISceneFeature
    {
        private const float LaneOffsetMin = -0.35f;
        private const float LaneOffsetMax = -0.2f;

        public string Name => "Street and pedestrians";

        public bool IsInstalled()
        {
            return Object.FindAnyObjectByType<Street>() != null;
        }

        public void Install()
        {
            CustomerQueue queue = FindQueue();
            float lineY = CustomerLineY(queue);

            var root = new GameObject("Street");
            var street = root.AddComponent<Street>();
            SetVector2(street, "laneY", new Vector2(lineY + LaneOffsetMin, lineY + LaneOffsetMax));

            var spawner = root.AddComponent<PedestrianSpawner>();
            SetReference(spawner, "pedestrianPrefab", AssetDatabase.LoadAssetAtPath<Pedestrian>(PrefabBuilder.PedestrianPrefabPath));
            SetReference(spawner, "street", street);
            SetSpriteArray(spawner, "looks", PrefabBuilder.StreetLooks);

            ConnectCustomers(queue.GetComponent<CustomerSpawner>(), street);
        }

        /// <summary>Customers now arrive along the street and look like any other passer-by.</summary>
        private static void ConnectCustomers(CustomerSpawner customerSpawner, Street street)
        {
            if (customerSpawner == null)
            {
                Debug.LogWarning("No CustomerSpawner next to the CustomerQueue. Assign the Street on it by hand.");
                return;
            }

            SetReference(customerSpawner, "street", street);
            SetSpriteArray(customerSpawner, "looks", PrefabBuilder.StreetLooks);
        }

        private static CustomerQueue FindQueue()
        {
            var queue = Object.FindAnyObjectByType<CustomerQueue>();
            if (queue == null)
            {
                throw new InvalidOperationException("The bakery scene has no CustomerQueue.");
            }

            return queue;
        }

        private static float CustomerLineY(CustomerQueue queue)
        {
            var frontSlot = (Transform)new SerializedObject(queue).FindProperty("frontSlot").objectReferenceValue;
            return frontSlot != null ? frontSlot.position.y : queue.transform.position.y;
        }
    }
}
