using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevionGames
{
    [Serializable]
    public class ObjectVariable : Variable
    {
        [SerializeField] private Object m_Value;

        public Object Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (Object)value;
        }

        public override Type type => typeof(Object);

        public ObjectVariable() { }

        public ObjectVariable(string name) : base(name) { }

        public static implicit operator ObjectVariable(Object value)
        {
            return new ObjectVariable{
                Value = value
            };
        }

        public static implicit operator Object(ObjectVariable value)
        {
            return value.Value;
        }
    }
}