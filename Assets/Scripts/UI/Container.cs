using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace UI
{
    public class Container : MonoBehaviour, IName
    {
        public string Name => _name;
        public bool CanDragIn => _canDragIn;
        public bool CanDragOut => _canDragOut;

        [SerializeField] private string _name;
        [SerializeField] private List<Slot> _slots;
        [SerializeField] private bool _canDragIn;
        [SerializeField] private bool _canDragOut;

        private void Start()
            => _slots = new List<Slot>(GetComponentsInChildren<Slot>());

        public bool TryAddItem(Item item)
        {
            if(!CanDragIn)
                return false;

            foreach(Slot slot in _slots.Where(slot => slot.IsEmpty)){
                return AddItem(slot, item);
            }

            return false;
        }

        public void Drop()
        {
            if(!CanDragOut)
                return;
        }

        private bool AddItem(Slot slot, Item item)
        {
            slot.SetItem(item);
            return true;
        }
    }
}