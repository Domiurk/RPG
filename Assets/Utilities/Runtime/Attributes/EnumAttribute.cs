using System;
using UnityEngine;

namespace Utilities.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnumAttribute : PropertyAttribute
    { }
}