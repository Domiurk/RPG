using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/new ItemDatabase", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
#if UNITY_EDITOR
        public static string PropNameItems => nameof(_items);
        public static string PropNameNames => nameof(_namesParent);
#endif

        public List<Item> Items => _items;
        public List<string> NamesParent => _namesParent;

        [SerializeField] private List<Item> _items;
        [SerializeField] private List<string> _namesParent;
    }
}