using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Text_Renaming.Scripts.Runtime
{
    public class AddPrefix : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private string _prefix;
        [SerializeField] private string _suffix;
        [SerializeField, Min(0)] private int _childIndexToStart = 7;
        private readonly List<Transform> bones = new();

        private static IEnumerable<Transform> GetTransform(GameObject obj)
        {
            var objects = obj.GetComponentsInChildren<Transform>();

            foreach(Transform o in objects){
                yield return o;
            }
        }

        private void AddBoneToList()
        {
            bones.Clear();

            if(_prefab == null){
                Debug.Log($"Start Renaming {name} prefab");
                _prefab = this.gameObject;
            }

            bones.AddRange(_prefab.GetComponentsInChildren<Transform>());
        }

        public void AddPrefixName()
        {
            // AddBoneToList();

            int count = 0;

            foreach(Transform o in GetTransform(_prefab)){
                o.name = _prefix + o.name + _suffix;
            }

            Debug.Log($"You Successful Rename {count} children.");
        }
    }

    [CustomEditor(typeof(AddPrefix))]
    public class AddPrefixInspector : Editor
    {
        private SerializedProperty _prefab;
        private SerializedProperty _prefix;
        private SerializedProperty _suffix;
        private SerializedProperty _childIndexToStart;
        private AddPrefix script;

        private void OnEnable()
        {
            if(script == null)
                script = (AddPrefix)target;
            _prefab = serializedObject.FindProperty("_prefab");
            _prefix = serializedObject.FindProperty("_prefix");
            _suffix = serializedObject.FindProperty("_suffix");
            _childIndexToStart = serializedObject.FindProperty("_childIndexToStart");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Add Prefix", new GUIStyle(GUI.skin.label)
                                { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold });
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(_prefab);
            EditorGUILayout.PropertyField(_prefix);
            EditorGUILayout.PropertyField(_suffix);
            EditorGUILayout.PropertyField(_childIndexToStart, new GUIContent("Start Rename Index"));
            string buttonName = _prefix.stringValue != string.Empty &&
                                _suffix.stringValue != string.Empty ? "Add Prefix and Suffix" :
                                _prefix.stringValue != string.Empty ? "Add Prefix" :
                                _suffix.stringValue != string.Empty ? "Add Suffix" : "Write Prefix or Suffix";
            GUI.enabled = _prefix.stringValue != string.Empty || _suffix.stringValue != string.Empty;

            if(GUILayout.Button(buttonName))
                ((AddPrefix)target).AddPrefixName();

            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            if(GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }
}