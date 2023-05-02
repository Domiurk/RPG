using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Items
{
    public class InventoryService : MonoBehaviour
    {
        public static InventoryService Current { get; private set; }

        public ItemDatabase ItemDatabase => _itemDatabase;

        [SerializeField] private ItemDatabase _itemDatabase;
        [SerializeField] private List<Container> _containers;

        private void Awake()
        {
            if(Current != null && Current != this)
                Destroy(this);
            else
                Current = this;

            _containers = new List<Container>(GameObject.FindObjectsByType<Container>(FindObjectsSortMode.None));
        }

        public bool AddItemForWindow(string nameWindow, Item item)
        {
            Item itemInstance = Instantiate(item);

            foreach(Container container in _containers.Where(container => container.Name == nameWindow)){
                container.TryAddItem(itemInstance);
                return true;
            }

            return false;
        }
    }
}