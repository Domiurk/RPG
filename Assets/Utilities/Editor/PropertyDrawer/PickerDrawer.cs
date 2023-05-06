using System.Collections.Generic;
using System.Linq;
using Items;
using Runtime.Items;
using UnityEditor;
using UnityEngine;
using Utilities.Runtime;
using Object = UnityEngine.Object;

namespace Utilities.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(PickerAttribute), true)]
    public abstract class PickerDrawer<T> : UnityEditor.PropertyDrawer where T : ScriptableObject, IName
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            T current = (T)property.objectReferenceValue;
            position = EditorGUI.PrefixLabel(position, label);
            DoSelection(position, property, current);
            EditorGUI.EndProperty();
        }

        protected virtual void DoSelection(Rect buttonRect, SerializedProperty property, T current)
        {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(current != null ? current.Name : "Null Element");

            if(GUI.Button(buttonRect, buttonContent, buttonStyle))
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), BuildSelectableObjects(),
                                              obj => {
                                                  property.serializedObject.Update();
                                                  property.objectReferenceValue = obj;
                                                  property.serializedObject.ApplyModifiedProperties();
                                              },
                                              () => EditorTools.Create<ItemDatabase>(true));
        }

        protected abstract List<T> GetItems(ItemDatabase database);

        protected Dictionary<Object, List<Object>> BuildSelectableObjects()
        {
            Dictionary<Object, List<Object>> selectableObject = new Dictionary<Object, List<Object>>();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemDatabase)}");

            foreach(string guid in guids){
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
                List<Object> items = GetItems((ItemDatabase)obj).Cast<Object>().ToList();

                foreach(Object item in items)
                    item.name = ((IName)item).Name;
                selectableObject.Add(obj, items);
            }

            return selectableObject;
        }
    }
}