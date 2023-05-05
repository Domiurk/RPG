using Items;
using UnityEngine;

namespace Runtime.Items
{
    public class Bone : ScriptableObject, IName
    {
#if UNITY_EDITOR
        public static string PropName => nameof(_name);
#endif
        
        public string Name => _name;

        [SerializeField] private string _name;
    }
}