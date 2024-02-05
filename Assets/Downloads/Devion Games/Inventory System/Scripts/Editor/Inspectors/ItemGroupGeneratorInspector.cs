using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ItemGroupGenerator),true)]
    public class ItemGroupGeneratorInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_From;
        private SerializedProperty m_Filters;
        private SerializedProperty m_MinStack;
        private SerializedProperty m_MaxStack;
        private SerializedProperty m_MinAmount;
        private SerializedProperty m_MaxAmount;
        private SerializedProperty m_Chance;

        private ReorderableList m_FilterList;

        private SerializedProperty m_Modifiers;
        private ReorderableList m_ModifierList;


        protected virtual void OnEnable()
        {
            m_Script = serializedObject.FindProperty("m_Script");
            m_From = serializedObject.FindProperty("m_From");
            m_Filters = serializedObject.FindProperty("m_Filters");
            m_MinStack = serializedObject.FindProperty("m_MinStack");
            m_MaxStack = serializedObject.FindProperty("m_MaxStack");
            m_MinAmount = serializedObject.FindProperty("m_MinAmount");
            m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
            m_Chance = serializedObject.FindProperty("m_Chance");
            m_Modifiers = serializedObject.FindProperty("m_Modifiers");

            CreateModifierList("Modifiers", serializedObject, m_Modifiers);

            m_FilterList = new ReorderableList(serializedObject, m_Filters, true, true, true, true);
            m_FilterList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Filters");
            };


            m_FilterList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                SerializedProperty element = m_FilterList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, (element.objectReferenceValue as INameable).Name+" ("+element.objectReferenceValue.GetType().Name+")");
            };

  
            m_FilterList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };

            m_FilterList.onAddDropdownCallback = (Rect rect, ReorderableList _) => {

                GenericMenu menu = new GenericMenu();
                string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    ItemDatabase database = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase)) as ItemDatabase;
                    for (int j = 0; j < database.categories.Count; j++) {
                        Category category = database.categories[j];
                        menu.AddItem(new GUIContent(database.name + "/Category/" + database.categories[j].Name), false, () => {
                            serializedObject.Update();
                            m_Filters.InsertArrayElementAtIndex(m_Filters.arraySize);
                            SerializedProperty property = m_Filters.GetArrayElementAtIndex(m_Filters.arraySize - 1);
                            property.objectReferenceValue = category;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                    for (int j = 0; j < database.raritys.Count; j++)
                    {
                        Rarity rarity = database.raritys[j];
                        menu.AddItem(new GUIContent(database.name + "/Rarity/" + database.raritys[j].Name), false, () => {
                            serializedObject.Update();
                            m_Filters.InsertArrayElementAtIndex(m_Filters.arraySize);
                            SerializedProperty property = m_Filters.GetArrayElementAtIndex(m_Filters.arraySize - 1);
                            property.objectReferenceValue = rarity;
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                menu.DropDown(rect);
            };

        }



        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property)
        {

            m_ModifierList = new ReorderableList(serializedObject, property.FindPropertyRelative("modifiers"), true, true, true, true);
            m_ModifierList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };
            m_ModifierList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = m_ModifierList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            m_ModifierList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_From);
     
            m_FilterList.DoLayoutList();
            EditorGUILayout.PropertyField(m_MinStack);
            EditorGUILayout.PropertyField(m_MaxStack);
            EditorGUILayout.PropertyField(m_MinAmount);
            EditorGUILayout.PropertyField(m_MaxAmount);
            EditorGUILayout.PropertyField(m_Chance);
            EditorGUILayout.Space();
            m_ModifierList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        

    }
}