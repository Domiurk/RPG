using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class Vector3Variable : Variable
    {
        [SerializeField] private Vector3 m_Value;

        public Vector3 Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (Vector3)value;
        }

        public override Type type => typeof(Vector3);

        public Vector3Variable() { }

        public Vector3Variable(string name) : base(name) { }

        public static implicit operator Vector3Variable(Vector3 value)
        {
            return new Vector3Variable{
                Value = value
            };
        }

        public static implicit operator Vector3(Vector3Variable value)
        {
            return value.Value;
        }
    }
}