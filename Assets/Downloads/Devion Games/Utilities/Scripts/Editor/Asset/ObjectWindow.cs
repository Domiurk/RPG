using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    [Serializable]
    public class ObjectWindow : EditorWindow
    {
        private SerializedObject m_SerializedObject;
        private SerializedProperty m_SerializedProperty;

        private GameObject m_GameObject;
        private int m_ComponentInstanceID;
        private UnityEngine.Object m_Target;
        private string m_PropertyPath;

        private IList m_List;
        private string m_FieldName;
        private string m_ElementTypeName;
        private Type m_ElementType;
        private static object m_ObjectToCopy;
        private Vector2 m_ScrollPosition;
        private System.Action onChange;

        public static void ShowWindow(string title, SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            ObjectWindow[] objArray = Resources.FindObjectsOfTypeAll<ObjectWindow>();
            ObjectWindow window = (objArray.Length <= 0 ? CreateInstance<ObjectWindow>() : objArray[0]);

            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);
            window.Initialize(serializedObject, serializedProperty);
            window.ShowUtility();
        }

        public static void ShowWindow(string title, IList list, System.Action onChange)
        {
            ObjectWindow[] objArray = Resources.FindObjectsOfTypeAll<ObjectWindow>();
            ObjectWindow window = (objArray.Length <= 0 ? CreateInstance<ObjectWindow>() : objArray[0]);

            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);

            window.Initialize(list, onChange);
            window.ShowUtility();
        }
        

        private void Initialize(IList list, System.Action onChange)
        {
            m_List = list;
            m_ElementType = Utility.GetElementType(m_List.GetType());
            m_ElementTypeName = m_ElementType.Name;
            this.onChange = onChange;
        }

        private void Initialize(SerializedObject serializedObject, SerializedProperty serializedProperty) {
            m_SerializedObject = serializedObject;
            m_SerializedProperty = serializedProperty;
            m_PropertyPath = serializedProperty.propertyPath;
            m_Target = serializedObject.targetObject;
            m_List = serializedProperty.GetValue() as IList;
            m_ElementType = Utility.GetElementType(m_List.GetType());
            m_ElementTypeName = m_ElementType.Name;
            FieldInfo[] fields = m_Target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(m_Target);
                if (temp == m_List)
                    m_FieldName = fields[i].Name;
            }
        }

        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChange;
            Selection.selectionChanged += OnSelectionChange;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChange;
            Selection.selectionChanged -= OnSelectionChange;
            GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(m_Target);
            if (prefab != null)
                PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            GUILayout.Space(1f);
            if (m_Target != null) {
                DoSerializedPropertyGUI();
            }else {
                DoListGUI();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DoAddButton();
            GUILayout.Space(10f);
        }

        private void DoListGUI() {
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < m_List.Count; i++)
            {
                object value = m_List[i];
                if (EditorTools.Titlebar(value, GetObjectMenu(i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", EditorTools.FindMonoScript(value.GetType()), typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    EditorTools.DrawFields(value);
                    EditorGUI.indentLevel -= 1;
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                onChange.Invoke();
            }
        }

        private void DoSerializedPropertyGUI() {
            m_SerializedObject.Update();
            for (int i = 0; i < m_List.Count; i++)
            {
                object value = m_List[i];
                EditorGUI.BeginChangeCheck();
                if (m_Target != null)
                    Undo.RecordObject(m_Target, "ObjectWindow");

                if (EditorTools.Titlebar(value, GetObjectMenu(i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", EditorTools.FindMonoScript(value.GetType()), typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    SerializedProperty element = m_SerializedProperty.GetArrayElementAtIndex(i);
                    if (EditorTools.HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(element, true);
                    }
                    else
                    {
                        foreach (var child in element.EnumerateChildProperties())
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
                {
                    EditorUtility.SetDirty(m_Target);
                }
            }
            m_SerializedObject.ApplyModifiedProperties();
        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add " + m_ElementType.Name);
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.width = buttonStyle.fixedWidth;
            buttonRect.x = position.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, m_ElementType, Add, CreateScript);
            }
        }
    
        private void CreateScript(string scriptName)
        {
            Debug.LogWarning("This is not implemented yet!");
        }


        private void Add(Type type)
        {
            object value = Activator.CreateInstance(type);
            if (m_Target != null)
            {
                m_SerializedObject.Update();
                m_SerializedProperty.arraySize++;
                m_SerializedProperty.GetArrayElementAtIndex(m_SerializedProperty.arraySize - 1).managedReferenceValue = value;
                m_SerializedObject.ApplyModifiedProperties();
                GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(m_Target);
                if(prefab != null)
                    PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.AutomatedAction);
            }
            else {
                m_List.Add(value);
                onChange.Invoke();
            }

        }


        private GenericMenu GetObjectMenu(int index)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, delegate
            {
                object value = Activator.CreateInstance(m_List[index].GetType());
                m_List[index] = value;
                if (onChange != null)
                    onChange.Invoke();
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove " + m_ElementType.Name), false, delegate { 
                m_List.RemoveAt(index);
                if (m_Target != null)
                    EditorUtility.SetDirty(m_Target);

                if (onChange != null)
                    onChange.Invoke();
            });

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate
                {
                    object value = m_List[index];
                    m_List.RemoveAt(index);
                    m_List.Insert(index - 1, value);
                    if (m_Target != null)
                        EditorUtility.SetDirty(m_Target);
                    if (onChange != null)
                        onChange.Invoke();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < m_List.Count - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    object value = m_List[index];
                    m_List.RemoveAt(index);
                    m_List.Insert(index + 1, value);
                    if (m_Target != null)
                        EditorUtility.SetDirty(m_Target);
                    if (onChange != null)
                        onChange.Invoke();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            menu.AddItem(new GUIContent("Copy " + m_ElementType.Name), false, delegate
            {
                object value = m_List[index];
                m_ObjectToCopy = value;
                if (onChange != null)
                    onChange.Invoke();
            });

            if (m_ObjectToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste " + m_ElementType.Name + " As New"), false, delegate
                {
                    object instance = Activator.CreateInstance(m_ObjectToCopy.GetType());
                    FieldInfo[] fields = instance.GetType().GetSerializedFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(m_ObjectToCopy);
                        fields[i].SetValue(instance, value);
                    }
                    m_List.Insert(index + 1, instance);
                    if (m_Target != null)
                        EditorUtility.SetDirty(m_Target);

                    if (onChange != null)
                        onChange.Invoke();
                });

                if (m_List[index].GetType() == m_ObjectToCopy.GetType())
                {
                    menu.AddItem(new GUIContent("Paste " + m_ElementType.Name + " Values"), false, delegate
                    {
                        object instance = m_List[index];
                        FieldInfo[] fields = instance.GetType().GetSerializedFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            object value = fields[i].GetValue(m_ObjectToCopy);
                            fields[i].SetValue(instance, value);
                        }
                        if (m_Target != null)
                            EditorUtility.SetDirty(m_Target);

                        if (onChange != null)
                            onChange.Invoke();
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste " + m_ElementType.Name + " Values"));
                }
            }


            MonoScript script = EditorTools.FindMonoScript(m_List[index].GetType());
            if (script != null)
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Edit Script"), false, delegate { AssetDatabase.OpenAsset(script); });
            }
            return menu;
        }

        private void OnPlaymodeStateChange(PlayModeStateChange stateChange)
        {

            Reload();
        }

        public void OnBeforeAssemblyReload()
        {
            /*this.m_ElementTypeName = this.m_ElementType.Name;
            FieldInfo[] fields = this.m_Target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(this.m_Target);
                if (temp == this.m_List)
                    this.m_FieldName = fields[i].Name;
            }*/
            if (m_Target != null && m_Target is Component)
            {
                m_GameObject = (m_Target as Component).gameObject;
                m_ComponentInstanceID = (m_Target as Component).GetInstanceID();
            }
        }

        public void OnAfterAssemblyReload()
        {
            Reload();
        }

        private void OnSelectionChange()
        {
            Reload();
        }

        private void Reload()
        {
            if (m_Target == null){
                Close();
                return;
            }

            if (m_GameObject != null)
            {
                Component[] components = m_GameObject.GetComponents(typeof(Component));
                m_Target = Array.Find(components, x => x.GetInstanceID() == m_ComponentInstanceID);
            }
            m_SerializedObject = new SerializedObject(m_Target);
            m_SerializedProperty = m_SerializedObject.FindProperty(m_PropertyPath);
            m_ElementType = Utility.GetType(m_ElementTypeName);
            m_List = m_Target.GetType().GetSerializedField(m_FieldName).GetValue(m_Target) as IList;
        }
    }
}