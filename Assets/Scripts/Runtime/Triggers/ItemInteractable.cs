using Items;
using Player;
using Runtime.Items;
using UnityEngine;

namespace Runtime.Triggers
{
    public class ItemInteractable : Interactable
    {
        [SerializeField] private string _nameWindow = "Inventory";
        [SerializeField] private Item _item;
        private EquipHandler _equipHandler;
        
        protected override void Interact()
        {
            if(_item != null && _equipHandler != null){
                if(InventoryService.Current.TryAdd(_nameWindow,_item))
                    Debug.Log($"You pickup {_item.Name}.");
                _equipHandler.Equip(_item);
                Destroy(gameObject);
            }
        }

        protected override void Enter(Collider other)
        {
            var inventory = other.GetComponent<EquipHandler>();
            if(inventory != null)
                _equipHandler = inventory;
        }

        protected override void Exit(Collider other)
        {
            if(other.GetComponent<EquipHandler>() && _equipHandler != null)
                _equipHandler = null;
        }

    }
}