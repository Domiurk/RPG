using Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Items
{
    public class Bone : ScriptableObject, IName
    {
#if UNITY_EDITOR
        public static string PropName => nameof(_name);
        public static string PropIndex => nameof(_index);
#endif
        
        public string Name => _name;
        public int Index => _index;

        [SerializeField] private string _name;
        private int _index;
    }
}