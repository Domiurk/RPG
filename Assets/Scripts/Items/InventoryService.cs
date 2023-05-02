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
        
        private List<Container> _containers;

        private void Awake()
        {
            if(Current != null && Current != this)
                Destroy(this);
            else
                Current = this;

            _containers = new List<Container>(GameObject.FindObjectsByType<Container>(FindObjectsSortMode.None));
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