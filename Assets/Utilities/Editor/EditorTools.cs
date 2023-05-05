using System;
using Items;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Editor
{
    public static class EditorTools
    {
        public static ReorderableList CreateReorderable(string nameList,
                                                        SerializedProperty propertyList,
                                                        SerializedObject serializedObject,
                                                        ReorderableList.AddDropdownCallbackDelegate
                                                            onAddDropdownCallbackDelegate = null,
                                                        ReorderableList.RemoveCallbackDelegate removeCallback = null,
                                                        ReorderableList.ElementCallbackDelegate drawElementCallback =
                                                            null)

        {
            ReorderableList reorderList = new ReorderableList(serializedObject, propertyList){
                drawHeaderCallback = rect => {
                    GUIStyle style = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(rect, nameList, style);
                },
            };
            if(removeCallback != null)
                reorderList.onRemoveCallback = removeCallback;
            if(drawElementCallback != null)
                reorderList.drawElementCallback = drawElementCallback;
            if(onAddDropdownCallbackDelegate != null)
                reorderList.onAddDropdownCallback = onAddDropdownCallbackDelegate;

            return reorderList;
        }

        public static ReorderableList CreateReorderable(string nameList,
                                                        SerializedProperty propertyList,
                                                        SerializedObject serializedObject,
                                                        ReorderableList.AddCallbackDelegate addCallback = null,
                                                        ReorderableList.RemoveCallbackDelegate removeCallback = null,
                                                        ReorderableList.ElementCallbackDelegate drawElementCallback =
                                                            null)

        {
            ReorderableList reorderList = new ReorderableList(serializedObject, propertyList){
                drawHeaderCallback = rect => {
                    GUIStyle style = new GUIStyle(GUI.skin.label){ alignment = TextAnchor.MiddleCenter };
                    EditorGUI.LabelField(rect, nameList, style);
                },
            };
            if(removeCallback != null)
                reorderList.onRemoveCallback = removeCallback;
            if(drawElementCallback != null)
                reorderList.drawElementCallback = drawElementCallback;
            if(addCallback != null)
                reorderList.onAddCallback = addCallback;

            return reorderList;
        }

        public static void DeleteAsset(int index, SerializedProperty list)
        {
            Object obj = list.GetArrayElementAtIndex(index).objectReferenceValue;
            list.DeleteArrayElementAtIndex(index);
            // list.arraySize--;
            // list.GetArrayElementAtIndex(index).objectReferenceValue = null;
            AssetDatabase.RemoveObjectFromAsset(obj);
            Object.DestroyImmediate(obj, true);
            AssetDatabase.SaveAssets();
            list.serializedObject.ApplyModifiedProperties();
        }

        public static T CreateElementInstance<T>(Object targetObj,
                                                 SerializedProperty list,
                                                 SerializedObject serializedObject) where T : ScriptableObject, IName
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = $"new {typeof(T).Name}";
            AssetDatabase.AddObjectToAsset(instance, targetObj);
            AssetDatabase.SaveAssets();
            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = instance;
            serializedObject.ApplyModifiedProperties();
            return instance;
        }

        public static T Create<T>(bool displayFilePanel) where T : ScriptableObject
            => (T)Create(typeof(T), displayFilePanel);

        public static ScriptableObject Create(Type type, bool displayFilePanel)
        {
            if(displayFilePanel){
                string mPath = EditorUtility.SaveFilePanelInProject($"Create {type.Name} type",
                                                                    $"New {type.Name}.asset",
                                                                    "asset",
                                                                    $"You create a {type.Name}");
                return CreateAsset(type, mPath);
            }

            return CreateAsset(type);
        }

        public static ScriptableObject CreateAsset(Type type)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(string.IsNullOrEmpty(path))
                path = "Assets";
            else if(System.IO.Path.GetExtension(path) != "")
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)),
                                    null);
            string assetsNameAndPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/New {type.Name}.asset");
            return CreateAsset(type, assetsNameAndPath);
        }

        public static ScriptableObject CreateAsset(Type type, string path)
        {
            if(string.IsNullOrEmpty(path))
                return null;
            ScriptableObject data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
        }
    }
}