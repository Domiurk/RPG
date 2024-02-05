using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class GameObjectVariable : Variable
    {
        [SerializeField] private GameObject m_Value;

        public GameObject Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (GameObject)value;
        }

        public override Type type => typeof(GameObject);

        public GameObjectVariable() { }

        public GameObjectVariable(string name) : base(name) { }

        public static implicit operator GameObjectVariable(GameObject value)
        {
            return new GameObjectVariable{
                Value = value
            };
        }

        public static implicit operator GameObject(GameObjectVariable value)
        {
            return value.Value;
        }
    }
}