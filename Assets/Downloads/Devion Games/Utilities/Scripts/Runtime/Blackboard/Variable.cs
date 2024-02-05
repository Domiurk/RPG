using System;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public abstract class Variable
    {
        [SerializeField] private string m_Name = string.Empty;

        public virtual string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        [SerializeField]
        private bool m_IsShared;

        public virtual bool isShared
        {
            get => m_IsShared;
            set => m_IsShared = value;
        }

        public virtual bool isNone => (m_Name == "None" || string.IsNullOrEmpty(m_Name)) && m_IsShared;

        public abstract Type type { get; }

        public abstract object RawValue { get; set; }

        public Variable() { }

        public Variable(string name)
        {
            this.name = name;
        }
    }
}