using System;

namespace DevionGames
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CustomDrawerAttribute : Attribute
    {
        private readonly Type m_Type;
        public Type Type => m_Type;

        private readonly bool m_UseForChildren;
        public bool UseForChildren => m_UseForChildren;

        public CustomDrawerAttribute(Type type)
        {
            m_Type = type;
        }

        public CustomDrawerAttribute(Type type, bool useForChildren)
        {
            m_Type = type;
            m_UseForChildren = useForChildren;
        }
    }
}