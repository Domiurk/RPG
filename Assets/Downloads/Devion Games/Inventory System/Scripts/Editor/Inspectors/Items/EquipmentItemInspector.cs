using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(EquipmentItem), true)]
    public class EquipmentItemInspector :  UsableItemInspector
    {
        private ReorderableList regionList;
        private SerializedProperty m_OverrideEquipPrefab;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            m_OverrideEquipPrefab = serializedObject.FindProperty("m_OverrideEquipPrefab");

            regionList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Region"), true, true, true, true);
            regionList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Equipment Region");
            };
            regionList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                SerializedProperty element = regionList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };
        }


        private void DrawInspector() {
            GUILayout.Space(5f);
            GUILayout.Label("Equipment:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_OverrideEquipPrefab);
            EditorGUILayout.HelpBox("The equipment region defines where the item should be equiped to. Use Left Hand and Right Hand for two-handed weapons.", MessageType.Info);
            regionList.DoLayoutList();
        }

    }
}