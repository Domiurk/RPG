using Items;
using Runtime.Items;
using Runtime.Player;
using UnityEngine;

namespace Runtime.Triggers
{
    public class ItemInteractable : Interactable
    {
        [SerializeField] private string _nameWindow = "Inventory";
        [SerializeField] private StaticItem _item;
        private EquipHandler _equipHandler;

        public void Init(StaticItem item, string nameWindow = "Inventory", KeyCode keyInteract = KeyCode.E)
        {
            _item = item;
            _nameWindow = nameWindow;
            InteractKey = keyInteract;
        }

        protected override void Interact()
        {
            if(_item != null && _equipHandler != null){
                if(InventoryService.Current.TryAdd(_nameWindow, _item))
                    Debug.Log($"You pickup {_item.Name}.");
                if(_item is EquipItem equip)
                    _equipHandler.Equip(equip);
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