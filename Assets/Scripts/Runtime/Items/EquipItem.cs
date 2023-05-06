using Items;
using Runtime.Player;
using UnityEngine;
using Utilities.Runtime;

namespace Runtime.Items
{
    public class EquipItem : UsableItem
    {
#if UNITY_EDITOR
        public static string PropTypeEquip => nameof(_typeEquip);
        public static string PropOffset => nameof(_offset);
#endif
        public ItemOffset Offset => _offset;
        public BoneHandType TypeEquip => _typeEquip;
        public Bone Bone => _bone;

        [SerializeField] private ItemOffset _offset;
        [SerializeField] private BoneHandType _typeEquip = BoneHandType.None;
        [SerializeField, BonePicker] private Bone _bone;
    }
}