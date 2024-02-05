using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [CustomEditor(typeof(StatsHandler))]
    public class StatsHandlerInspector : Editor
    {
        protected SerializedProperty m_Script;
        protected SerializedProperty m_Stats;
        protected ReorderableList m_StatList;
        protected SerializedProperty m_Effects;
        protected ReorderableList m_EffectsList;

        protected SerializedProperty m_StatOverrides;

        protected virtual void OnEnable() {
            if (target == null) return;
            m_Script = serializedObject.FindProperty("m_Script");
            m_Stats = serializedObject.FindProperty("m_Stats");
            m_StatList = CreateList("Stats", serializedObject, m_Stats);
            m_Effects = serializedObject.FindProperty("m_Effects");
            m_EffectsList = CreateList("Effects", serializedObject, m_Effects);
            m_StatOverrides = serializedObject.FindProperty("m_StatOverrides");

            int selectedStatIndex = EditorPrefs.GetInt("SelectedStatIndex." + target.GetInstanceID(), -1);
            m_StatList.index = selectedStatIndex;
        }


        protected virtual void OnDisable() {
            if (target == null) return;
            EditorPrefs.SetInt("SelectedStatIndex." + target.GetInstanceID(),m_StatList.index) ;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject,m_Script.propertyPath,m_Stats.propertyPath,m_Effects.propertyPath);

            m_StatList.DoLayoutList();

            if (m_StatOverrides.arraySize < m_Stats.arraySize)
            {
                for (int i = m_StatOverrides.arraySize; i < m_Stats.arraySize; i++)
                {
                    m_StatOverrides.InsertArrayElementAtIndex(i);
                }
            }

            int selectedStatIndex = m_StatList.index;
            if (selectedStatIndex > -1 && m_Stats.arraySize > 0)
            {
                SerializedProperty statOverride = m_StatOverrides.GetArrayElementAtIndex(selectedStatIndex);
                SerializedProperty overrideBaseValue = statOverride.FindPropertyRelative("overrideBaseValue");
                SerializedProperty baseValue = statOverride.FindPropertyRelative("baseValue");
              
                EditorGUILayout.PropertyField(overrideBaseValue);
                if (overrideBaseValue.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(baseValue);
                    EditorGUI.indentLevel -= 1;
                }
            }


            EditorGUILayout.Space();
            m_EffectsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList CreateList(string title, SerializedObject serializedObject, SerializedProperty elements)
        {
            ReorderableList reorderableList = new ReorderableList(serializedObject, elements, true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = elements.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
                if(element.propertyType == SerializedPropertyType.ObjectReference)
                    list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;

                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
            return reorderableList;
        }
    }
}