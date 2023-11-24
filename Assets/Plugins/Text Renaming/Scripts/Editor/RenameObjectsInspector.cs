using Text_Renaming.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace Text_Renaming.Scripts.Editor
{
    [CustomEditor(typeof(RenameObjects))]
    public class RenameObjectsInspector : UnityEditor.Editor
    {
        private RenameObjects script;

        private SerializedProperty nameScript;

        private void OnEnable()
        {
            if(target == null){
                Debug.Log("Target is null");
                return;
            }

            script = (RenameObjects)target;
            nameScript = serializedObject.FindProperty("m_Script");
        }

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();

            DrawMain();
        }

        private void ScriptGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(nameScript);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawMain()
        {
            GUILayout.Label("Rename Mixamo",
                            new GUIStyle(GUI.skin.label){
                                alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold,
                                fontSize = 20
                            });

            GUILayout.BeginVertical(GUI.skin.box);
            script.Object = (GameObject)EditorGUILayout.ObjectField("Prefab", script.Object, typeof(GameObject), true);
            script.Prefix =
                EditorGUILayout.TextField(new GUIContent("Prefix", "Text which want to replace"), script.Prefix);
            script.Change =
                EditorGUILayout
                    .TextField(new GUIContent("Change", "The Text you want to replace(if Empty then Remove)"),
                               script.Change);
            bool enableRename = script.Prefix != string.Empty;
            GUIContent buttonName = new GUIContent(enableRename ? "Rename Children" : "Write Prefix");
            GUI.enabled = enableRename;
            if(GUILayout.Button(buttonName))
                script.Rename();

            GUILayout.EndVertical();

            if(GUI.changed){
                const string renameScript = "Rename Mixamo Script";
                EditorUtility.SetDirty(script);
                Undo.RecordObject(script, renameScript);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}