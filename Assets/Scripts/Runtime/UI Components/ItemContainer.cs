using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Runtime.UI_Components
{
    public class ItemContainer : BaseContainer
    {
        public bool CanDragIn => _canDragIn;
        public bool CanDragOut => _canDragOut;
        
        [SerializeField] private List<Slot> _slots;
        [SerializeField] private bool _canDragIn;
        [SerializeField] private bool _canDragOut;

        private GameObject _player;

        private void Awake()
        {
            _slots = new List<Slot>(GetComponentsInChildren<Slot>());
            _player = GameObject.FindWithTag("Player");
        }

        public bool TryAddItem(Item item)
        {
            if(!CanDragIn)
                return false;

            foreach(Slot slot in _slots.Where(slot => slot.IsEmpty)){
                return AddItem(slot, item);
            }

            return false;
        }

        public void Drop(Slot slot)
        {
            if(!CanDragOut)
                return;
            GameObject prefab = Instantiate(slot.Drop(out Item item), transform.position, Quaternion.identity);
        }

        private bool AddItem(Slot slot, Item item)
        {
            slot.SetItem(item);
            return true;
        }
    }
}