using UnityEngine;

namespace Utilities.Runtime
{
    public class PickerAttribute : PropertyAttribute
    {
        public bool Utilities;

        public PickerAttribute() : this(true)
        { }

        public PickerAttribute(bool utilities)
        {
            Utilities = utilities;
        }
    }
}