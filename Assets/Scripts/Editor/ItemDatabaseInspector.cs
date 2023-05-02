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
            _items = serializedObject.FindProperty(ItemDatabase.PropNameItems);
            _names = serializedObject.FindProperty(ItemDatabase.PropNameNames);
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
                onAddCallback = OnAddCallback,
                onRemoveCallback = list => { DeleteAsset(list.index); }
            };
        }

        private void OnAddCallback(ReorderableList list)
        {
            _items.arraySize++;
            _items.GetArrayElementAtIndex(_items.arraySize - 1).objectReferenceValue = CreateAsset();
        }

        private Item CreateAsset()
        {
            var instance = CreateInstance<Item>();
            instance.name = "new " + nameof(Item);
            AssetDatabase.AddObjectToAsset(instance, target);
            AssetDatabase.SaveAssets();
            return instance;
        }

        private void DeleteAsset(int index)
        {
            Object obj = _items.GetArrayElementAtIndex(index).objectReferenceValue;
            _items.GetArrayElementAtIndex(index).objectReferenceValue = null;
            _items.DeleteArrayElementAtIndex(index);
            AssetDatabase.RemoveObjectFromAsset(obj);
            Object.DestroyImmediate(obj, true);
            AssetDatabase.SaveAssets();
        }

        private bool _foldout;

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedObject item = new SerializedObject(_items.GetArrayElementAtIndex(index).objectReferenceValue);
            SerializedProperty nameItem = item.FindProperty(Item.PropName);
            SerializedProperty iconItem = item.FindProperty(Item.PropIcon);
            SerializedProperty prefabItem = item.FindProperty(Item.PropPrefab);
            SerializedProperty offsetItem = item.FindProperty(Item.PropOffset);
            SerializedProperty typeEquipItem = item.FindProperty(Item.PropTypeEquip);

            item.Update();

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2f;
            Rect toggleRect = new Rect(rect.x + rect.height, rect.y, rect.height, rect.height);
            Rect propertyRect = new Rect(toggleRect.x, rect.y, rect.width - toggleRect.width,
                                         rect.height);
            EditorGUI.LabelField(propertyRect,
                                 string.IsNullOrEmpty(nameItem.stringValue) ? "new Item" : nameItem.stringValue);

            _foldout = EditorGUI.Foldout(toggleRect, _foldout, "");

            if(_foldout && isActive && _reorderableList.index == index){
                EditorGUILayout.PropertyField(nameItem);
                EditorGUILayout.PropertyField(iconItem);
                EditorGUILayout.PropertyField(prefabItem);
                EditorGUILayout.PropertyField(offsetItem);
                EditorGUILayout.PropertyField(typeEquipItem);
            }

            if(item.ApplyModifiedProperties() && item.targetObject.name != nameItem.stringValue &&
               !string.IsNullOrEmpty(nameItem.stringValue)){
                item.targetObject.name = nameItem.stringValue;
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