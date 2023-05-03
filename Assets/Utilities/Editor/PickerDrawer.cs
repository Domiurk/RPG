using Codice.Client.BaseCommands.BranchExplorer.ExplorerTree;
using Items;
using UnityEditor;
using UnityEngine;
using Utilities.Runtime;

namespace Utilities.Editor
{
    [CustomPropertyDrawer(typeof(PickerAttribute), true)]
    public abstract class PickerDrawer<T> : PropertyDrawer where T : ScriptableObject, IName
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            T current = (T)property.objectReferenceValue;
            position = EditorGUI.PrefixLabel(position, label);
            DoSelection(position, property,label,current);
            EditorGUI.EndProperty();
        }

        protected virtual void DoSelection(Rect buttonRect, SerializedProperty property, GUIContent label, T current)
        {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(current != null ? current.Name : "NULL");

            if(GUI.Button(buttonRect, buttonContent, buttonStyle)){
                
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}