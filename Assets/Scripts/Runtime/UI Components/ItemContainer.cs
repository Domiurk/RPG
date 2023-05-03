using System.Collections.Generic;
using System.Linq;
using Items;
using Runtime.Items;
using Runtime.Player;
using Runtime.Triggers;
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
            if(CanDragIn){
                foreach(Slot slot in _slots.Where(slot => slot.IsEmpty)){
                    slot.SetItem(item);
                    return true;
                }
            }

            return false;
        }

        public void Drop(Slot slot)
        {
            if(!CanDragOut)
                return;

            if(slot.Item is StaticItem staticItem){
                Vector3 position = _player.transform.position;
                GameObject prefab = Instantiate(staticItem.Prefab, new Vector3(position.x,position.y,position.z + 2), Quaternion.identity);
                MeshCollider meshCollider = prefab.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                prefab.AddComponent<ItemInteractable>();
                prefab.AddComponent<Rigidbody>();
                slot.Clear();
                _player.GetComponent<EquipHandler>()?.UnEquip();
            }
        }
    }
}