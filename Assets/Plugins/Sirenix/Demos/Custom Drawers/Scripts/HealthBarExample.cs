#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using UnityEditor;
    using Sirenix.Utilities.Editor;
    using Utilities;

#endif

    [TypeInfoBox("Here a visualization of a health bar being drawn with with a custom attribute drawer.")]
    public class HealthBarExample : MonoBehaviour
    {
        [HealthBar(100)]
        public float Health;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HealthBarAttribute : Attribute
    {
        public float MaxHealth { get; private set; }

        public HealthBarAttribute(float maxHealth)
        {
            MaxHealth = maxHealth;
        }
    }

#if UNITY_EDITOR

    public class HealthBarAttributeDrawer : OdinAttributeDrawer<HealthBarAttribute, float>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);

            Rect rect = EditorGUILayout.GetControlRect();

            float width = Mathf.Clamp01(ValueEntry.SmartValue / Attribute.MaxHealth);
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0f, 0f, 0f, 0.3f), false);
            SirenixEditorGUI.DrawSolidRect(rect.SetWidth(rect.width * width), Color.red, false);
            SirenixEditorGUI.DrawBorders(rect, 1);
        }
    }

#endif
}
#endif
