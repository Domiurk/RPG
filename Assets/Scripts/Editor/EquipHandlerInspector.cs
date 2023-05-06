using Runtime.Items;
using Runtime.Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EquipHandler))]
    public class EquipHandlerInspector : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _itemDatabase;
        private SerializedProperty _transforms;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _itemDatabase = serializedObject.FindProperty(EquipHandler.PropItemDatabase);
            _transforms = serializedObject.FindProperty(EquipHandler.PropBoneTransforms);
        }

        private void DrawScript()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;
        }

        private SerializedProperty _bones;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();

            EditorGUILayout.PropertyField(_itemDatabase);
            if(_itemDatabase.objectReferenceValue == null)
                return;

            SerializedObject database = new SerializedObject(_itemDatabase.objectReferenceValue);
            _bones = database.FindProperty(ItemDatabase.PropBones);
            
            if(_transforms.arraySize != _bones.arraySize)
                _transforms.arraySize = _bones.arraySize;
            DrawList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawList()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            for(int i = 0; i < _bones.arraySize; i++){
                SerializedProperty bone = _bones.GetArrayElementAtIndex(i);
                SerializedProperty transform = _transforms.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical();
                Rect rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(((Bone)bone.objectReferenceValue)?.Name));
                transform.objectReferenceValue =
                    EditorGUI.ObjectField(rect,transform.objectReferenceValue, typeof(Transform), true);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }
}