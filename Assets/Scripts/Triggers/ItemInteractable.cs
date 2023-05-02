using Items;
using Player;
using UnityEngine;

namespace Triggers
{
    public class ItemInteractable : Interactable
    {
        [SerializeField] private Item _item;
    
        private PlayerInventory _inventory;

        protected override void Interact()
        {
            if(_item != null && _inventory != null){
                _inventory.Equip(_item);
                Destroy(gameObject);
            }
        }

        protected override void Enter(Collider other)
        {
            var inventory = other.GetComponent<PlayerInventory>();
            if(inventory != null)
                _inventory = inventory;
        }

        protected override void Exit(Collider other)
        {
            if(other.GetComponent<PlayerInventory>() && _inventory != null)
                _inventory = null;
        }
    }
}