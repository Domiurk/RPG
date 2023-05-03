using System.Collections.Generic;
using Items;
using Player;
using Runtime.Player;
using UnityEngine;
using UnityEngine.Serialization;

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
        public List<string> Bones => _bones;

        [SerializeField] private List<Item> _items;
        [SerializeField] private List<string> _bones;
    }
}