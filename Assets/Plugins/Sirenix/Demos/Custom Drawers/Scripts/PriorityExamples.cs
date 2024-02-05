#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using Sirenix.Utilities.Editor;

#endif

    [TypeInfoBox(
                    "In this example, we have three different drawers, with different priorities, all drawing the same value.\n\n" +
                    "The purpose is to demonstrate the drawer chain, and the general purpose of each drawer priority.")]
    public class PriorityExamples : MonoBehaviour
    {
        [ShowDrawerChain] public MyClass MyClass;
    }

    [Serializable]
    public class MyClass
    {
        public string Name;
        public float Value;
    }

#if UNITY_EDITOR

    [DrawerPriority(1, 0, 0)]
    public class CUSTOM_SuperPriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (ValueEntry.SmartValue == null)
            {
                ValueEntry.SmartValue = new MyClass();
            }

            CallNextDrawer(label);
        }
    }

    [DrawerPriority(0, 1, 0)]
    public class CUSTOM_WrapperPriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginBox(label);
            CallNextDrawer(null);
            SirenixEditorGUI.EndBox();
        }
    }

    [DrawerPriority(0, 0, 1)]
    public class CUSTOM_ValuePriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueEntry.Property.Children["Name"].Draw();
            ValueEntry.Property.Children["Value"].Draw();
        }
    }

#endif
}
#endif
