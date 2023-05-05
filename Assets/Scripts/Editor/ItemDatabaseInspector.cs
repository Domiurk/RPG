using Items;
using Runtime.Items;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utilities.Editor;

namespace Editor
{
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseInspector : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _items;
        private SerializedProperty _bones;

        private ReorderableList _itemsList;
        private ReorderableList _bonesList;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _items = serializedObject.FindProperty(ItemDatabase.PropItems);
            _bones = serializedObject.FindProperty(ItemDatabase.PropBones);
            _itemsList = EditorTools.CreateReorderable("Items", _items, serializedObject, 
                                                       AddDropdownCallbackItems,
                                                       l => EditorTools.DeleteAsset(l.index, _items),
                                                       DrawElementCallbackItems);
            _bonesList = EditorTools.CreateReorderable("Bones", _bones, serializedObject, 
                                                       AddCallbackBones,
                                                       RemoveCallbackBones, 
                                                       DrawElementCallbackBones);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScript();

            EditorGUILayout.PropertyField(_items);
            _itemsList.DoLayoutList();

            EditorGUILayout.PropertyField(_bones);
            EditorGUILayout.PrefixLabel(_bones.arraySize.ToString(), GUI.skin.box);
            _bonesList.DoLayoutList();
        }

        private void AddDropdownCallbackItems(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Static Item"), false,
                         () => { EditorTools.CreateElementInstance<StaticItem>(target, _items, serializedObject); });
            menu.AddItem(new GUIContent("Usable Item"), false,
                         () => { EditorTools.CreateElementInstance<UsableItem>(target, _items, serializedObject); });
            menu.AddItem(new GUIContent("Equip Item"), false,
                         () => { EditorTools.CreateElementInstance<EquipItem>(target, _items, serializedObject); });
            menu.ShowAsContext();
        }

        private void DrawElementCallbackItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            if(_items.GetArrayElementAtIndex(index).objectReferenceValue == null){
                Debug.Log(index + " in item == null");
                return;
            }

            SerializedObject item = new SerializedObject(_items.GetArrayElementAtIndex(index).objectReferenceValue);
            SerializedProperty nameItem = item.FindProperty(Item.PropName);
            SerializedProperty iconItem = item.FindProperty(Item.PropIcon);
            SerializedProperty prefabItem = item.FindProperty(StaticItem.PropPrefab);
            SerializedProperty offsetItem = item.FindProperty(EquipItem.PropOffset);
            SerializedProperty typeEquipItem = item.FindProperty(EquipItem.PropTypeEquip);

            item.Update();

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2f;
            Rect toggleRect = new Rect(rect.x + rect.height, rect.y, rect.height, rect.height);
            Rect propertyRect = new Rect(toggleRect.x, rect.y, rect.width - toggleRect.width,
                                         rect.height);
            EditorGUI.LabelField(propertyRect,
                                 string.IsNullOrEmpty(nameItem.stringValue)
                                     ? $"new {item.targetObject.GetType().Name}"
                                     : nameItem.stringValue);

            if(_itemsList.IsSelected(index)){
                EditorGUILayout.PropertyField(nameItem);
                EditorGUILayout.PropertyField(iconItem);

                if(prefabItem != null)
                    EditorGUILayout.PropertyField(prefabItem);

                if(offsetItem != null && typeEquipItem != null){
                    EditorGUILayout.PropertyField(offsetItem);
                    EditorGUILayout.PropertyField(typeEquipItem);
                }
            }

            if(item.ApplyModifiedProperties() && item.targetObject.name != nameItem.stringValue &&
               !string.IsNullOrEmpty(nameItem.stringValue)){
                item.targetObject.name = nameItem.stringValue;
                AssetDatabase.SaveAssets();
            }
        }

        private void RemoveCallbackBones(ReorderableList list)
            => EditorTools.DeleteAsset(list.index, _bones);

        private void AddCallbackBones(ReorderableList list)
            => EditorTools.CreateElementInstance<Bone>(target, _bones, serializedObject);

        private void DrawElementCallbackBones(Rect rect, int index, bool isActive, bool isFocused)
        {
            if(_bones.arraySize <= 0 && _bones.GetArrayElementAtIndex(index) == null){
                Debug.Log(index + "Bone none");
                return;
            }

            SerializedObject bone = new SerializedObject(_bones.GetArrayElementAtIndex(index).objectReferenceValue);
            SerializedProperty nameBone = bone.FindProperty(Bone.PropName);
            bone.Update();
            EditorGUI.PropertyField(rect, nameBone);
            if(bone.ApplyModifiedProperties() && bone.targetObject.name != nameBone.stringValue &&
               !string.IsNullOrEmpty(nameBone.stringValue)){
                bone.targetObject.name = $"Bone {nameBone.stringValue}";
                AssetDatabase.SaveAssets();
            }
        }
        
        private void DrawScript()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;
        }
    }
}