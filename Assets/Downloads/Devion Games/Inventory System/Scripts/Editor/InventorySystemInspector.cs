using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class InventorySystemInspector
    {
        private ItemDatabase m_Database;
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
            m_Database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(EditorPrefs.GetString("ItemDatabasePath"));
            if (m_Database == null) {
                string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_Database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(path);
                }
            }
            toolbarIndex = EditorPrefs.GetInt("InventoryToolbarIndex");

            ResetChildEditors();

        }

        public void OnDisable()
        {
            if (m_Database != null) {
                EditorPrefs.SetString("ItemDatabasePath",AssetDatabase.GetAssetPath(m_Database));
            }
            EditorPrefs.SetInt("InventoryToolbarIndex",toolbarIndex);

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

        private void DoToolbar() {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            SelectDatabaseButton();
           
            if (m_ChildEditors != null)
                toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarNames, GUILayout.MinWidth(200));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void SelectDatabaseButton() {
            GUIStyle buttonStyle = EditorStyles.objectField;
            GUIContent buttonContent = new GUIContent(m_Database != null ? m_Database.name : "Null");
            Rect buttonRect = GUILayoutUtility.GetRect(180f,18f);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), 
                    (Object obj)=> { 
                        m_Database = obj as ItemDatabase;
                        ResetChildEditors();
                    }, 
                    ()=> {
                        ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
                        if (db != null)
                        {
                            CreateDefaultCategory(db);
                            m_Database = db;
                            ResetChildEditors();
                        }
                    });
            }
        }

        private static void CreateDefaultCategory(ItemDatabase database)
        {
            Category category = ScriptableObject.CreateInstance<Category>();
            category.Name = "None";
            category.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(category, database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            database.categories.Add(category);
            EditorUtility.SetDirty(database);
        }

        private void ResetChildEditors() {

            if (m_Database != null)
            {
                m_Database.items.RemoveAll(x => x == null);
                EditorUtility.SetDirty(m_Database);
                m_ChildEditors = new List<ICollectionEditor>{
                    new ItemCollectionEditor(m_Database, m_Database.items,
                                             m_Database.categories.Select(x => x.Name).ToList()),
                    new ScriptableObjectCollectionEditor<Currency>(m_Database, m_Database.currencies),
                    new ScriptableObjectCollectionEditor<Rarity>(m_Database, m_Database.raritys),
                    new ScriptableObjectCollectionEditor<Category>(m_Database, m_Database.categories),
                    new ScriptableObjectCollectionEditor<EquipmentRegion>(m_Database, m_Database.equipments),
                    new ScriptableObjectCollectionEditor<ItemGroup>(m_Database, m_Database.itemGroups),
                    new Configuration.ItemSettingsEditor(m_Database, m_Database.settings)
                };

                for (int i = 0; i < m_ChildEditors.Count; i++)
                {
                    m_ChildEditors[i].OnEnable();
                }
            }
        }

    }
}