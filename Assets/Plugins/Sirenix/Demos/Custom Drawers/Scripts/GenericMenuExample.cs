#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using UnityEditor;

#endif

    [TypeInfoBox(
                    "In this example, we have an attribute drawer that adds new options to the generic context menu.\n" +
                    "In this case, we're adding options to select a color.")]
    public class GenericMenuExample : MonoBehaviour
    {
        [ColorPicker]
        public Color Color;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColorPickerAttribute : Attribute
    {
    }

#if UNITY_EDITOR

    public class ColorPickerAttributeDrawer : OdinAttributeDrawer<ColorPickerAttribute, Color>, IDefinesGenericMenuItems
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
        }

        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            genericMenu.AddItem(new GUIContent("Colors/Red"), false, () => SetColor(Color.red));
            genericMenu.AddItem(new GUIContent("Colors/Green"), false, () => SetColor(Color.green));
            genericMenu.AddItem(new GUIContent("Colors/Blue"), false, () => SetColor(Color.blue));
            genericMenu.AddItem(new GUIContent("Colors/Yellow"), false, () => SetColor(Color.yellow));
            genericMenu.AddItem(new GUIContent("Colors/Cyan"), false, () => SetColor(Color.cyan));
            genericMenu.AddItem(new GUIContent("Colors/White"), false, () => SetColor(Color.white));
            genericMenu.AddItem(new GUIContent("Colors/Black"), false, () => SetColor(Color.black));
            genericMenu.AddDisabledItem(new GUIContent("Colors/Magenta"));
        }

        private void SetColor(Color color)
        {
            ValueEntry.SmartValue = color;
            ValueEntry.ApplyChanges();
        }
    }

#endif
}
#endif
