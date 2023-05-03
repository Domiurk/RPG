using Player;
using Runtime.Player;
using UnityEngine;

namespace Items
{
    public class EquipItem : UsableItem
    {
#if UNITY_EDITOR
        public static string PropTypeEquip => nameof(_typeEquip);
        public static string PropOffset => nameof(_offset);
#endif
        
        [SerializeField] private ItemOffset _offset;
        [SerializeField] private BoneHandType _typeEquip = BoneHandType.None;
        [SerializeField] private string _nameBone;
        
        public ItemOffset Offset => _offset;
        public BoneHandType TypeEquip => _typeEquip;
        public string NameBone => _nameBone;
    }
}