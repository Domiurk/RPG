using System;
using UnityEditor;
using UnityEngine;
using Utilities.Runtime.Attributes;

namespace Utilities.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(EnumAttribute))]
    public class EnumDrawer : UnityEditor.PropertyDrawer
    {
        private SerializedProperty _property;

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            _property = property;

            if(property.propertyType != SerializedPropertyType.Enum){
                position.height *= 1.5f;
                EditorGUI.HelpBox(position, $"Sorry but {property} field is not enum", MessageType.Warning);
                return;
            }

            Type type = fieldInfo.FieldType;
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            Rect buttonPosition = position;

            if(GUI.Button(buttonPosition, property.enumDisplayNames[property.enumValueIndex], GUI.skin.textField))
                EnumWindow.Show(buttonPosition, type, ChangeProperty, property);

            EditorGUI.EndProperty();

            Save(property);
        }

        private static void Save(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.SetIsDifferentCacheDirty();
        }

        private void ChangeProperty(int index)
            => _property.enumValueIndex = index;
    }
}