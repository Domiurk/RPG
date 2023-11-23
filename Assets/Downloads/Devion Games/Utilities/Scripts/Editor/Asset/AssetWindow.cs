using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace DevionGames
{
    public class AssetWindow : EditorWindow
    {
        protected UnityEngine.Object[] m_Targets;
        protected Vector2 m_ScrollPosition;
        protected List<Editor> m_Editors;
        protected string m_ElementPropertyPath;
        protected string m_ElementTypeName;

        protected Type m_ElementType;
        protected Type elementType {
            get {
                if (m_ElementType == null) {
                    m_ElementType = Utility.GetType(m_ElementTypeName);
                }
                return m_ElementType;
            }
            set {
                m_ElementType = value;
                m_ElementTypeName = m_ElementType.Name;
            }
        }
        protected UnityEngine.Object m_Target;
        protected static Component m_CopyComponent;
        protected bool m_ApplyToPrefab=false;
        protected bool m_HasPrefab;
        protected static Component[] m_CopyComponents;


        public static void ShowWindow(string title, SerializedProperty elements)
        {
            AssetWindow[] objArray = Resources.FindObjectsOfTypeAll<AssetWindow>();
            AssetWindow window = (objArray.Length <= 0 ? CreateInstance<AssetWindow>() : objArray[0]);
            window.hideFlags = HideFlags.HideAndDontSave;
            window.minSize = new Vector2(260f, 200f);
            window.titleContent = new GUIContent(title);
            window.m_ElementPropertyPath = elements.propertyPath;
            window.m_Target = elements.serializedObject.targetObject;
            window.m_Targets = new UnityEngine.Object[elements.arraySize];
            for (int i = 0; i < elements.arraySize; i++){
                window.m_Targets[i] = elements.GetArrayElementAtIndex(i).objectReferenceValue;
            }
            window.m_HasPrefab = PrefabUtility.GetNearestPrefabInstanceRoot(window.m_Target) != null;
            window.m_Editors = new List<Editor>();
            window.elementType = Utility.GetType(elements.arrayElementType.Replace("PPtr<$", "").Replace(">", ""));
            for (int i = 0; i < window.m_Targets.Length; i++)
            {
                Editor editor = Editor.CreateEditor(window.m_Targets[i]);
                window.m_Editors.Add(editor);
            }
            window.FixMissingAssets();
          //  EditorApplication.playModeStateChanged += OnPlaymodeStateChange;
            window.ShowUtility();
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }


        protected virtual void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Close();
        }

        protected virtual void OnAfterAssemblyReload()
        {
            Close();
        }


        /*protected static void OnPlaymodeStateChange(PlayModeStateChange state) {
            AssetWindow[] objArray = Resources.FindObjectsOfTypeAll<AssetWindow>();
            for (int i = 0; i < objArray.Length; i++) {
                objArray[i].Close();
            }
        }*/

        protected virtual void OnGUI()
        {
            DoApplyToPrefab();
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            GUILayout.Space(1f);
            for (int i = 0; i < m_Targets.Length; i++)
            {
                UnityEngine.Object target = m_Targets[i];
                Editor editor = m_Editors[i];

                if (EditorTools.Titlebar(target, GetContextMenu(target)))
                {
                    EditorGUI.indentLevel += 1;
                    editor.OnInspectorGUI();
                    EditorGUI.indentLevel -= 1;
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DoAddButton();
            GUILayout.Space(10f);
            DoCopyPaste();
        }



        protected virtual void DoApplyToPrefab() {
            if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Prefab Overrides: " + PrefabUtility.GetObjectOverrides((m_Target as Component).gameObject).Count);

                GUILayout.FlexibleSpace();

                if (!m_ApplyToPrefab)
                {
                    if (GUILayout.Button("Apply to prefab"))
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        protected virtual void DoAddButton() {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add " + ObjectNames.NicifyVariableName(elementType.Name));
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.width = buttonStyle.fixedWidth;
            buttonRect.x = position.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, elementType, AddAsset, CreateScript);
            }
        }

        protected virtual void RemoveTarget(int index) {
            DestroyImmediate(m_Editors[index]);
            m_Editors.RemoveAt(index);
            DestroyImmediate(m_Targets[index]);
            ArrayUtility.RemoveAt(ref m_Targets, index);

            SerializedObject serializedObject = new SerializedObject(m_Target);
            SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
            serializedObject.Update();
            elements.GetArrayElementAtIndex(index).objectReferenceValue = null;
            elements.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void DoCopyPaste() {

            Event currentEvent = Event.current;
            switch (currentEvent.rawType)
            {
                case EventType.KeyUp:
                    if (currentEvent.control)
                    {
                        if (currentEvent.keyCode == KeyCode.C)
                        {
                            m_CopyComponents = m_Targets.Where(x => typeof(Component).IsAssignableFrom(x.GetType())).Select(y => y as Component).ToArray();
                        }
                        else if (currentEvent.keyCode == KeyCode.V && m_CopyComponents != null && m_CopyComponents.Length > 0)
                        {
                            for (int i = 0; i < m_Targets.Length; i++)
                            {
                                int index = i;
                                RemoveTarget(index);
                            }
                            for (int i = 0; i < m_CopyComponents.Length; i++)
                            {
                                Component copy = m_CopyComponents[i];
                                AddAsset(copy.GetType());
                                UnityEditorInternal.ComponentUtility.CopyComponent(copy);
                                UnityEditorInternal.ComponentUtility.PasteComponentValues((Component)m_Targets[i]);
                            }
                            if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                            {
                                PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                            }
                        }
                    }
                    break;
            }

        }

        protected virtual void AddAsset(Type type) {
            SerializedObject serializedObject = new SerializedObject(m_Target);
            SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
           
            UnityEngine.Object element = null;
            if (m_Target is Component)
            {
                element = (m_Target as Component).gameObject.AddComponent(type);
            }

            element.hideFlags = HideFlags.HideInInspector;
            ArrayUtility.Add<UnityEngine.Object>(ref m_Targets, element);
            Editor editor = Editor.CreateEditor(element);
            m_Editors.Add(editor);
            serializedObject.Update();
            elements.arraySize++;
            elements.GetArrayElementAtIndex(elements.arraySize - 1).objectReferenceValue = element;
            serializedObject.ApplyModifiedProperties();

            m_ScrollPosition.y = float.MaxValue;
            if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
            {
                PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
            }
            Focus();
        }

        protected virtual void CreateScript(string scriptName) {
            scriptName = scriptName.Replace(" ", "_");
            scriptName = scriptName.Replace("-", "_");
            string path = "Assets/" + scriptName + ".cs";

            if (File.Exists(path) == false)
            {
                using (StreamWriter outfile = new StreamWriter(path))
                {
                    MethodInfo[] methods = elementType.GetAllMethods();
                    methods = methods.Where(x => x.IsAbstract).ToArray();

                    outfile.WriteLine("using UnityEngine;");
                    outfile.WriteLine("using System.Collections;");
                    outfile.WriteLine("using "+elementType.Namespace+";");
                    outfile.WriteLine("");
                    if (!typeof(Component).IsAssignableFrom(elementType))
                    {
                        outfile.WriteLine("[System.Serializable]");
                    }
                    outfile.WriteLine("public class " + scriptName + " : "+elementType.Name+ "{");
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        ParameterInfo[] parameters = method.GetParameters();
                        string parameterString = string.Empty;
                        for (int j = 0; j < parameters.Length; j++) {
                            string typeName = parameters[j].ParameterType.Name;
                            string parameterName = string.Empty;
                            if (Char.IsLower(typeName, 0)) {
                                parameterName = "_" + typeName;
                            }else {
                               parameterName =  char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);
                            }
                            parameterString += ", " + typeName + " " + parameterName;
                        }

                        if (!string.IsNullOrEmpty(parameterString)) {
                            parameterString = parameterString.Substring(1);
                        }

                        outfile.WriteLine("\t"+(method.IsPublic?"public":"protected")+" override "+ EditorTools.CovertToAliasString(method.ReturnType) +" "+method.Name  +"("+parameterString+") {");

                        if (method.ReturnType == typeof(string))
                        {
                            outfile.WriteLine("\t\treturn string.Empty;");
                        }
                        else if (method.ReturnType == typeof(bool))
                        {
                            outfile.WriteLine("\t\treturn true;");
                        }
                        else if (method.ReturnType == typeof(Vector2))
                        {
                            outfile.WriteLine("\t\treturn Vector2.zero;");
                        }
                        else if (method.ReturnType == typeof(Vector3))
                        {
                            outfile.WriteLine("\t\treturn Vector3.zero;");
                        }
                        else if (!method.ReturnType.IsValueType || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            outfile.WriteLine("\t\treturn null;");
                        }
                        else if (UnityTools.IsInteger(method.ReturnType))
                        {
                            outfile.WriteLine("\t\treturn 0;");
                        }
                        else if (UnityTools.IsFloat(method.ReturnType))
                        {
                            outfile.WriteLine("\t\treturn 0.0f;");
                        }else if (method.ReturnType.IsEnum) {
                            outfile.WriteLine("\t\treturn "+method.ReturnType.Name+"."+ Enum.GetNames(method.ReturnType)[0] + ";");
                        } 

                        outfile.WriteLine("\t}");
                        outfile.WriteLine("");
                    }
                    outfile.WriteLine("}");
                }
            }
            AssetDatabase.Refresh();
            EditorPrefs.SetString("NewScriptToCreate", scriptName);
            EditorPrefs.SetInt("AssetWindowID", GetInstanceID());
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        protected static void OnScriptsReloaded()
        {
             string scriptName = EditorPrefs.GetString("NewScriptToCreate");
             int windowID = EditorPrefs.GetInt("AssetWindowID");
            if (string.IsNullOrEmpty(scriptName))
            {
                EditorPrefs.DeleteKey("NewScriptToCreate");
                EditorPrefs.DeleteKey("AssetWindowID");
                return;
            }

            Type type = Utility.GetType(scriptName);
             if (!EditorApplication.isPlayingOrWillChangePlaymode && !string.IsNullOrEmpty(scriptName) && type != null)
             {
                AssetWindow[] windows = Resources.FindObjectsOfTypeAll<AssetWindow>();
                AssetWindow window = windows.Where(x => x.GetInstanceID() == windowID).FirstOrDefault();
                if (window != null) {
                    window.AddAsset(type);
                }

             }
             EditorPrefs.DeleteKey("NewScriptToCreate");
             EditorPrefs.DeleteKey("AssetWindowID");
        }


        protected virtual GenericMenu GetContextMenu(UnityEngine.Object target) {
            GenericMenu menu = new GenericMenu();
            int index = Array.IndexOf(m_Targets,target);
            menu.AddItem(new GUIContent("Reset"), false, delegate {
                Type type = target.GetType();
                DestroyImmediate(target);
                m_Targets[index] = (m_Target as Component).gameObject.AddComponent(type);
                DestroyImmediate(m_Editors[index]);
                m_Editors[index] = Editor.CreateEditor(m_Targets[index]);
                SerializedObject serializedObject = new SerializedObject(m_Target);
                SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                serializedObject.Update();
                elements.GetArrayElementAtIndex(index).objectReferenceValue = m_Targets[index];
                serializedObject.ApplyModifiedProperties();
                if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                {
                    PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                }

            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove"), false, delegate {
                DestroyImmediate(m_Editors[index]);
                m_Editors.RemoveAt(index);
                DestroyImmediate(target);
                ArrayUtility.RemoveAt(ref m_Targets, index);

                SerializedObject serializedObject = new SerializedObject(m_Target);
                SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                serializedObject.Update();
                elements.GetArrayElementAtIndex(index).objectReferenceValue = null;
                elements.DeleteArrayElementAtIndex(index); 
                serializedObject.ApplyModifiedProperties();
                if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                {
                    PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                }
            });

            menu.AddItem(new GUIContent("Copy"), false, delegate {
                m_CopyComponent = target as Component;
            });

            if (m_CopyComponent != null && m_CopyComponent.GetType() == target.GetType())
            {
                menu.AddItem(new GUIContent("Paste"), false, delegate {
                    UnityEditorInternal.ComponentUtility.CopyComponent(m_CopyComponent);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues((Component)target);
                    if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"));
            }

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate
                {
                    ArrayUtility.RemoveAt(ref m_Targets, index);
                    ArrayUtility.Insert(ref m_Targets, index - 1, target);
                    Editor editor = m_Editors[index];
                    m_Editors.RemoveAt(index);
                    m_Editors.Insert(index-1,editor);

                    SerializedObject serializedObject = new SerializedObject(m_Target);
                    SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                    serializedObject.Update();
                    elements.MoveArrayElement(index,index-1);
                    serializedObject.ApplyModifiedProperties();
                    if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }
            if (index < m_Targets.Length - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    ArrayUtility.RemoveAt(ref m_Targets, index);
                    ArrayUtility.Insert(ref m_Targets, index + 1, target);
                    Editor editor = m_Editors[index];
                    m_Editors.RemoveAt(index);
                    m_Editors.Insert(index + 1, editor);

                    SerializedObject serializedObject = new SerializedObject(m_Target);
                    SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                    serializedObject.Update();
                    elements.MoveArrayElement(index, index + 1);
                    serializedObject.ApplyModifiedProperties();
                    if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }
            return menu;
        }

        protected void FixMissingAssets() {

            //Component added manually
            if (typeof(Component).IsAssignableFrom(m_Target.GetType()))
            {
                Component[] components = (m_Target as Component).GetComponents(elementType);
                components = components.Where(x => !m_Targets.Contains(x)).ToArray();
                for (int i = 0; i < components.Length; i++)
                {
                    ArrayUtility.Add<UnityEngine.Object>(ref m_Targets, components[i]);
                    Editor editor = Editor.CreateEditor(components[i]);
                    m_Editors.Add(editor);
                    SerializedObject serializedObject = new SerializedObject(m_Target);
                    SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                    serializedObject.Update();
                    elements.arraySize++;
                    elements.GetArrayElementAtIndex(elements.arraySize - 1).objectReferenceValue = components[i];
                    serializedObject.ApplyModifiedProperties();
                    if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                }
            }
            //Component removed manually
            for (int i = 0; i < m_Targets.Length; i++) {
                if (m_Targets[i] == null) {
                    DestroyImmediate(m_Editors[i]);
                    m_Editors.RemoveAt(i);
                    ArrayUtility.RemoveAt(ref m_Targets, i);

                    SerializedObject serializedObject = new SerializedObject(m_Target);
                    SerializedProperty elements = serializedObject.FindProperty(m_ElementPropertyPath);
                    serializedObject.Update();
                    elements.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    elements.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    if (m_HasPrefab && typeof(Component).IsAssignableFrom(m_Target.GetType()) && m_ApplyToPrefab)
                    {
                        PrefabUtility.ApplyPrefabInstance((m_Target as Component).gameObject, InteractionMode.AutomatedAction);
                    }
                }
            }
        }


        protected virtual void Update()
        {
            Repaint();
        }

        protected virtual void OnDestroy()
        {
            for (int i = m_Editors.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(m_Editors[i]);
            }
        }

    }
}