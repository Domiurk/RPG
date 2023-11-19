using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Text_Renaming.Scripts.Runtime
{
    public class AddPrefix : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField, Min(0)] private int childIndexToStart = 7;
        [SerializeField, HideInInspector] private List<Transform> bones = new();

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

            if(prefab == null){
                Debug.Log($"Start Renaming {name} prefab");
                prefab = this.gameObject;
            }

            bones.AddRange(prefab.GetComponentsInChildren<Transform>());
        }

        public void AddPrefixName()
        {
            // AddBoneToList();

            int count = 0;

            foreach(Transform o in GetTransform(prefab)){
                o.name = prefix + o.name + suffix;
            }

            Debug.Log($"You Successful Rename {count} children.");
        }
    }

    [CustomEditor(typeof(AddPrefix))]
    public class AddPrefixInspector : Editor
    {
        private SerializedProperty prefab;
        private SerializedProperty prefix;
        private SerializedProperty suffix;
        private SerializedProperty childIndexToStart;
        private AddPrefix script;

        private void OnEnable()
        {
            if(script == null)
                script = (AddPrefix)target;
            prefab = serializedObject.FindProperty("_prefab");
            prefix = serializedObject.FindProperty("_prefix");
            suffix = serializedObject.FindProperty("_suffix");
            childIndexToStart = serializedObject.FindProperty("_childIndexToStart");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Add Prefix", new GUIStyle(GUI.skin.label)
                                { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold });
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(prefix);
            EditorGUILayout.PropertyField(suffix);
            EditorGUILayout.PropertyField(childIndexToStart, new GUIContent("Start Rename Index"));
            string buttonName = prefix.stringValue != string.Empty &&
                                suffix.stringValue != string.Empty ? "Add Prefix and Suffix" :
                                prefix.stringValue != string.Empty ? "Add Prefix" :
                                suffix.stringValue != string.Empty ? "Add Suffix" : "Write Prefix or Suffix";
            GUI.enabled = prefix.stringValue != string.Empty || suffix.stringValue != string.Empty;

            if(GUILayout.Button(buttonName))
                ((AddPrefix)target).AddPrefixName();

            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            if(GUI.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }
}