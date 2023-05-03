using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Runtime.Player
{
    public class EquipHandler : MonoBehaviour
    {
        [SerializeField] private List<Bone> _bones;
        
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;

        private GameObject _instance;

        public void Equip(EquipItem item)
        {
            EquipItem instantiate = Instantiate(item);

            if(instantiate != null && instantiate.Prefab != null){
                _instance = Instantiate(instantiate.Prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));

                if((_rightHand != null || _leftHand != null) && instantiate.TypeEquip != BoneHandType.None){
                    switch(instantiate.TypeEquip){
                        case BoneHandType.Right:
                            _instance.transform.SetParent(_rightHand);
                            break;
                        case BoneHandType.Left:
                            _instance.transform.SetParent(_leftHand);
                            break;
                        case BoneHandType.None:
                        default:
                            break;
                    }

                    _instance.transform.localPosition = instantiate.Offset.Position;
                    _instance.transform.localRotation = Quaternion.Euler(instantiate.Offset.Rotation);
                    _instance.transform.localScale = instantiate.Offset.Scale;
                }     
            }
        }

        public void EquipTest(EquipItem item)
        {
            Bone bone = _bones.Find(bone => bone.Name == item.NameBone);

            if(bone.IsEmpty){
                bone.SetItem(item);
            }
        }

        public void UnEquip(/*EquipItem item*/)
        {
            
            Destroy(_instance);
        }
    }

    [System.Serializable]
    public class Bone : IName
    {
        public string Name => _name;
        public Transform BoneTransform;
        public bool IsEmpty { get; private set; }

        [SerializeField] private string _name;
        private GameObject _instance;

        public void SetItem(EquipItem item)
        {
            EquipItem instance = Object.Instantiate(item);
            _instance = Object.Instantiate(instance.Prefab, BoneTransform);
            _instance.transform.localPosition = instance.Offset.Position;
            _instance.transform.localRotation = Quaternion.Euler(instance.Offset.Rotation);
            _instance.transform.localScale = instance.Offset.Scale;

            IsEmpty = false;
        }

        public void ClearBone()
        {
            Object.Destroy(_instance);

            IsEmpty = true;
        }
    }
    
    public enum BoneHandType
    {
        Right,
        Left,
        None
    }
}