using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class FloatVariable : Variable
    {
        [SerializeField] private float m_Value;

        public float Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = Convert.ToSingle(value);
        }

        public override Type type => typeof(float);

        public FloatVariable() { }

        public FloatVariable(string name) : base(name) { }

        public static implicit operator FloatVariable(float value)
        {
            return new FloatVariable{
                Value = value
            };
        }

        public static implicit operator float(FloatVariable value)
        {
            return value.Value;
        }
    }
}