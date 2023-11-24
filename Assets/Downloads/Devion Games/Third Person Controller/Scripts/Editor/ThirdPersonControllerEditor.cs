using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;

namespace DevionGames
{
    [CustomEditor(typeof(ThirdPersonController))]
    public class ThirdPersonControllerEditor : Editor
    {
        private ThirdPersonController m_Controller;
        private ReorderableList m_MotionList;
        private SerializedProperty m_Motions;

        private SerializedProperty m_Script;
        private int m_RenameMotionIndex = -1;
        private int m_ClickCount;

        private GameObject m_GameObject;
        private List<MotionState> m_NotReferencedMotions;

        private void OnEnable()
        {
            m_Controller = target as ThirdPersonController;
            m_GameObject = m_Controller.gameObject;
            m_Script = serializedObject.FindProperty("m_Script");
            m_Motions = serializedObject.FindProperty("Motions");

            m_MotionList = new ReorderableList(serializedObject, m_Motions, true, true, true, true){
                drawHeaderCallback = DrawMotionHeader,
                drawElementCallback = DrawMotion,
                onAddDropdownCallback = AddMotion,
                onRemoveCallback = RemoveMotion,
                onSelectCallback = SelectMotion,
                drawElementBackgroundCallback = DrawMotionBackground
            };
            int motionIndex = EditorPrefs.GetInt("MotionIndex" + target.GetInstanceID().ToString(), -1);

            if(m_MotionList.count > motionIndex){
                m_MotionList.index = motionIndex;
                SelectMotion(m_MotionList);
            }

            MotionState[] states = m_Controller.GetComponents<MotionState>();
            foreach(MotionState state in states)
                state.hideFlags = HideFlags.HideInInspector;

            m_NotReferencedMotions = new List<MotionState>();
            foreach(MotionState state in states)
                if(!m_Controller.Motions.Contains(state))
                    m_NotReferencedMotions.Add(state);

            for(int i = 0; i < m_Controller.Motions.Count; i++){
                if(m_Controller.Motions[i] != null){
                    if(m_Controller.Motions[i].gameObject != m_GameObject){
                        if(ComponentUtility.CopyComponent(m_Controller.Motions[i])){
                            MotionState component =
                                m_GameObject.AddComponent(m_Controller.Motions[i].GetType()) as MotionState;
                            ComponentUtility.PasteComponentValues(component);
                            m_Controller.Motions[i] = component;
                        }
                    }
                }
            }

            EditorApplication.playModeStateChanged += PlayModeState;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeState;
        }

        bool playModeStateChange;

        private void PlayModeState(PlayModeStateChange state)
        {
            playModeStateChange = state is PlayModeStateChange.ExitingEditMode 
                                      or PlayModeStateChange.EnteredEditMode
                                      or PlayModeStateChange.ExitingPlayMode;
        }

