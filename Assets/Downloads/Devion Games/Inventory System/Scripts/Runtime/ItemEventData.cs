using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class ItemEventData : CallbackEventData
    {
        public Item item;
        public Slot slot;
        public GameObject gameObject;

        public ItemEventData(Item item) {
            this.item = item;
            if (item != null){
                slot = item.Slot;
                gameObject = item.Prefab;
            }
        }
    }
}