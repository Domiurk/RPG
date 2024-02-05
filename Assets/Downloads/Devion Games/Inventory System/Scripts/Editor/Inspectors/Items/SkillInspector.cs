using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Skill),true)]
    public class SkillInspector : UsableItemInspector
    {
        protected SerializedProperty m_FixedSuccessChance;
        protected SerializedProperty m_GainModifier;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;

            m_FixedSuccessChance = serializedObject.FindProperty("m_FixedSuccessChance");
            m_GainModifier = serializedObject.FindProperty("m_GainModifier");

        }

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ItemName, new GUIContent("Name"));
            EditorGUILayout.PropertyField(m_UseItemNameAsDisplayName, new GUIContent("Use name as display name"));
            m_ShowItemDisplayNameOptions.target = !m_UseItemNameAsDisplayName.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowItemDisplayNameOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_ItemDisplayName);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Icon);
            EditorGUILayout.PropertyField(m_Description);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Category);
            EditorGUILayout.PropertyField(m_FixedSuccessChance);
            EditorGUILayout.PropertyField(m_GainModifier);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_BuyPrice, new GUIContent("Price"));
            EditorGUILayout.PropertyField(m_BuyCurrency, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            DrawCooldownGUI();
            ActionGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}