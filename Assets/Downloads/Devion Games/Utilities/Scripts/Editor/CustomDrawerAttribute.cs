﻿using System;

namespace DevionGames
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CustomDrawerAttribute : Attribute
    {
        private Type m_Type;
        public Type Type => this.m_Type;

        private bool m_UseForChildren;
        public bool UseForChildren => this.m_UseForChildren;

        public CustomDrawerAttribute(Type type)
        {
            this.m_Type = type;
        }

        public CustomDrawerAttribute(Type type, bool useForChildren)
        {
            this.m_Type = type;
            this.m_UseForChildren = useForChildren;
        }
    }
}