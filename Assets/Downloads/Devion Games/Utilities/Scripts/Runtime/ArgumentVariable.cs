using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevionGames
{
    [Serializable]
    public class ArgumentVariable
    {
        [SerializeField] private bool m_BoolValue;
        [SerializeField] private int m_IntValue;
        [SerializeField] private float m_FloatValue;
        [SerializeField] private string m_StringValue = string.Empty;
        [SerializeField] private Vector2 m_Vector2Value = Vector2.zero;
        [SerializeField] private Vector3 m_Vector3Value = Vector3.zero;
        [SerializeField] private Color m_ColorValue = Color.white;
        [SerializeField] private Object m_ObjectValue;

        [SerializeField] private ArgumentType m_ArgumentType = ArgumentType.None;

        public ArgumentType ArgumentType
        {
            get => m_ArgumentType;
            set => m_ArgumentType = value;
        }

        public bool IsNone => m_ArgumentType == ArgumentType.None;

        public object GetValue()
        {
            return m_ArgumentType switch{
                ArgumentType.Bool => m_BoolValue,
                ArgumentType.Int => m_IntValue,
                ArgumentType.Float => m_FloatValue,
                ArgumentType.String => m_StringValue,
                ArgumentType.Vector2 => m_Vector2Value,
                ArgumentType.Vector3 => m_Vector3Value,
                ArgumentType.Color => m_ColorValue,
                ArgumentType.Object => m_ObjectValue,
                _ => null
            };
        }

        public static string GetPropertyValuePath(ArgumentType argumentType)
        {
            return argumentType switch{
                ArgumentType.Bool => "m_BoolValue",
                ArgumentType.Int => "m_IntValue",
                ArgumentType.Float => "m_FloatValue",
                ArgumentType.String => "m_StringValue",
                ArgumentType.Vector2 => "m_Vector2Value",
                ArgumentType.Vector3 => "m_Vector3Value",
                ArgumentType.Color => "m_ColorValue",
                ArgumentType.Object => "m_ObjectValue",
                _ => string.Empty
            };
        }
    }

    public enum ArgumentType
    {
        None,
        Bool,
        Int,
        Float,
        String,
        Vector2,
        Vector3,
        Color,
        Object
    }
}