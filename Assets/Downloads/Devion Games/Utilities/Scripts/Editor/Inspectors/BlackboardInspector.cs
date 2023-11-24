using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames
{
    [CustomEditor(typeof(Blackboard),true)]
    public class BlackboardInspector : Editor
    {
        protected SerializedProperty m_Variables;
        private ReorderableList m_VariableList;
        private string m_VariableName = string.Empty;

        protected virtual void OnEnable() {
            if (target == null) return;
            m_Variables = serializedObject.FindProperty("m_Variables");
            CreateVariableList();
        }

        public override void OnInspectorGUI(){
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginVertical();
            serializedObject.Update();
            m_VariableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
            GUILayout.Space(-4.5f);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.margin = new RectOffset(0,0,1,1);
            textStyle.alignment = TextAnchor.MiddleRight;
            m_VariableName = EditorGUILayout.TextField(m_VariableName,textStyle);
            if (string.IsNullOrEmpty(m_VariableName))
            {
                Rect variableNameRect = GUILayoutUtility.GetLastRect();
                GUIStyle variableNameOverlayStyle = new GUIStyle(EditorStyles.label);
                variableNameOverlayStyle.alignment = TextAnchor.MiddleRight;
                variableNameOverlayStyle.normal.textColor = Color.grey;
                GUI.Label(variableNameRect, "(New Variable Name)",variableNameOverlayStyle);
            }
            GUIStyle createAddNewDropDown = new GUIStyle("ToolbarCreateAddNewDropDown");

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), createAddNewDropDown, GUILayout.Width(35f)))
            {
                
                if (string.IsNullOrEmpty(m_VariableName))
                {
                    EditorUtility.DisplayDialog("New Variable", "Please enter a variable name.", "OK");
                }else if (VariableNameExists(m_VariableName)){
                    EditorUtility.DisplayDialog("New Variable", "A variable with the same name already exists.", "OK");
                }
                else
                {
                    Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(x => typeof(Variable).IsAssignableFrom(x) && !x.IsAbstract && !x.HasAttribute(typeof(ExcludeFromCreation))).ToArray();
                    types = types.OrderBy(x => x.BaseType.Name).ToArray();

                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName( type.Name.Replace("Variable",""))), false, () => { AddVariable(type); });
                    }
                    menu.ShowAsContext();
                }
                EditorGUI.FocusTextInControl("");
            }

            EditorGUI.BeginDisabledGroup(m_VariableList.index == -1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), EditorStyles.toolbarButton, GUILayout.Width(25f)))
            {
                serializedObject.Update();
                m_Variables.DeleteArrayElementAtIndex(m_VariableList.index);
                serializedObject.ApplyModifiedProperties();
                m_VariableList.index = m_Variables.arraySize - 1;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private bool VariableNameExists(string name) {
            for (int i = 0; i < m_Variables.arraySize;i++) {
                SerializedProperty element = m_Variables.GetArrayElementAtIndex(i);
                if (name == element.FindPropertyRelative("m_Name").stringValue) {
                    return true;
                }
            }
            return false;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected void CreateVariableList() {
            m_VariableList = new ReorderableList(serializedObject, m_Variables, true, false, false, false);
            m_VariableList.headerHeight = 0f;
            m_VariableList.footerHeight = 0f;
            m_VariableList.showDefaultBackground = false;
            float defaultHeight = m_VariableList.elementHeight;
            float verticalOffset = (defaultHeight - EditorGUIUtility.singleLineHeight) * 0.5f;

            m_VariableList.elementHeight = (defaultHeight+verticalOffset)*2;
            m_VariableList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = m_Variables.GetArrayElementAtIndex(index);
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("m_Name"));
                rect.y = rect.y +verticalOffset+defaultHeight;

                SerializedProperty elementValue = element.FindPropertyRelative("m_Value");
                if (elementValue != null)
                {
                    EditorGUI.PropertyField(rect, elementValue, true);
                }
                else {
                    EditorGUI.LabelField(rect,"Runtime Value");
                }
    
            };
            m_VariableList.drawElementBackgroundCallback = (Rect rect, int _, bool isActive, bool isFocused) => {

                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle style = new GUIStyle("AnimItemBackground");
                    style.Draw(rect, false, isActive, isActive, isFocused);

                    GUIStyle style2 = new GUIStyle("RL Element");
                    style2.Draw(rect, false, isActive, isActive, isFocused);
                }
            };

            m_VariableList.onAddCallback = (ReorderableList _) => {

                Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(x => typeof(Variable).IsAssignableFrom(x) && !x.IsAbstract && !x.HasAttribute(typeof(ExcludeFromCreation))).ToArray();
                types = types.OrderBy(x => x.BaseType.Name).ToArray();

                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    menu.AddItem(new GUIContent(type.Name), false, () => { AddVariable(type); });
                }
                menu.ShowAsContext();
            };

        }

        private void AddVariable(Type type)
        {
            Variable value = Activator.CreateInstance(type) as Variable;
            value.name = m_VariableName;
            serializedObject.Update();
            m_Variables.arraySize++;
            m_Variables.GetArrayElementAtIndex(m_Variables.arraySize - 1).managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
            m_VariableName = string.Empty;
            m_VariableList.index = m_Variables.arraySize-1;
     


        }

 
    }
}