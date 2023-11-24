using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ItemGenerator),true)]
    public class ItemGeneratorInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_MaxAmount;
        private SerializedProperty m_ItemGeneratorData;
        private ReorderableList m_ItemGeneratorDataList;

        private ReorderableList m_ModifierList;

        protected virtual void OnEnable() {
            m_Script = serializedObject.FindProperty("m_Script");
            m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
            m_ItemGeneratorData = serializedObject.FindProperty("m_ItemGeneratorData");
            m_ItemGeneratorDataList = new ReorderableList(serializedObject, m_ItemGeneratorData, true, true, true, true)
            {
                drawHeaderCallback = DrawGeneratorDataHeader,
                drawElementCallback = DrawGeneratorData,
                onSelectCallback = SelectGeneratorData,
                onAddCallback = AddGeneratorData,
            };


            int index = EditorPrefs.GetInt("GeneratorIndex" + target.GetInstanceID().ToString(), -1);
            if (m_ItemGeneratorDataList.count > index)
            {
                m_ItemGeneratorDataList.index = index;
                SelectGeneratorData(m_ItemGeneratorDataList);
               if(index > -1)
                    CreateModifierList("Modifiers", serializedObject, m_ItemGeneratorData.GetArrayElementAtIndex(index).FindPropertyRelative("modifiers")) ;
            }

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
            EditorGUILayout.PropertyField(m_MaxAmount);
            if (m_MaxAmount.intValue > m_ItemGeneratorData.arraySize)
            {
                m_MaxAmount.intValue=m_ItemGeneratorData.arraySize;
            }
            if (m_MaxAmount.intValue < 0 )
            {
                m_MaxAmount.intValue = 0;
            }

            m_ItemGeneratorDataList.DoLayoutList();

            if (m_ItemGeneratorDataList.index != -1)
            {
                GUILayout.Space(5f);
                DrawSelectedGeneratorData(m_ItemGeneratorData.GetArrayElementAtIndex(m_ItemGeneratorDataList.index));
            }
            serializedObject.ApplyModifiedProperties();
            
        }

        private void DrawGeneratorDataHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Items");
        }

        private void DrawGeneratorData(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2f;
            SerializedProperty element = m_ItemGeneratorData.GetArrayElementAtIndex(index);
            SerializedProperty item = element.FindPropertyRelative("item");
            if (item.objectReferenceValue != null)
            {
                SerializedObject obj = new SerializedObject(item.objectReferenceValue);
                SerializedProperty itemName = obj.FindProperty("m_ItemName");
                GUI.Label(rect, itemName.stringValue);
            }
            else {
                GUI.Label(rect, "Null");
            }
          
        }

        private void AddGeneratorData(ReorderableList list)
        {
            list.serializedProperty.serializedObject.Update();
            list.serializedProperty.arraySize++;
            list.index = list.serializedProperty.arraySize - 1;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private void SelectGeneratorData(ReorderableList list)
        {
            EditorPrefs.SetInt("GeneratorIndex" + target.GetInstanceID().ToString(), list.index);
            if(list.index > -1)
                CreateModifierList("Modifiers", serializedObject, m_ItemGeneratorData.GetArrayElementAtIndex(list.index).FindPropertyRelative("modifiers"));
        }

        private void DrawSelectedGeneratorData(SerializedProperty element)
        {
            EditorGUILayout.PropertyField(element.FindPropertyRelative("item"));

            SerializedProperty minStack = element.FindPropertyRelative("minStack");
            EditorGUILayout.PropertyField(minStack);

            if (minStack.intValue < 1) {
                minStack.intValue = 1;
            }
            SerializedProperty maxStack = element.FindPropertyRelative("maxStack");
            EditorGUILayout.PropertyField(maxStack);
            if (maxStack.intValue < 1) {
                maxStack.intValue = 1;
            }
            EditorGUILayout.PropertyField(element.FindPropertyRelative("chance"));

            EditorGUILayout.Space();
            if (m_ModifierList != null)
                m_ModifierList.DoLayoutList();
        }

    }
}