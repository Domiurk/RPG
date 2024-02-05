using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatSystemInspector
    {
        private StatDatabase m_Database;
        private List<ICollectionEditor> m_ChildEditors;

        [SerializeField]
        private int toolbarIndex;

        private string[] toolbarNames
        {
            get
            {
                string[] items = new string[m_ChildEditors.Count];
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    items[i] = m_ChildEditors[i].ToolbarName;
                }
                return items;
            }
        }

        public void OnEnable()
        {
            m_Database = AssetDatabase.LoadAssetAtPath<StatDatabase>(EditorPrefs.GetString("StatDatabasePath"));
            if (m_Database == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:StatDatabase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_Database = AssetDatabase.LoadAssetAtPath<StatDatabase>(path);
                }
            }
            toolbarIndex = EditorPrefs.GetInt("StatToolbarIndex");
            ResetChildEditors();

        }

        public void OnDisable()
        {
            if (m_Database != null)
            {
                EditorPrefs.SetString("StatDatabasePath", AssetDatabase.GetAssetPath(m_Database));
            }
            EditorPrefs.SetInt("StatToolbarIndex", toolbarIndex);
            if (m_ChildEditors != null)
            {
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnDisable();
                }
            }
        }

        public void OnDestroy()
        {
            if (m_ChildEditors != null)
            {
                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnDestroy();
                }
            }
        }

        public void OnGUI(Rect position)
        {

            DoToolbar();

            if (m_ChildEditors != null)
            {
                m_ChildEditors[toolbarIndex].OnGUI(new Rect(0f, 30f, position.width, position.height - 30f));
            }
        }

        private void DoToolbar()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SelectDatabaseButton();

            if (m_ChildEditors != null)
                toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarNames, GUILayout.MinWidth(200));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void SelectDatabaseButton()
        {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(m_Database != null ? m_Database.name : "Null");
            Rect buttonRect = GUILayoutUtility.GetRect(180f, 18f);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(StatDatabase),
                    (Object obj) => {
                        m_Database = obj as StatDatabase;
                        ResetChildEditors();
                    },
                    () => {
                        StatDatabase db = EditorTools.CreateAsset<StatDatabase>(true);
                        if (db != null)
                        {
                            m_Database = db;
                            ResetChildEditors();
                        }
                    });
            }
        }

        private void ResetChildEditors()
        {

            if (m_Database != null)
            {
                m_ChildEditors = new List<ICollectionEditor>{
                    new StatCollectionEditor(m_Database, m_Database.items, new List<string>()),
                    new ScriptableObjectCollectionEditor<StatEffect>("Effects", m_Database, m_Database.effects, false),
                    new Configuration.StatSettingsEditor(m_Database, m_Database.settings)
                };

                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnEnable();
                }
            }
        }
    }
}