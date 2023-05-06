using Items;
using Runtime.Items;
using Runtime.Player;
using UnityEngine;
using Utilities.Runtime.Attributes;

namespace Runtime.Triggers
{
    public class ItemInteractable : Interactable
    {
        [SerializeField] private string _nameWindow = "Inventory";
        [SerializeField, ItemPicker] private StaticItem _item;
        [SerializeField] private MonoBehaviour _behaviour;
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

                if(_item is EquipItem equip && _equipHandler.EquipTest2(gameObject.transform,equip)){
                    this.enabled = false;
                    GetComponent<Rigidbody>().isKinematic = true;
                }
                    // _equipHandler.Equip(equip);
                // Destroy(gameObject);
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