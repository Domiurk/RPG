using System;
using System.Collections.Generic;
using Runtime.Items;
using UnityEngine;

namespace Runtime.Player
{
    public class EquipHandler : MonoBehaviour
    {
#if UNITY_EDITOR
        public static string PropBoneTransforms => nameof(_boneTransforms);
        public static string PropItemDatabase => nameof(_itemDatabase);
#endif

        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private List<Transform> _boneTransforms = new List<Transform>();
        [SerializeField] private ItemDatabase _itemDatabase;

        private GameObject _instance;
        // [SerializeField]private List<Bone> _bones;
        private readonly Dictionary<Bone, Transform> _boneTransform = new();

#if UNITY_EDITOR

        private void OnValidate()
        {
            if(_itemDatabase == null)
                return;

            if(_itemDatabase.Bones.Count != _boneTransform.Count)
                for(int i = 0; i < _itemDatabase.Bones.Count; i++)
                    _boneTransform.Add(_itemDatabase.Bones[i], _boneTransforms[i]);
        }
#endif

        public bool EquipTest2(Transform gameObj, EquipItem item)
        {
            if(item.Bone == null)
                throw new NullReferenceException($"Please Select bone in {item.Name}");

            Transform boneTransform = _boneTransform[item.Bone];
            if(boneTransform != null && item.Prefab != null){ // не шукає Transform
                gameObj.SetParent(boneTransform);

                gameObj.transform.localPosition = item.Offset.Position;
                gameObj.transform.localRotation = Quaternion.Euler(item.Offset.Rotation);
                gameObj.transform.localScale = item.Offset.Scale;
                return true;
            }

            return false;
        }

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
            // Bone bone = _bones.Find(bone => bone.Name == item.NameBone);

            // if(bone.IsEmpty){
            // bone.SetItem(item);
            // }
        }

        public void UnEquip( /*EquipItem item*/)
        {
            Destroy(_instance);
        }
    }

    public enum BoneHandType
    {
        Right,
        Left,
        None
    }
}