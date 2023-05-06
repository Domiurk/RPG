using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Runtime.Items
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/new ItemDatabase", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
#if UNITY_EDITOR
        public static string PropItems => nameof(_items);
        public static string PropBones => nameof(_bones);
#endif

        public List<Item> Items => _items;
        public List<Bone> Bones => _bones;

        [SerializeField] private List<Item> _items;
        [SerializeField] private List<Bone> _bones;

        public Bone GetBone(IName nameBone)
            => _bones.Find(b => b.Name == nameBone.Name);

        public int GetBoneIndex(Bone bone)
            => _bones.IndexOf(bone);
    }
}