using System.Collections.Generic;
using UnityEngine;

namespace CookieClicker.Customers
{
    /// <summary>
    /// A single-file line of customers. The first one stands at the front slot next to the table.
    /// </summary>
    public class CustomerQueue : MonoBehaviour
    {
        [SerializeField] private Transform frontSlot;
        [SerializeField] private Vector3 spacing = new Vector3(1.6f, 0f, 0f);
        [SerializeField, Min(0)] private int maxLength = 5;

        private readonly List<Customer> customers = new List<Customer>();

        public int Count => customers.Count;
        public bool HasRoom => customers.Count < maxLength;
        public Customer Front => customers.Count > 0 ? customers[0] : null;

        /// <summary>Upgrades can make the line longer at runtime.</summary>
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
            set
            {
                maxLength = Mathf.Max(0, value);
            }
        }

        public void Join(Customer customer)
        {
            customers.Add(customer);
            customer.WalkTo(SlotPosition(customers.Count - 1));
        }

        public void Remove(Customer customer)
        {
            if (customers.Remove(customer))
            {
                MoveEveryoneUp();
            }
        }

        private void MoveEveryoneUp()
        {
            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].WalkTo(SlotPosition(i));
            }
        }

        private Vector3 SlotPosition(int index)
        {
            return frontSlot.position + spacing * index;
        }
    }
}
