using UnityEditor;

namespace DevionGames.InventorySystem.Configuration
{
    [CustomEditor(typeof(Default))]
    public class DefaultInspector : Editor
    {
        private SerializedProperty m_ShowAllComponents;
        private SerializedProperty m_Script;

        private void OnEnable()
        {
            if (target == null) return;
            m_Script = serializedObject.FindProperty("m_Script");
            m_ShowAllComponents = serializedObject.FindProperty("showAllComponents");
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, m_ShowAllComponents.propertyPath, m_Script.propertyPath);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ShowAllComponents);
            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool("InventorySystem.showAllComponents", m_ShowAllComponents.boolValue);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
