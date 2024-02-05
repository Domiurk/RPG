
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using DevionGames.UIWidgets;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;


namespace DevionGames.InventorySystem
{
   [CustomEditor(typeof(ItemContainer),true)]
    public class ItemContainerInspector : UIWidgetInspector
    {

        private SerializedProperty m_UseButton;
        private SerializedProperty m_DynamicContainer;
        private SerializedProperty m_SlotPrefab;
        private SerializedProperty m_SlotParent;
        private AnimBool m_ShowDynamicContainer;

        private SerializedProperty m_UseReferences;

        private SerializedProperty m_MoveUsedItems;
        private SerializedProperty m_MoveItemConditions;
        private AnimBool m_ShowMoveUsedItems;


        private ReorderableList m_MoveItemConditionList;

        private SerializedProperty m_Restrictions;

        private string[] m_PropertiesToExcludeForDefaultInspector;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_UseButton = serializedObject.FindProperty("useButton");
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

            m_UseReferences = serializedObject.FindProperty("m_UseReferences");

            m_MoveUsedItems = serializedObject.FindProperty("m_MoveUsedItem");
            m_MoveItemConditions = serializedObject.FindProperty("moveItemConditions");
            m_ShowMoveUsedItems = new AnimBool(m_MoveUsedItems.boolValue);
            m_ShowMoveUsedItems.valueChanged.AddListener(Repaint);

            m_MoveItemConditionList = new ReorderableList(serializedObject, m_MoveItemConditions, true, true, true, true);
            m_MoveItemConditionList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Move Conditions (Window, Requires Visibility)");
            };

            m_MoveItemConditionList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                SerializedProperty element = m_MoveItemConditionList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width - 22f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("window"), GUIContent.none);
                rect.x += rect.width + 7f;
                rect.width = 20f;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("requiresVisibility"), GUIContent.none);
            };

            m_Restrictions = serializedObject.FindProperty("restrictions");
           
            for (int i = 0; i < m_Restrictions.arraySize; i++) {
                m_Restrictions.GetArrayElementAtIndex(i).objectReferenceValue.hideFlags = HideFlags.HideInInspector;
            }


            m_PropertiesToExcludeForDefaultInspector = new [] {
                m_UseButton.propertyPath,
                m_DynamicContainer.propertyPath,
                m_SlotParent.propertyPath,
                m_SlotPrefab.propertyPath,
                m_UseReferences.propertyPath,
                m_MoveUsedItems.propertyPath,
                m_MoveItemConditions.propertyPath,
                m_Restrictions.propertyPath
            };
        }

        protected virtual void OnDisable()
        {
            if (m_ShowMoveUsedItems != null){
                m_ShowMoveUsedItems.valueChanged.RemoveListener(Repaint);
            }
        }


        private void DrawInspector()
        {
            EditorGUILayout.PropertyField(m_UseButton);
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
            ItemCollection collection = (target as ItemContainer).GetComponent<ItemCollection>();
            EditorGUI.BeginDisabledGroup(collection != null);
            if (collection != null) {
                EditorGUILayout.HelpBox("You can't use references with an ItemCollection component.", MessageType.Warning);
                m_UseReferences.boolValue = false;
            }
            EditorGUILayout.PropertyField(m_UseReferences);
            
            EditorGUI.EndDisabledGroup();

        
            DrawTypePropertiesExcluding(typeof(ItemContainer),m_PropertiesToExcludeForDefaultInspector);

            EditorGUILayout.PropertyField(m_MoveUsedItems);
            m_ShowMoveUsedItems.target = m_MoveUsedItems.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowMoveUsedItems.faded))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(16f);
                GUILayout.BeginVertical();
                m_MoveItemConditionList.DoLayoutList();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            if (EditorTools.RightArrowButton(new GUIContent("Restrictions", "Container Restrictions")))
            {
                AssetWindow.ShowWindow("Container Restrictions", m_Restrictions);
            }
        }

    }
}