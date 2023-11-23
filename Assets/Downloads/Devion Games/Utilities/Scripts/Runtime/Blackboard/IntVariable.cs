using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class IntVariable : Variable
    {
        [SerializeField] private int m_Value;

        public int Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (int)value;
        }

        public override Type type => typeof(int);

        public IntVariable() { }

        public IntVariable(string name) : base(name) { }

        public static implicit operator IntVariable(int value)
        {
            return new IntVariable{
                Value = value
            };
        }

        public static implicit operator int(IntVariable value)
        {
            return value.Value;
        }
    }
}