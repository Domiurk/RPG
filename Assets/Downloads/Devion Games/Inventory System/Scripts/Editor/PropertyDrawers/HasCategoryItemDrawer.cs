using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomPropertyDrawer(typeof(HasCategoryItem))]
    public class HasCategoryItemDrawer : PropertyDrawer
    {
        private SerializedProperty m_RequiredItems;
        private readonly Dictionary<string, ReorderableList> m_ListMap = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            ReorderableList m_RequiredItemList = null;
            if (!m_ListMap.TryGetValue(property.propertyPath, out m_RequiredItemList))
            {
          
                m_RequiredItems = property.FindPropertyRelative("requiredItems");
                m_RequiredItemList = new ReorderableList(property.serializedObject, m_RequiredItems, true, true, true, true);
                m_RequiredItemList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
                {
                    SerializedProperty element = m_RequiredItemList.serializedProperty.GetArrayElementAtIndex(index);
                    SerializedProperty itemProperty = element.FindPropertyRelative("category");
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width *= 0.5f;
                    EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
                    rect.x += rect.width + 5;
                    rect.width -= 5f;
                    SerializedProperty window = element.FindPropertyRelative("stringValue");
                    EditorGUI.PropertyField(rect, window, GUIContent.none);
                };
                m_RequiredItemList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Required Category(Category, Window)");
                };
                m_ListMap.Add(property.propertyPath, m_RequiredItemList);
            }

            try
            {
                m_RequiredItemList.DoLayoutList();
            }catch  {
                if(Event.current.type == EventType.Repaint)
                    m_ListMap.Remove(property.propertyPath);
            }
            EditorGUILayout.Space();

        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 5f;
        }
    }
}