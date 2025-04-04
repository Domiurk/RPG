﻿using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class Vector2Variable : Variable
    {
        [SerializeField] private Vector2 m_Value;

        public Vector2 Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (Vector2)value;
        }

        public override Type type => typeof(Vector2);

        public Vector2Variable() { }

        public Vector2Variable(string name) : base(name) { }

        public static implicit operator Vector2Variable(Vector2 value)
        {
            return new Vector2Variable{
                Value = value
            };
        }

        public static implicit operator Vector2(Vector2Variable value)
        {
            return value.Value;
        }
    }
}