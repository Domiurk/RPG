#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Editor;
    using UnityEngine;

    [TypeInfoBox("This example demonstrates how AttributeProcessors are ordered by priority.")]
    public class AttributeProcessorPriorityExample : MonoBehaviour
    {
        public PrioritizedProcessed Processed;
    }

    [Serializable]
    public class PrioritizedProcessed
    {
        public int A;
    }

    [ResolverPriority(100)]
    public class FirstAttributeProcessor : OdinAttributeProcessor<PrioritizedProcessed>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Add(new BoxGroupAttribute("First"));
            attributes.Add(new RangeAttribute(0, 10));
        }
    }

    public class SecondAttributeProcessor : OdinAttributeProcessor<PrioritizedProcessed>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.RemoveAttributeOfType<RangeAttribute>();

            BoxGroupAttribute boxGroup = attributes.OfType<BoxGroupAttribute>().FirstOrDefault();
            boxGroup.GroupName = boxGroup.GroupName + " - Second";
        }
    }

    [ResolverPriority(-100)]
    public class ThirdAttributeProcessor : OdinAttributeProcessor<PrioritizedProcessed>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            BoxGroupAttribute boxGroup = attributes.OfType<BoxGroupAttribute>().FirstOrDefault();
            boxGroup.GroupName = boxGroup.GroupName + " - Third";
        }
    }
}
#endif
