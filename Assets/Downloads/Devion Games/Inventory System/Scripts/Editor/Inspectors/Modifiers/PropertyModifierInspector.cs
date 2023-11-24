using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(PropertyModifier), true)]
    public class PropertyModifierInspector : Editor
    {
        protected SerializedProperty m_ApplyToAll;
        protected AnimBool m_ApplyToAllOptions;

        protected SerializedProperty m_Properties;
        protected ReorderableList m_PropertyList;

        protected SerializedProperty m_ModifierType;
        protected SerializedProperty m_Range;

        protected virtual void OnEnable()
        {
            m_ApplyToAll = serializedObject.FindProperty("m_ApplyToAll");
            m_ApplyToAllOptions = new AnimBool(!m_ApplyToAll.boolValue);
            m_ApplyToAllOptions.valueChanged.AddListener(Repaint);

            m_Properties = serializedObject.FindProperty("m_Properties");
            m_PropertyList = new ReorderableList(serializedObject, m_Properties, true, true, true, true);
            m_PropertyList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Properties");
            };
            m_PropertyList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = m_PropertyList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
            m_ModifierType = serializedObject.FindProperty("m_ModifierType");
            m_Range = serializedObject.FindProperty("m_Range");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ModifierType);
            EditorGUILayout.PropertyField(m_Range);

            EditorGUILayout.PropertyField(m_ApplyToAll);
            m_ApplyToAllOptions.target = !m_ApplyToAll.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ApplyToAllOptions.faded))
            {
                m_PropertyList.DoLayoutList();
            }
            EditorGUILayout.EndFadeGroup();
     
            serializedObject.ApplyModifiedProperties();
        }

    }
}