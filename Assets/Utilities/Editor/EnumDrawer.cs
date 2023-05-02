using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Runtime;

namespace Utilities.Editor
{
    [CustomPropertyDrawer(typeof(EnumAttribute))]
    public class EnumDrawer : PropertyDrawer
    {
        private SerializedProperty _property;

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            _property = property;

            if(property.propertyType != SerializedPropertyType.Enum){
                position.height *= 1.5f;
                EditorGUI.HelpBox(position,$"Sorry but {property} field is not enum", MessageType.Warning);
                return;
            }
            
            Type type = ((EnumAttribute)attribute).Type;
            EditorGUI.BeginProperty(position, label, property);

            Rect labelPosition = new Rect(position.x, position.y, position.width / 2, position.height); 
            EditorGUI.LabelField(labelPosition, label);
            Rect buttonPosition = new Rect(labelPosition.x + labelPosition.width, labelPosition.y, position.width - labelPosition.width, position.height);

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