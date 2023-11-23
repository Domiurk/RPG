using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class StringVariable : Variable
    {
        [SerializeField] private string m_Value = string.Empty;

        public string Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (string)value;
        }

        public override Type type => typeof(string);

        public StringVariable() { }

        public StringVariable(string name) : base(name) { }

        public static implicit operator StringVariable(string value)
        {
            return new StringVariable{
                Value = value
            };
        }

        public static implicit operator string(StringVariable value)
        {
            return value.Value;
        }
    }
}