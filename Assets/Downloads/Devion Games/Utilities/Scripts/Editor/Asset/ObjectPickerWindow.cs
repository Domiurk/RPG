using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    public class ObjectPickerWindow : EditorWindow
    {
        private static Styles m_Styles;
        private string m_SearchString = string.Empty;
        private bool isSearching => !string.IsNullOrEmpty(m_SearchString);

        private Vector2 m_ScrollPosition;
        private Type m_Type;
        private bool m_SelectChildren = false;
        private UnityEngine.Object m_Root;
        private Dictionary<UnityEngine.Object, List<UnityEngine.Object>> m_SelectableObjects;
        public delegate void SelectCallbackDelegate(UnityEngine.Object obj);
        public SelectCallbackDelegate onSelectCallback;
        public delegate void CreateCallbackDelegate();
        public CreateCallbackDelegate onCreateCallback;
        private bool m_AcceptNull;

        public static void ShowWindow<T>(Rect buttonRect, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull=false)
        {
            ShowWindow(buttonRect, typeof(T), selectCallback, createCallback,acceptNull);
        }

        public static void ShowWindow(Rect buttonRect, Type type, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull=false)
        {
            ObjectPickerWindow window = CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window.m_Type = type;
            window.BuildSelectableObjects(type);
            window.onSelectCallback = selectCallback;
            window.onCreateCallback = createCallback;
            window.m_AcceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        public static void ShowWindow(Rect buttonRect,Type type, Dictionary<UnityEngine.Object,List<UnityEngine.Object>> selectableObjects, SelectCallbackDelegate selectCallback, CreateCallbackDelegate createCallback, bool acceptNull=false)
        {
            ObjectPickerWindow window = CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window.m_SelectableObjects = selectableObjects;
            window.m_Type = type;
            window.m_SelectChildren = true;
            window.onSelectCallback = selectCallback;
            window.onCreateCallback = createCallback;
            window.m_AcceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (m_Styles == null)
            {
                m_Styles = new Styles();
            }
            GUILayout.Space(5f);
            m_SearchString = SearchField(m_SearchString);
            Header();

            DrawSelectableObjects();

            if (Event.current.type == EventType.Repaint)
            {
                m_Styles.background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
            }
        }

        private void Header()
        {
            GUIContent content = new GUIContent(m_Root==null?"Select " +ObjectNames.NicifyVariableName(m_Type.Name):m_Root.name);
            Rect headerRect = GUILayoutUtility.GetRect(content, m_Styles.header);
            if (GUI.Button(headerRect, content, m_Styles.header))
            {
                m_Root = null;
            }
        }

        private void DrawSelectableObjects()
         {
            List<UnityEngine.Object> selectableObjects = m_Root == null ? m_SelectableObjects.Keys.ToList() : m_SelectableObjects[m_Root];

             m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
             foreach (UnityEngine.Object obj in selectableObjects)
             {
                 if (!SearchMatch(obj))
                     continue;

                 Color backgroundColor = GUI.backgroundColor;
                 Color textColor = m_Styles.elementButton.normal.textColor;
                 int padding = m_Styles.elementButton.padding.left;
                 GUIContent label = new GUIContent(obj.name);
                 Rect rect = GUILayoutUtility.GetRect(label, m_Styles.elementButton, GUILayout.Height(20f));
                 GUI.backgroundColor = (rect.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));
                 m_Styles.elementButton.normal.textColor = (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);

                Texture2D icon = EditorGUIUtility.LoadRequired("d_ScriptableObject Icon") as Texture2D;
                IconAttribute iconAttribute = obj.GetType().GetCustomAttribute<IconAttribute>();
                if (iconAttribute != null)
                {
                    if (iconAttribute.type != null)
                    {
                        icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.type);
                    }
                    else
                    {
                        icon = Resources.Load<Texture2D>(iconAttribute.path);
                    }
                }

                m_Styles.elementButton.padding.left = (icon != null ? 22 : padding);


                 if (GUI.Button(rect, label, m_Styles.elementButton))
                 {
                    if (m_Root != null && m_SelectableObjects[m_Root].Count > 0)
                    {
                        onSelectCallback?.Invoke(obj);
                        Close();
                    }
                    m_Root = obj;
                    if (!m_SelectChildren)
                    {
                        onSelectCallback?.Invoke(m_Root);
                        Close();
                    }

                }
                 GUI.backgroundColor = backgroundColor;
                 m_Styles.elementButton.normal.textColor = textColor;
                 m_Styles.elementButton.padding.left = padding;

                 if (icon != null)
                 {
                     GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), icon);
                 }
             }

            if (m_Root == null)
            {
                if (m_AcceptNull)
                {
                    GUIContent nullContent = new GUIContent("Null");
                    Rect rect2 = GUILayoutUtility.GetRect(nullContent, m_Styles.elementButton, GUILayout.Height(20f));
                    GUI.backgroundColor = (rect2.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

                    if (GUI.Button(rect2, nullContent, m_Styles.elementButton))
                    {
                        onSelectCallback?.Invoke(null);
                        Close();
                    }
                    GUI.Label(new Rect(rect2.x, rect2.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);
                }

                GUIContent createContent = new GUIContent("Create New " + m_Type.Name);
                Rect rect1 = GUILayoutUtility.GetRect(createContent, m_Styles.elementButton, GUILayout.Height(20f));
                GUI.backgroundColor = (rect1.Contains(Event.current.mousePosition) ? GUI.backgroundColor : new Color(0, 0, 0, 0.0f));

                if (GUI.Button(rect1, createContent, m_Styles.elementButton))
                {
                    onCreateCallback?.Invoke();
                    Close();
                }
                GUI.Label(new Rect(rect1.x, rect1.y, 20f, 20f), EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);


            }
            EditorGUILayout.EndScrollView();
        }

        private void BuildSelectableObjects(Type type) {
            m_SelectableObjects = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();

            string[] guids = AssetDatabase.FindAssets("t:"+type.Name);
            for(int i = 0; i < guids.Length; i++) { 
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path,type);
                m_SelectableObjects.Add(obj, new List<UnityEngine.Object>());
            }
        }

        private bool SearchMatch(UnityEngine.Object element)
        {
            if (isSearching && (element == null || !element.name.ToLower().Contains(m_SearchString.ToLower())))
            {
                return false;
            }
            return true;
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

            if (!string.IsNullOrEmpty(before))
                EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Arrow);

            if (Event.current.type == EventType.MouseUp && buttonRect.Contains(Event.current.mousePosition) || before == "Search..." && GUI.GetNameOfFocusedControl() == "SearchTextFieldFocus")
            {
                before = "";
                GUI.changed = true;
                GUI.FocusControl(null);

            }
            GUI.SetNextControlName("SearchTextFieldFocus");
            GUIStyle style = new GUIStyle("ToolbarSeachTextField");
            if (before == "Search...")
            {
                style.normal.textColor = Color.gray;
                style.hover.textColor = Color.gray;
            }
            string after = EditorGUI.TextField(rect, "", before, style);
            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            GUI.Button(buttonRect, GUIContent.none, (after != "" && after != "Search...") ? "ToolbarSeachCancelButton" : "ToolbarSeachCancelButtonEmpty");
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
                elementButton.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.788f, 0.788f, 0.788f, 1f) : new Color(0.047f, 0.047f, 0.047f, 1f);
            }
        }
    }
}