using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    [CustomEditor(typeof(UIContainer<>), true)]
    public class UIContainerInspector : UIWidgetInspector
    {
        private SerializedProperty m_DynamicContainer;
        private SerializedProperty m_SlotPrefab;
        private SerializedProperty m_SlotParent;
        private AnimBool m_ShowDynamicContainer;

        private string[] m_PropertiesToExcludeForDefaultInspector;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_DynamicContainer = serializedObject.FindProperty("m_DynamicContainer");
            m_SlotParent = serializedObject.FindProperty("m_SlotParent");
            m_SlotPrefab = serializedObject.FindProperty("m_SlotPrefab");

            if (m_SlotParent.objectReferenceValue == null)
            {
                GridLayoutGroup group = ((MonoBehaviour)target).gameObject.GetComponentInChildren<GridLayoutGroup>();
                if (group != null)
                {
                    serializedObject.Update();
                    m_SlotParent.objectReferenceValue = group.transform;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            m_ShowDynamicContainer = new AnimBool(m_DynamicContainer.boolValue);
            m_ShowDynamicContainer.valueChanged.AddListener(Repaint);
            m_PropertiesToExcludeForDefaultInspector = new[] {
                m_DynamicContainer.propertyPath,
                m_SlotParent.propertyPath,
                m_SlotPrefab.propertyPath,
            };
        }

        private void DrawInspector()
        {
            EditorGUILayout.PropertyField(m_DynamicContainer);
            m_ShowDynamicContainer.target = m_DynamicContainer.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowDynamicContainer.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_SlotParent);
                EditorGUILayout.PropertyField(m_SlotPrefab);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
            DrawClassPropertiesExcluding(m_PropertiesToExcludeForDefaultInspector);
        }

    }
}