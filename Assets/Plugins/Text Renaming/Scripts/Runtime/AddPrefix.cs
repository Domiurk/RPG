using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Text_Renaming.Scripts.Runtime
{
    public class AddPrefix : MonoBehaviour
    {
#if UNITY_EDITOR
        public static readonly string PrefabField = nameof(prefab);
        public static string PrefixField = nameof(prefix);
        public static readonly string SuffixField = nameof(suffix);
#endif

        [SerializeField] private GameObject prefab;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField, HideInInspector] private List<Transform> bones = new();

        private static IEnumerable<Transform> GetTransform(GameObject obj)
        {
            Transform[] objects = obj.GetComponentsInChildren<Transform>();

            foreach(Transform o in objects){
                yield return o;
            }
        }

        public void AddPrefixName()
        {
            const int count = 0;

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
            prefab = serializedObject.FindProperty(AddPrefix.PrefabField);
            prefix = serializedObject.FindProperty(AddPrefix.PrefabField);
            suffix = serializedObject.FindProperty(AddPrefix.SuffixField);
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