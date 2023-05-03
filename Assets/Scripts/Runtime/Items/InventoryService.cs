using System.Collections.Generic;
using System.Linq;
using Items;
using Runtime.UI_Components;
using UnityEngine;

namespace Runtime.Items
{
    public class InventoryService : MonoBehaviour
    {
        public static InventoryService Current { get; private set; }
        
        public ItemDatabase ItemDatabase => _itemDatabase;

        [SerializeField] private ItemDatabase _itemDatabase;
        
        private List<ItemContainer> _containers;

        private void Awake()
        {
            if(Current != null && Current != this)
                Destroy(this);
            else
                Current = this;

            _containers = new List<ItemContainer>(GameObject.FindObjectsByType<ItemContainer>(FindObjectsSortMode.None));
        }

        public bool TryAdd(string nameWindow, Item item)
        {
            Item itemInstance = Instantiate(item);

            return _containers.Where(container => container.Name == nameWindow)
                              .Select(container => container.TryAddItem(itemInstance))
                              .FirstOrDefault();
        }

        public bool TryAdd(string nameWindow, string nameItem)
        {
            Item item = GetItem(nameItem);
            return item != null && TryAdd(nameWindow, item);
        }

        private Item GetItem(string nameItem)
            => Instantiate(_itemDatabase.Items.Find(item => item.Name == nameItem));
    }
}