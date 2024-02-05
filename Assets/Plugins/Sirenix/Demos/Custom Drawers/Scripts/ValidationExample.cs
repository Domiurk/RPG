#if UNITY_EDITOR
[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(Sirenix.OdinInspector.Demos.NotOneAttributeValidator))]

namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor.Validation;

#endif

    [TypeInfoBox(
                    "This is example demonstrates how to implement a custom validator, that validates the property's value, " +
                    "and how to get Odin Project Validator (if installed) to pick up that validation warning or error.")]
    public class ValidationExample : MonoBehaviour
    {
        [NotOne]
        public int NotOne;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotOneAttribute : Attribute
    {
    }

#if UNITY_EDITOR

    public class NotOneAttributeValidator : AttributeValidator<NotOneAttribute, int>
    {
        protected override void Validate(ValidationResult result)
        {
            if (ValueEntry.SmartValue == 1)
            {
                result.Message = "1 is not a valid value.";
                result.ResultType = ValidationResultType.Error;
            }
        }
    }

#endif
}
#endif
