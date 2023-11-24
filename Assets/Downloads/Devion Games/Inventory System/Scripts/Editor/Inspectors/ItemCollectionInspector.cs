using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
	[CustomEditor (typeof(ItemCollection), true)]
	public class ItemCollectionInspector : Editor
	{
		private SerializedProperty script;
        private SerializedProperty savebale;
	
        private SerializedProperty m_Items;
        private ReorderableList m_ItemList;

        private SerializedProperty m_Modifiers;
        private ReorderableList m_ModifierList;

		private void OnEnable ()
		{
			script = serializedObject.FindProperty ("m_Script");
            savebale = serializedObject.FindProperty("saveable");

            m_Items = serializedObject.FindProperty("m_Items");
            m_Modifiers = serializedObject.FindProperty("m_Modifiers");

            CreateItemList(serializedObject, m_Items);
        }

        private void CreateItemList(SerializedObject serializedObject, SerializedProperty elements) {
            m_ItemList = new ReorderableList(serializedObject, elements, true, true, true, true);
            m_ItemList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Items (Item, Amount)");
            };

            m_ItemList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                rect.width = rect.width - 52f;
                SerializedProperty element = elements.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);

                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                if (amounts.arraySize < m_Items.arraySize)
                {
                    for (int i = amounts.arraySize; i < m_Items.arraySize; i++)
                    {
                        amounts.InsertArrayElementAtIndex(i);
                        amounts.GetArrayElementAtIndex(i).intValue = 1;
                    }
                }
                SerializedProperty amount = amounts.GetArrayElementAtIndex(index);
                rect.x += rect.width + 2f;
                rect.width = 50f;
             
                if (EditorApplication.isPlaying)
                {
                    amount.intValue = element.objectReferenceValue != null ? (element.objectReferenceValue as Item).Stack : amount.intValue;
                }
                EditorGUI.PropertyField(rect, amount, GUIContent.none);

            };

            m_ItemList.onReorderCallbackWithDetails = (ReorderableList _, int oldIndex, int newIndex) => {
                m_Modifiers.MoveArrayElement(oldIndex, newIndex);
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                amounts.MoveArrayElement(oldIndex, newIndex);
            };

            m_ItemList.onAddCallback = (ReorderableList list) => {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                m_Modifiers.InsertArrayElementAtIndex(list.index);
            };

            m_ItemList.onRemoveCallback = (ReorderableList list) =>
            {
                m_Modifiers.DeleteArrayElementAtIndex(list.index);
                m_ModifierList = null;
                SerializedProperty amounts = serializedObject.FindProperty("m_Amounts");
                amounts.DeleteArrayElementAtIndex(list.index);

                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };

            m_ItemList.onSelectCallback = (ReorderableList list) =>
            {
                if (m_Modifiers.arraySize < m_Items.arraySize)
                {
                    for (int i = m_Modifiers.arraySize; i < m_Items.arraySize; i++)
                    {
                        m_Modifiers.InsertArrayElementAtIndex(i);
                    }
                }
                CreateModifierList("Modifiers", serializedObject, m_Modifiers.GetArrayElementAtIndex(list.index));
            };
        }

        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property) {

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

        public override void OnInspectorGUI ()
		{
            EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField (script);
            EditorGUI.EndDisabledGroup();

			serializedObject.Update ();
            EditorGUILayout.PropertyField(savebale);
			GUILayout.Space (3f);
            m_ItemList.DoLayoutList ();
            EditorGUILayout.Space();
            if (m_ModifierList != null)
                m_ModifierList.DoLayoutList();
			serializedObject.ApplyModifiedProperties ();
		}
	}
}