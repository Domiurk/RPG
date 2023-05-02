using Items;
using UnityEngine;

namespace Player
{
    public class EquipHandler : MonoBehaviour
    {
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;

        public void Equip(Item item)
        {
            /*if(!InventoryService.Current.AddItemForWindow("Inventory", item))
                return;*/
            if(item is not EquipItem)
                return;
            EquipItem instantiate = Instantiate(item) as EquipItem;

            if(instantiate != null && instantiate.Prefab != null){
                GameObject instance = Instantiate(instantiate.Prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));

                if((_rightHand != null || _leftHand != null) && instantiate.TypeEquip != BoneHandType.None){
                    switch(instantiate.TypeEquip){
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

                    instance.transform.localPosition = instantiate.Offset.Position;
                    instance.transform.localRotation = Quaternion.Euler(instantiate.Offset.Rotation);
                    instance.transform.localScale = instantiate.Offset.Scale;
                }
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