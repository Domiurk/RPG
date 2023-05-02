using Items;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseInspector : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _items;
        private SerializedProperty _names;

        private ReorderableList _reorderableList;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _items = serializedObject.FindProperty(ItemDatabase.PropItems);
            _names = serializedObject.FindProperty(ItemDatabase.PropNames);
            CreateList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScript();

            _reorderableList.DoLayoutList();
            EditorGUILayout.PropertyField(_names);

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateList(string nameList = "Items")
        {
            _reorderableList = new ReorderableList(serializedObject, _items){
                drawHeaderCallback = rect => {
                    GUIStyle style = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(rect, nameList, style);
                },
                drawElementCallback = DrawElementCallback,
                onRemoveCallback = list => { DeleteAsset(list.index); },
                onAddDropdownCallback = OnAddDropdownCallback,
            };
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Static Item"), false, CreateElement<StaticItem>);
            menu.AddItem(new GUIContent("Usable Item"), false, CreateElement<UsableItem>);
            menu.AddItem(new GUIContent("Equip Item"), false, CreateElement<EquipItem>);
            menu.ShowAsContext();
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
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
                                 string.IsNullOrEmpty(nameItem.stringValue) ? "new Item" : nameItem.stringValue);

            if(_reorderableList.IsSelected(index)){
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

            // item.ApplyModifiedProperties();
        }

        private void CreateElement<T>() where T : Item, IName
        {
            Item instance = ScriptableObject.CreateInstance<T>();
            instance.name = "new Item";
            AssetDatabase.AddObjectToAsset(instance, target);
            AssetDatabase.SaveAssets();
            _items.arraySize++;
            _items.GetArrayElementAtIndex(_items.arraySize - 1).objectReferenceValue = instance;
            serializedObject.ApplyModifiedProperties();
        }

        private void DeleteAsset(int index)
        {
            Object obj = _items.GetArrayElementAtIndex(index).objectReferenceValue;
            AssetDatabase.RemoveObjectFromAsset(obj);
            Object.DestroyImmediate(obj, true);
            _items.GetArrayElementAtIndex(index).objectReferenceValue = null;
            _items.DeleteArrayElementAtIndex(index);
            AssetDatabase.SaveAssets();
        }

        private void DrawScript()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_script);
            GUI.enabled = true;
        }
    }
}