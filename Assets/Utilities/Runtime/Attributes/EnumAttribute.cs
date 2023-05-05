using System;
using UnityEngine;

namespace Utilities.Runtime
{
    // [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class EnumAttribute : PropertyAttribute
    {
        public Type Type { get; }

        public EnumAttribute(Type type)
        {
            Type = type;
        }
    }
}