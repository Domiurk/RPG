using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(RarityModifier),true)]
    public class RarityModifierInspector : Editor
    {
        protected SerializedProperty m_Rarities;
        protected ReorderableList m_RarityList;

        protected virtual void OnEnable()
        {
            m_Rarities = serializedObject.FindProperty("m_Rarities");
            m_RarityList = new ReorderableList(serializedObject, m_Rarities, true, true, true, true);
            m_RarityList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Possible Rarities");
            };
            m_RarityList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight)*0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y =rect.y+verticalOffset;
                SerializedProperty element = m_RarityList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            m_RarityList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_RarityList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

    }
}