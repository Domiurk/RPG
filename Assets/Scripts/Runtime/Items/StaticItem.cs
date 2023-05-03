using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Game/new Item", order = 0)]
    public class StaticItem : Item
    {
#if UNITY_EDITOR
        public static string PropPrefab => nameof(_prefab);
#endif

        [SerializeField] private GameObject _prefab;

        public GameObject Prefab => _prefab;
    }
}