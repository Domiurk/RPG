using UnityEngine;

namespace Items
{
    [System.Serializable]
    public class ItemOffset
    {
        public Vector3 Position => _position;
        public Vector3 Rotation => _rotation;
        public Vector3 Scale => _scale;

        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private Vector3 _scale;

        public ItemOffset() : this(Vector3.zero, Vector3.zero, Vector3.one)
        { }

        public ItemOffset(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            _position = position;
            _rotation = rotation;
            _scale = scale;
        }
    }
}