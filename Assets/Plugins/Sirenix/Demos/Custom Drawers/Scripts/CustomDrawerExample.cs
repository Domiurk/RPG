#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System;

#if UNITY_EDITOR

    using Editor;
    using UnityEditor;
    using Utilities;

#endif

    [TypeInfoBox("This example demonstrates how a custom drawer can be implemented for a custom struct or class.")]
    public class CustomDrawerExample : MonoBehaviour
    {
        public MyStruct MyStruct;
    }

    [Serializable]
    public struct MyStruct
    {
        public float X;
        public float Y;
    }

#if UNITY_EDITOR

    public class CustomStructDrawer : OdinValueDrawer<MyStruct>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            MyStruct value = ValueEntry.SmartValue;

            Rect rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20;

            value.X = EditorGUI.Slider(rect.AlignLeft(rect.width * 0.5f), "X", value.X, 0, 1);
            value.Y = EditorGUI.Slider(rect.AlignRight(rect.width * 0.5f), "Y", value.Y, 0, 1);

            EditorGUIUtility.labelWidth = prev;

            ValueEntry.SmartValue = value;
        }
    }

#endif
}
#endif
