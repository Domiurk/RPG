using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;

namespace DevionGames.InventorySystem.Configuration
{
    [CustomEditor(typeof(SavingLoading))]
    public class SavingLoadingInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_AutoSave;
        private AnimBool m_ShowSave;
        private SerializedProperty m_Provider;
        private AnimBool m_ShowMySQL;

        private SerializedProperty m_SavingKey;
        private SerializedProperty m_SavingRate;
        private SerializedProperty m_ServerAdress;
        private SerializedProperty m_SaveScript;
        private SerializedProperty m_LoadScript;

        protected virtual void OnEnable()
        {
            if (target == null) return;

            m_Script = serializedObject.FindProperty("m_Script");
            m_AutoSave = serializedObject.FindProperty("autoSave");
            m_ShowSave = new AnimBool(m_AutoSave.boolValue);
            m_ShowSave.valueChanged.AddListener(Repaint);

            m_Provider = serializedObject.FindProperty("provider");
            m_ShowMySQL = new AnimBool(m_Provider.enumValueIndex == 1);
            m_ShowMySQL.valueChanged.AddListener(Repaint);
            

            m_SavingKey = serializedObject.FindProperty("savingKey");
            m_SavingRate = serializedObject.FindProperty("savingRate");
            m_ServerAdress = serializedObject.FindProperty("serverAdress");
            m_SaveScript = serializedObject.FindProperty("saveScript");
            m_LoadScript = serializedObject.FindProperty("loadScript");
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_AutoSave);
            m_ShowSave.target = m_AutoSave.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowSave.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_SavingKey);
                EditorGUILayout.PropertyField(m_SavingRate);

                EditorGUILayout.PropertyField(m_Provider);
                m_ShowMySQL.target = m_Provider.enumValueIndex == 1;
                if (EditorGUILayout.BeginFadeGroup(m_ShowMySQL.faded))
                {

           
                    EditorGUILayout.PropertyField(m_ServerAdress);
                    EditorGUILayout.PropertyField(m_SaveScript);
                    EditorGUILayout.PropertyField(m_LoadScript);
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Space(2f);
            EditorTools.Seperator();

            List<string> keys = PlayerPrefs.GetString("InventorySystemSavedKeys").Split(';').ToList();
            keys.RemoveAll(x => string.IsNullOrEmpty(x));



            if (EditorTools.Foldout("InventorySavedData", new GUIContent("Saved Data " + keys.Count)))
            {
                EditorTools.BeginIndent(1,true);
                if (keys.Count == 0){
                    GUILayout.Label("No data saved on this device!");
                }


                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    List<string> scenes = PlayerPrefs.GetString(key + ".Scenes").Split(';').ToList();
                    scenes.RemoveAll(x => string.IsNullOrEmpty(x));
                    string uiData = PlayerPrefs.GetString(key + ".UI");
                    if (scenes.Count == 0 && string.IsNullOrEmpty(uiData)) {
                        List<string> allKeys = new List<string>(keys);
                        allKeys.Remove(key);
                        PlayerPrefs.SetString("InventorySystemSavedKeys", string.Join(";", allKeys));
                    }

                    GenericMenu keyMenu = new GenericMenu();

                    keyMenu.AddItem(new GUIContent("Delete Key"), false, () => {
                        List<string> allKeys = new List<string>(keys);
                        allKeys.Remove(key);
                        PlayerPrefs.SetString("InventorySystemSavedKeys", string.Join(";", allKeys));
                        PlayerPrefs.DeleteKey(key + ".UI");
                        PlayerPrefs.DeleteKey(key+".Scenes");
                        for (int j = 0; j < scenes.Count; j++)
                        {
                            PlayerPrefs.DeleteKey(key + "." + scenes[j]);
                        }
                    });

                    if (EditorTools.Foldout(key, new GUIContent(key),keyMenu)){
                        EditorTools.BeginIndent(1, true);
                        if (!string.IsNullOrEmpty(uiData)){
                            GenericMenu uiMenu = new GenericMenu();
                            uiMenu.AddItem(new GUIContent("Delete UI"), false, () => {
                                PlayerPrefs.DeleteKey(key + ".UI");
                            });

                            if (EditorTools.Foldout(key + ".UI", new GUIContent("UI"), uiMenu)) {
                                EditorTools.BeginIndent(1, true);
                                GUILayout.Label(uiData, EditorStyles.wordWrappedLabel);
                                EditorTools.EndIndent();
                            }
                        }
                        for (int j = 0; j < scenes.Count; j++) {
                            string scene = scenes[j];
                            GenericMenu sceneMenu = new GenericMenu();
                            sceneMenu.AddItem(new GUIContent("Delete "+scene), false, () => {
                                PlayerPrefs.DeleteKey(key + "." + scene);
                                List<string> allScenes = new List<string>(scenes);
                                allScenes.Remove(scene);
                                scenes.RemoveAll(x => string.IsNullOrEmpty(x));
                                PlayerPrefs.SetString(key+".Scenes",string.Join(";",allScenes));
                            });

                            if (EditorTools.Foldout(key + "." + scene, new GUIContent(scene), sceneMenu))
                            {
                                EditorTools.BeginIndent(1, true);
                                GUILayout.Label(PlayerPrefs.GetString(key + "." + scene), EditorStyles.wordWrappedLabel);
                                EditorTools.EndIndent();
                            }
                        }
                        EditorTools.EndIndent();
                    }
                }
                EditorTools.EndIndent();
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}