using UnityEngine;

namespace Runtime.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Game/Item", order = 0)]
    public class Item : ScriptableObject
    {
        public string Name => _name;
        public string Description => _description;
        public GameObject Prefab => _prefab;

        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private GameObject _prefab;
    }
}