using Items;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Item _item;

        public void Equip(Item item)
        {
            if(!InventoryService.Current.AddItemForWindow("Inventory", item))
                return;
            _item = Instantiate(item);

            GameObject instance = Instantiate(_item.Prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));

            if((_rightHand != null || _leftHand != null) && _item.TypeEquip != BoneHandType.None){
                switch(_item.TypeEquip){
                    case BoneHandType.Right:
                        instance.transform.SetParent(_rightHand);
                        break;
                    case BoneHandType.Left:
                        instance.transform.SetParent(_leftHand);
                        break;
                    case BoneHandType.None:
                    default:
                        break;
                }

                instance.transform.localPosition = _item.Offset.Position;
                instance.transform.localRotation = Quaternion.Euler(_item.Offset.Rotation);
                instance.transform.localScale = _item.Offset.Scale;
            }
        }
    }

    public enum BoneHandType
    {
        Right,
        Left,
        None
    }
}