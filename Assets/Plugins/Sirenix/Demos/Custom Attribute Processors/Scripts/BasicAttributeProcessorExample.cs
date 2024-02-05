#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Editor;
    using UnityEngine;

    public class BasicAttributeProcessorExample : MonoBehaviour
    {
        public MyCustomClass Processed = new();
    }

    [Serializable]
    public class MyCustomClass
    {
        public ScaleMode Mode;
        public float Size;
    }

    public class MyResolvedClassAttributeProcessor : OdinAttributeProcessor<MyCustomClass>
    {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            attributes.Add(new InfoBoxAttribute("Dynamically added attributes."));
            attributes.Add(new InlinePropertyAttribute());
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Add(new HideLabelAttribute());
            attributes.Add(new BoxGroupAttribute("Box", showLabel: false));

            if (member.Name == "Mode")
            {
                attributes.Add(new EnumToggleButtonsAttribute());
            }
            else if (member.Name == "Size")
            {
                attributes.Add(new RangeAttribute(0, 5));
            }
        }
    }
}
#endif