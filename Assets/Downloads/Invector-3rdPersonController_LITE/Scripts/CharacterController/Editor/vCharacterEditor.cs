using Invector.vCharacterController;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(vThirdPersonMotor), true)]
public class vCharacterEditor : Editor
{
    GUISkin skin;
    SerializedObject character;
    bool showWindow;
    
    public override void OnInspectorGUI()
    {
        vThirdPersonMotor motor = (vThirdPersonMotor)target;

        if (!motor) return;

        GUILayout.BeginVertical("BASIC CONTROLLER", GUI.skin.window);

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        base.OnInspectorGUI();

        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}