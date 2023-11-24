using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames
{
    [CustomEditor(typeof(AudioEventListener))]
    public class AudioEventListenerInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_AudioGroups;
        private string m_AudioGroupName = string.Empty;
        private ReorderableList m_AudioGroupList;

        private void OnEnable()
        {
            m_Script = serializedObject.FindProperty("m_Script");
            m_AudioGroups = serializedObject.FindProperty("m_AudioGroups");
            CreateAudioGroupList();
            
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginVertical();
            serializedObject.Update();
            m_AudioGroupList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
            GUILayout.Space(-4.5f);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.margin = new RectOffset(0,1, 1, 1);
            textStyle.alignment = TextAnchor.MiddleRight;
            m_AudioGroupName = EditorGUILayout.TextField(m_AudioGroupName, textStyle);
            if (string.IsNullOrEmpty(m_AudioGroupName))
            {
                Rect variableNameRect = GUILayoutUtility.GetLastRect();
                GUIStyle variableNameOverlayStyle = new GUIStyle(EditorStyles.label);
                variableNameOverlayStyle.alignment = TextAnchor.MiddleRight;
                variableNameOverlayStyle.normal.textColor = Color.grey;
                GUI.Label(variableNameRect, "(New Group Name)", variableNameOverlayStyle);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), "toolbarbuttonLeft", GUILayout.Width(28f)))
            {

                if (string.IsNullOrEmpty(m_AudioGroupName))
                {
                    EditorUtility.DisplayDialog("New Audio Group", "Please enter a group name.", "OK");
                }
                else if (AudioGroupNameExists(m_AudioGroupName))
                {
                    EditorUtility.DisplayDialog("New Audio Group", "A group with the same name already exists.", "OK");
                }
                else
                {
                    AddGroup();
                }
                EditorGUI.FocusTextInControl("");
            }

            EditorGUI.BeginDisabledGroup(m_AudioGroupList.index == -1);
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), EditorStyles.toolbarButton, GUILayout.Width(25f)))
            {
                serializedObject.Update();
                m_AudioGroups.DeleteArrayElementAtIndex(m_AudioGroupList.index);
                serializedObject.ApplyModifiedProperties();
                m_AudioGroupList.index = m_AudioGroups.arraySize - 1;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private bool AudioGroupNameExists(string name)
        {
            for (int i = 0; i < m_AudioGroups.arraySize; i++)
            {
                SerializedProperty element = m_AudioGroups.GetArrayElementAtIndex(i);
                if (name == element.FindPropertyRelative("name").stringValue)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        protected void CreateAudioGroupList()
        {
            m_AudioGroupList = new ReorderableList(serializedObject, m_AudioGroups, true, false, false, false);
            m_AudioGroupList.headerHeight = 0f;
            m_AudioGroupList.footerHeight = 0f;
            m_AudioGroupList.showDefaultBackground = false;
            float defaultHeight = m_AudioGroupList.elementHeight;
            float verticalOffset = (defaultHeight - EditorGUIUtility.singleLineHeight) * 0.5f;

            m_AudioGroupList.elementHeight = (defaultHeight + verticalOffset) * 2;
            m_AudioGroupList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = m_AudioGroups.GetArrayElementAtIndex(index);
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("name"));
                rect.y = rect.y + verticalOffset + defaultHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("m_AudioMixerGroup"), true);

            };
            m_AudioGroupList.drawElementBackgroundCallback = (Rect rect, int _, bool isActive, bool isFocused) => {

                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle style = new GUIStyle("AnimItemBackground");
                    style.Draw(rect, false, isActive, isActive, isFocused);

                    GUIStyle style2 = new GUIStyle("RL Element");
                    style2.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
        }

        private void AddGroup()
        {
           
            serializedObject.Update();
            m_AudioGroups.arraySize++;
            SerializedProperty property = m_AudioGroups.GetArrayElementAtIndex(m_AudioGroups.arraySize - 1);
            property.FindPropertyRelative("name").stringValue=m_AudioGroupName;
            serializedObject.ApplyModifiedProperties();
            m_AudioGroupName = string.Empty;
            m_AudioGroupList.index = m_AudioGroups.arraySize - 1;



        }
    }
}