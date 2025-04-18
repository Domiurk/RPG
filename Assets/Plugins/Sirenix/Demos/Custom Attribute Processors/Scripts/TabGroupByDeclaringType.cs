#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Editor;
    using Utilities;

    [TypeInfoBox(
        "This example demonstrates how you could use AttributeProcessors to arrange properties " +
        "into different groups, based on where they were declared.")]
    public class TabGroupByDeclaringType : Bar
    {
        public string A, B, C;
    }

    public class TabifyFooResolver<T> : OdinAttributeProcessor<T> where T : Foo
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            int inheritanceDistance = member.DeclaringType.GetInheritanceDistance(typeof(object));
            string tabName = member.DeclaringType.Name;
            attributes.Add(new TabGroupAttribute(tabName));
            attributes.Add(new PropertyOrderAttribute(-inheritanceDistance));
        }
    }
}
#endif
