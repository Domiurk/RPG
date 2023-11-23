using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    public class AddObjectWindow : EditorWindow
    {
        private static Styles m_Styles;
        private string m_SearchString = string.Empty;
        private bool isSearching => !string.IsNullOrEmpty(m_SearchString);

        private Vector2 m_ScrollPosition;
        private Type m_Type;
        private Element m_RootElement;
        private Element m_SelectedElement;
        private string m_NewScriptName;

        public delegate void AddCallbackDelegate(Type type);

        public AddCallbackDelegate onAddCallback;

        public delegate void CreateCallbackDelegate(string scriptName);

        public CreateCallbackDelegate onCreateCallback;

        public static void ShowWindow<T>(Rect buttonRect,
                                         AddCallbackDelegate addCallback,
                                         CreateCallbackDelegate createCallback)
        {
            ShowWindow(buttonRect, typeof(T), addCallback, createCallback);
        }

        public static void ShowWindow(Rect buttonRect,
                                      Type type,
                                      AddCallbackDelegate addCallback,
                                      CreateCallbackDelegate createCallback)
        {
            AddObjectWindow window = CreateInstance<AddObjectWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window.m_Type = type;
            window.onAddCallback = addCallback;
            window.onCreateCallback = createCallback;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 280f));
        }

        private void OnEnable()
        {
            m_SearchString = EditorPrefs.GetString("AddAssetSearch", m_SearchString);
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if(m_Styles == null){
                m_Styles = new Styles();
            }

            if(m_RootElement == null){
                m_RootElement = BuildElements();
                m_SelectedElement = m_RootElement;
            }

            GUILayout.Space(5f);
            m_SearchString = SearchField(m_SearchString);
            Header();

            if(isSearching){
                Element[] elements = GetAllElements(m_RootElement);
                DrawElements(elements);
            }
            else{
                DrawElements(m_SelectedElement.children.ToArray());
            }

            if(Event.current.type == EventType.Repaint){
                m_Styles.background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
            }
        }

        private void Header()
        {
            GUIContent content = m_SelectedElement.label;
            Rect headerRect = GUILayoutUtility.GetRect(content, m_Styles.header);

            if(GUI.Button(headerRect, content, m_Styles.header)){
                if(m_SelectedElement.parent != null && !isSearching){
                    m_SelectedElement = m_SelectedElement.parent;
                }
            }

            if(Event.current.type == EventType.Repaint && m_SelectedElement.parent != null){
                m_Styles.leftArrow.Draw(new Rect(headerRect.x, headerRect.y + 4f, 16f, 16f), false, false, false,
                                        false);
            }
        }

        private void DrawElements(Element[] elements)
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            foreach(Element element in elements){
                if(element.onGUI != null && !isSearching){
                    element.onGUI();
                    continue;
                }

                if(!SearchMatch(element)){
                    continue;
                }

                Color backgroundColor = GUI.backgroundColor;
                Color textColor = m_Styles.elementButton.normal.textColor;
                int padding = m_Styles.elementButton.padding.left;
                Rect rect = GUILayoutUtility.GetRect(element.label, m_Styles.elementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition)
                                           ? GUI.backgroundColor
                                           : new Color(0, 0, 0, 0.0f));
                m_Styles.elementButton.normal.textColor =
                    (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);
                Texture2D icon = null; //(Texture2D)EditorGUIUtility.ObjectContent(null, element.type).image;

                if(element.type != null){
                    icon = EditorGUIUtility.FindTexture("cs Script Icon");
                    IconAttribute iconAttribute = element.type.GetCustomAttribute<IconAttribute>();

                    if(iconAttribute != null){
                        if(iconAttribute.type != null){
                            icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.type);
                        }
                        else{
                            icon = Resources.Load<Texture2D>(iconAttribute.path);
                        }
                    }
                }

                m_Styles.elementButton.padding.left = (icon != null ? 22 : padding);

                if(GUI.Button(rect, element.label, m_Styles.elementButton)){
                    if(element.children.Count == 0){
                        if(onAddCallback != null){
                            onAddCallback(element.type);
                        }

                        Close();
                    }
                    else{
                        m_SelectedElement = element;
                    }
                }

                GUI.backgroundColor = backgroundColor;
                m_Styles.elementButton.normal.textColor = textColor;
                m_Styles.elementButton.padding.left = padding;

                if(icon != null){
                    GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), icon);
                }

                if(element.children.Count > 0){
                    GUI.Label(new Rect(rect.x + rect.width - 16f, rect.y + 2f, 16f, 16f), "", m_Styles.rightArrow);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private bool SearchMatch(Element element)
        {
            if(isSearching && (element.type == null || element.type.IsAbstract ||
                               !m_SearchString.ToLower().Split(' ').All(element.type.Name.ToLower().Contains))){
                return false;
            }

            return true;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach(var it in interfaceTypes){
                if(it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if(givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if(baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        private Element BuildElements()
        {
            Element root = new Element(ObjectNames.NicifyVariableName(m_Type.Name), "");

            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(assembly => assembly.GetTypes())
                                    .Where(type => (IsAssignableToGenericType(type, m_Type) ||
                                                    m_Type.IsAssignableFrom(type)) && !type.IsAbstract &&
                                                   !type.HasAttribute(typeof(ExcludeFromCreation)))
                                    .ToArray();
            // Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(c => c.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == this.m_Type)).ToArray();
            types = types.OrderBy(x => x.BaseType.Name).ToArray();

            foreach(Type type in types){
                ComponentMenu attribute = type.GetCustomAttribute<ComponentMenu>();

                string menu = attribute != null ? attribute.componentMenu : string.Empty;

                if(string.IsNullOrEmpty(menu)){
                    Element element = new Element(ObjectNames.NicifyVariableName(type.Name), menu);
                    element.type = type;
                    element.parent = root;
                    root.children.Add(element);
                }

                menu = menu.Replace("/", ".");
                string[] s = menu.Split('.');

                Element prev = null;
                string cur = string.Empty;

                for(int i = 0; i < s.Length - 1; i++){
                    cur += (string.IsNullOrEmpty(cur) ? "" : ".") + s[i];
                    Element parent = root.Find(cur);

                    if(parent == null){
                        parent = new Element(s[i], cur);

                        if(prev != null){
                            parent.parent = prev;
                            prev.children.Add(parent);
                        }
                        else{
                            parent.parent = root;
                            root.children.Add(parent);
                        }
                    }

                    prev = parent;
                }

                if(prev != null){
                    Element element = new Element(ObjectNames.NicifyVariableName(type.Name), menu);
                    element.type = type;
                    element.parent = prev;
                    prev.children.Add(element);
                }
            }

            root.children = root.children.OrderByDescending(x => x.children.Count).ToList();

            Element newScript = new Element("New script", "");
            newScript.parent = root;
            Element script = new Element(ObjectNames.NicifyVariableName(m_Type.Name),
                                         "New script." + ObjectNames.NicifyVariableName(m_Type.Name));
            script.parent = newScript;
            script.type = m_Type;
            script.onGUI = delegate()
                               {
                                   GUILayout.Label("Name");
                                   GUI.SetNextControlName("AddAssetNewScript");
                                   m_NewScriptName = GUILayout.TextField(m_NewScriptName);
                                   GUI.FocusControl("AddAssetNewScript");
                                   GUILayout.FlexibleSpace();
                                   EditorGUI.BeginDisabledGroup(onCreateCallback == null ||
                                                                string.IsNullOrEmpty(m_NewScriptName));

                                   if(GUILayout.Button("Create and add") || Event.current.keyCode == KeyCode.Return){
                                       onCreateCallback(m_NewScriptName);
                                       Close();
                                   }

                                   EditorGUI.EndDisabledGroup();
                               };
            newScript.children.Add(script);
            root.children.Add(newScript);

            return root;
        }

        private Element[] GetAllElements(Element root)
        {
            List<Element> elements = new List<Element>();
            GetElements(root, ref elements);
            return elements.ToArray();
        }

        private void GetElements(Element current, ref List<Element> list)
        {
            list.Add(current);

            for(int i = 0; i < current.children.Count; i++){
                GetElements(current.children[i], ref list);
            }
        }

        private string SearchField(string search, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            string before = search;

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, "ToolbarSeachTextField", options);
            rect.x += 2f;
            rect.width -= 2f;
            Rect buttonRect = rect;
            buttonRect.x = rect.width - 14;
            buttonRect.width = 14;

            if(!String.IsNullOrEmpty(before))
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);

            if(Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition) ||
               before == "Search..." && GUI.GetNameOfFocusedControl() == "SearchTextFieldFocus"){
                before = "";
                GUI.changed = true;
                GUI.FocusControl(null);
            }

            GUI.SetNextControlName("SearchTextFieldFocus");
            GUIStyle style = new GUIStyle("ToolbarSeachTextField");

            if(before == "Search..."){
                style.normal.textColor = Color.gray;
                style.hover.textColor = Color.gray;
            }

            string after = EditorGUI.TextField(rect, "", before, style);
            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            GUI.Button(buttonRect, GUIContent.none,
                       (after != "" && after != "Search...")
                           ? "ToolbarSeachCancelButton"
                           : "ToolbarSeachCancelButtonEmpty");
            EditorGUILayout.EndHorizontal();
            return after;
        }

        private static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector.x;
            guiRect.y = vector.y;
            return guiRect;
        }

        public class Element
        {
            public Type type;
            public Element parent;
            public System.Action onGUI;

            private string m_Path;

            public string path => m_Path;

            private GUIContent m_Label;

            public GUIContent label
            {
                get => m_Label;
                set => m_Label = value;
            }

            public Element(string label, string path)
            {
                this.label = new GUIContent(label);
                m_Path = path;
            }

            private List<Element> m_children;

            public List<Element> children
            {
                get{
                    if(m_children == null){
                        m_children = new List<Element>();
                    }

                    return m_children;
                }
                set => m_children = value;
            }

            public bool Contains(Element item)
            {
                if(item.label.text == label.text){
                    return true;
                }

                for(int i = 0; i < children.Count; i++){
                    bool contains = children[i].Contains(item);

                    if(contains){
                        return true;
                    }
                }

                return false;
            }

            public Element Find(string path)
            {
                if(this.path == path){
                    return this;
                }

                for(int i = 0; i < children.Count; i++){
                    Element tree = children[i].Find(path);

                    if(tree != null){
                        return tree;
                    }
                }

                return null;
            }
        }

        private class Styles
        {
            public GUIStyle header = new GUIStyle("DD HeaderStyle");
            public GUIStyle rightArrow = "AC RightArrow";
            public GUIStyle leftArrow = "AC LeftArrow";
            public GUIStyle elementButton = new GUIStyle("MeTransitionSelectHead");
            public GUIStyle background = "grey_border";

            public Styles()
            {
                header.stretchWidth = true;
                header.margin = new RectOffset(1, 1, 0, 4);

                elementButton.alignment = TextAnchor.MiddleLeft;
                elementButton.padding.left = 22;
                elementButton.margin = new RectOffset(1, 1, 0, 0);
                elementButton.normal.textColor = EditorGUIUtility.isProSkin
                                                     ? new Color(0.788f, 0.788f, 0.788f, 1f)
                                                     : new Color(0.047f, 0.047f, 0.047f, 1f);
            }
        }
    }
}