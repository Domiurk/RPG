using System.Collections;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor.AnimatedValues;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(UsableItem), true)]
    public class UsableItemInspector : ItemInspector
    {
        protected SerializedProperty m_UseCategoryCooldown;
        protected SerializedProperty m_Cooldown;
        protected AnimBool m_ShowCategoryCooldownOptions;

        private GameObject m_GameObject;
        private int m_ComponentInstanceID;
        private UnityEngine.Object m_Target;
        private IList m_List;
        private string m_FieldName;
        private string m_ElementTypeName;
        private Type m_ElementType;
        private static object m_ObjectToCopy;

        private SerializedProperty m_Actions;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;

            ArrayUtility.Add(ref m_PropertiesToExcludeForChildClasses, serializedObject.FindProperty("actions").propertyPath);
            m_UseCategoryCooldown = serializedObject.FindProperty("m_UseCategoryCooldown");
            m_Cooldown = serializedObject.FindProperty("m_Cooldown");
            m_ShowCategoryCooldownOptions = new AnimBool(!m_UseCategoryCooldown.boolValue);

            m_ShowCategoryCooldownOptions.valueChanged.AddListener(Repaint);
            
            m_Target = target;
            m_List = (target as UsableItem).actions;
            m_ElementType = Utility.GetElementType(m_List.GetType());
            m_ElementTypeName = m_ElementType.FullName;
            FieldInfo[] fields = m_Target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(m_Target);
                if (temp == m_List)
                    m_FieldName = fields[i].Name;
            }
            m_Actions = serializedObject.FindProperty("actions");
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChange;

        }

        protected override void OnDisable() {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChange;
        }

        public override void OnInspectorGUI()
        {
             ScriptGUI();
             serializedObject.Update();
             DrawBaseInspector();
             for (int i = 0; i < m_DrawInspectors.Count; i++)
             {
                m_DrawInspectors[i].Invoke();
             }

             DrawPropertiesExcluding(serializedObject, m_PropertiesToExcludeForChildClasses);
             ActionGUI();
             serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector() {

            DrawCooldownGUI();
        }

        protected void DrawCooldownGUI() {
            EditorGUILayout.PropertyField(m_UseCategoryCooldown);
            m_ShowCategoryCooldownOptions.target = !m_UseCategoryCooldown.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowCategoryCooldownOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_Cooldown);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
        }


        protected void ActionGUI() {
           
            GUILayout.Space(10f);
            for (int i = 0; i < m_Actions.arraySize; i++) {
                SerializedProperty action = m_Actions.GetArrayElementAtIndex(i);

                object value = m_List[i];
                EditorGUI.BeginChangeCheck();
                if (m_Target != null)
                    Undo.RecordObject(m_Target, "Item Action");

                if (EditorTools.Titlebar((target.GetInstanceID()+i).ToString(),value, ElementContextMenu(m_List, i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", value != null ? EditorTools.FindMonoScript(value.GetType()) : null, typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    if (value == null)
                    {
                        EditorGUILayout.HelpBox("Managed reference values can't be removed or replaced. Only way to fix it is to recreate the renamed or deleted script file or delete and recreate the Item and rereference it in scenes. Unity throws an error: Unknown managed type referenced: [Assembly-CSharp] + Type which has been removed.", MessageType.Error);
                    }
                    if (EditorTools.HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(action, true);
                    }
                    else
                    {
                        foreach (SerializedProperty child in action.EnumerateChildProperties())
                        {
                            EditorGUILayout.PropertyField(
                                                          child,
                                                          includeChildren: true
                                                         );
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(m_Target);
            }
            GUILayout.FlexibleSpace();

            DoAddButton();
            GUILayout.Space(10f);

        }

        private void Add(Type type)
        {
            object value = Activator.CreateInstance(type);
            serializedObject.Update();
            m_Actions.arraySize++;
            m_Actions.GetArrayElementAtIndex(m_Actions.arraySize - 1).managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateScript(string scriptName)
        {
          
        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add " + m_ElementType.Name);
             Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
             buttonRect.x = buttonRect.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
             buttonRect.width = buttonStyle.fixedWidth;

             if (GUI.Button(buttonRect, buttonContent, buttonStyle))
             {
                 AddObjectWindow.ShowWindow(buttonRect, m_ElementType, Add, CreateScript);
             }
        }

        private void OnPlaymodeStateChange(PlayModeStateChange stateChange)
        {
            Reload();
        }

        public void OnBeforeAssemblyReload()
        {
            if (m_Target is Component)
            {
                m_GameObject = (m_Target as Component).gameObject;
                m_ComponentInstanceID = (m_Target as Component).GetInstanceID();
            }
        }


        public void OnAfterAssemblyReload()
        {
            Reload();
        }

        private GenericMenu ElementContextMenu(IList list, int index)
        {
           
            GenericMenu menu = new GenericMenu();
            if (list[index] == null) {
                return menu;
            }
            menu.AddItem(new GUIContent("Reset"), false, delegate {

                object value = Activator.CreateInstance(list[index].GetType());
                list[index] = value;
                EditorUtility.SetDirty(target);
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove " + m_ElementType.Name), false, delegate { list.RemoveAt(index); EditorUtility.SetDirty(target); });

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index - 1, value);
                    EditorUtility.SetDirty(target);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < list.Count - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index + 1, value);
                    EditorUtility.SetDirty(target);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            menu.AddItem(new GUIContent("Copy " + m_ElementType.Name), false, delegate {
                object value = list[index];
                m_ObjectToCopy = value;
            });

            if (m_ObjectToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste " + m_ElementType.Name + " As New"), false, delegate {
                    object instance = Activator.CreateInstance(m_ObjectToCopy.GetType());
                    FieldInfo[] fields = instance.GetType().GetSerializedFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(m_ObjectToCopy);
                        fields[i].SetValue(instance, value);
                    }
                    list.Insert(index + 1, instance);
                    EditorUtility.SetDirty(target);
                });

                if (list[index].GetType() == m_ObjectToCopy.GetType())
                {
                    menu.AddItem(new GUIContent("Paste " + m_ElementType.Name + " Values"), false, delegate
                    {
                        object instance = list[index];
                        FieldInfo[] fields = instance.GetType().GetSerializedFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            object value = fields[i].GetValue(m_ObjectToCopy);
                            fields[i].SetValue(instance, value);
                        }
                        EditorUtility.SetDirty(target);
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste " + m_ElementType.Name + " Values"));
                }
            }

            if (list[index] != null)
            {
                MonoScript script = EditorTools.FindMonoScript(list[index].GetType());
                if (script != null)
                {
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Edit Script"), false, delegate { AssetDatabase.OpenAsset(script); });
                }
            }
            return menu;
        }

        private void Reload()
        {
            if (m_GameObject != null)
            {
                Component[] components = m_GameObject.GetComponents(typeof(Component));
                m_Target = Array.Find(components, x => x.GetInstanceID() == m_ComponentInstanceID);
            }

            m_ElementType = Utility.GetType(m_ElementTypeName);
            m_List = m_Target.GetType().GetSerializedField(m_FieldName).GetValue(m_Target) as IList;
           
        }   
    }
}