        private void OnDestroy()
        {
            if(m_GameObject != null && target == null && !playModeStateChange){
                MotionState[] states = m_GameObject.GetComponents<MotionState>();

                foreach(MotionState state in states)
                    DestroyImmediate(state);

                Debug.LogWarning("ThirdPersonController component removed, cleaning up motions.");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            bool enabled = GUI.enabled;
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = enabled;
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "Motions");
            GUILayout.Space(10f);

            if(m_NotReferencedMotions.Count > 0){
                EditorGUILayout
                    .HelpBox("There are unreferenced motions! You should add them to the motions list or remove.",
                             MessageType.Warning);
                EditorGUILayout.BeginHorizontal();

                if(GUILayout.Button("Add")){
                    m_Controller.Motions.AddRange(m_NotReferencedMotions);
                    EditorUtility.SetDirty(m_Controller);
                    OnDisable();
                    OnEnable();
                }

                if(GUILayout.Button("Remove")){
                    for(int i = 0; i < m_NotReferencedMotions.Count; i++){
                        DestroyImmediate(m_NotReferencedMotions[i]);
                    }

                    EditorUtility.SetDirty(m_Controller);
                    OnDisable();
                    OnEnable();
                }

                EditorGUILayout.EndHorizontal();
            }

            if(m_MotionList != null){
                GUILayout.Space(3f);
                m_MotionList.DoLayoutList();
                GUILayout.Space(15f);

                if(m_MotionList.index != -1){
                    DrawSelectedMotion(m_Motions.GetArrayElementAtIndex(m_MotionList.index).objectReferenceValue as
                                           MotionState);
                }
            }

            if(EditorGUI.EndChangeCheck()){
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawMotionHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Motions");
        }

        private void DrawMotionBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            Color color = GUI.color;

            if(m_Controller.Motions != null){
                for(int i = 0; i < m_Controller.Motions.Count; i++){
                    MotionState motion = m_Controller.Motions[i];

                    if(i == index){
                        if(motion.IsActive){
                            GUI.color = Color.green * 0.8f;
                            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, true, false, true);
                        }
                        else{
                            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused,
                             true);
                        }
                    }

                    GUI.color = color;
                }
            }
        }

        private void DrawMotion(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2f;
            SerializedProperty element = m_Motions.GetArrayElementAtIndex(index);

            SerializedObject obj = new SerializedObject(element.objectReferenceValue);
            obj.Update();
            SerializedProperty friendlyName = obj.FindProperty("m_FriendlyName");

            if(string.IsNullOrEmpty(friendlyName.stringValue)){
                friendlyName.stringValue = element.objectReferenceValue.GetType().Name;
            }

            SerializedProperty enabled = obj.FindProperty("m_Enabled");
            Rect rect1 = rect;
            rect1.width = 20f;
            rect1.x += rect.width - 20f;

            if(index == m_RenameMotionIndex){
                string before = friendlyName.stringValue;
                GUI.SetNextControlName("RenameMotionField");
                rect.height = rect.height - 4f;
                rect.width -= 22f;
                string after = EditorGUI.TextField(rect, friendlyName.stringValue);

                if(before != after){
                    friendlyName.stringValue = after;
                }
            }
            else{
                GUI.Label(rect, friendlyName.stringValue);
            }

            enabled.boolValue = EditorGUI.Toggle(rect1, enabled.boolValue);
            obj.ApplyModifiedProperties();

            Event currentEvent = Event.current;

            switch(currentEvent.rawType){
                case EventType.MouseDown:
                    if(rect.Contains(currentEvent.mousePosition) && index == m_MotionList.index &&
                       currentEvent.button == 0 && currentEvent.type == EventType.MouseDown){
                        m_ClickCount += 1;
                    }

                    break;
                case EventType.KeyUp:
                    if(currentEvent.keyCode == KeyCode.Return && m_RenameMotionIndex != -1){
                        m_RenameMotionIndex = -1;
                        currentEvent.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if(m_ClickCount > 0 && rect.Contains(currentEvent.mousePosition) &&
                       index == m_MotionList.index && currentEvent.button == 0 &&
                       currentEvent.type == EventType.MouseUp){
                        m_RenameMotionIndex = index;
                        m_ClickCount = 0;
                        EditorGUI.FocusTextInControl("RenameMotionField");
                        Event.current.Use();
                    }
                    else if(!rect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0 &&
                            index == m_MotionList.index && m_RenameMotionIndex != -1){
                        m_RenameMotionIndex = -1;
                        Event.current.Use();
                    }

                    break;
            }
        }

        private void AddMotion(Rect rect, ReorderableList list)
        {
            Type[] motionTypes = AppDomain.CurrentDomain.GetAssemblies()
                                          .SelectMany(assembly => assembly.GetTypes())
                                          .Where(type => typeof(MotionState).IsAssignableFrom(type) && !type.IsAbstract)
                                          .ToArray();
            GenericMenu menu = new GenericMenu();

            for(int i = 0; i < motionTypes.Length; i++){
                Type motionType = motionTypes[i];
                menu.AddItem(new GUIContent(motionType.Name), false, delegate()
                                                                         {
                                                                             serializedObject.Update();
                                                                             MotionState motion =
                                                                                 m_Controller.gameObject
                                                                                         .AddComponent(motionType) as
                                                                                     MotionState;
                                                                             motion.hideFlags =
                                                                                 HideFlags.HideInInspector;
                                                                             m_Motions.arraySize++;
                                                                             m_Motions
                                                                                 .GetArrayElementAtIndex(m_Motions
                                                                                     .arraySize - 1)
                                                                                 .objectReferenceValue = motion;
                                                                             list.index = m_Motions.arraySize - 1;
                                                                             serializedObject.ApplyModifiedProperties();
                                                                         });
            }

            menu.ShowAsContext();
        }

        private void RemoveMotion(ReorderableList list)
        {
            MotionState motion = m_Motions.GetArrayElementAtIndex(list.index).objectReferenceValue as MotionState;
            m_Motions.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
            m_Motions.DeleteArrayElementAtIndex(list.index);
            DestroyImmediate(motion, true);
            list.index = list.index - 1;

            if(list.index == -1 && m_Motions.arraySize > 0){
                list.index = 0;
            }

            EditorUtility.SetDirty(m_Controller);
        }

        private void SelectMotion(ReorderableList list)
        {
            m_RenameMotionIndex = -1;
            EditorPrefs.SetInt("MotionIndex" + target.GetInstanceID().ToString(), list.index);
        }

        private void DrawSelectedMotion(MotionState motion)
        {
            SerializedObject obj = new SerializedObject(motion);
            obj.Update();

            GUIStyle style = new GUIStyle("ProjectBrowserHeaderBgMiddle"){
                fontSize = 11,
                fontStyle = FontStyle.BoldAndItalic,
            };

            EditorGUILayout
                .LabelField(System.Text.RegularExpressions.Regex.Replace(motion.GetType().Name, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim(),
                            style);
            GUILayout.Space(8f);
            EditorGUI.BeginChangeCheck();

            SerializedProperty property = obj.GetIterator();
            property.NextVisible(true);
            property.NextVisible(false);

            do{
                EditorGUILayout.PropertyField(property, true);
            }
            while(property.NextVisible(false));

            if(EditorGUI.EndChangeCheck()){
                obj.ApplyModifiedProperties();
            }
        }
    }
}