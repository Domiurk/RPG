using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Game/new Item", order = 0)]
    public class Item : ScriptableObject, IName
    {
#if UNITY_EDITOR
        public static string PropName => nameof(_name);
        public static string PropIcon => nameof(_icon);
#endif

        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        // [SerializeField] private ItemOffset _offset;
        // [SerializeField] private BoneHandType _typeEquip = BoneHandType.None;

        public string Name => _name;
        public Sprite Icon => _icon;
        // public ItemOffset Offset => _offset;
        // public BoneHandType TypeEquip => _typeEquip;
    }
}