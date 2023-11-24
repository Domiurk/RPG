#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct StatValue : IEquatable<StatValue>
    {
        [HideInInspector]
        public StatType Type;

        [Range(-100, 100)]
        [LabelWidth(70)]
        [LabelText("$Type")]
        public float Value;

        public StatValue(StatType type, float value)
        {
            Type = type;
            Value = value;
        }

        public StatValue(StatType type)
        {
            Type = type;
            Value = 0;
        }

        public bool Equals(StatValue other)
        {
            return Type == other.Type && Value == other.Value;
        }
    }
}
#endif
