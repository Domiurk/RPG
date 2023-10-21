using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Invector.vCharacterController.vThirdPersonInput), true)]
public class vThirdPersonInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical("INPUT MANAGER", GUI.skin.window);

        EditorGUILayout.BeginVertical();

        base.OnInspectorGUI();

        GUILayout.Space(10);

        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        GUILayout.Space(2);
    }
}