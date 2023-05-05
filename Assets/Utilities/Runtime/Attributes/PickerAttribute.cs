using UnityEngine;

namespace Utilities.Runtime
{
    public class PickerAttribute : PropertyAttribute
    {
        public bool Utility;

        public PickerAttribute() : this(true)
        { }

        public PickerAttribute(bool utility)
        {
            Utility = utility;
        }
    }
}