﻿using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class BoolVariable : Variable
    {
        [SerializeField] private bool m_Value;

        public bool Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value;
            set => m_Value = (bool)value;
        }

        public override Type type => typeof(bool);

        public BoolVariable() { }

        public BoolVariable(string name) : base(name) { }

        public static implicit operator BoolVariable(bool value)
        {
            return new BoolVariable{
                Value = value
            };
        }

        public static implicit operator bool(BoolVariable value)
        {
            return value.Value;
        }
    }
}