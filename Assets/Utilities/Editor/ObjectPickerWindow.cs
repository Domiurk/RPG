using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using IconAttribute = Utilities.Runtime.Attributes.IconAttribute;
using Object = UnityEngine.Object;

namespace Utilities.Editor
{
    public class ObjectPickerWindow : EditorWindow
    {
        public delegate void SelectCallbackDelegate(Object obj);
        public SelectCallbackDelegate OnSelectCallback;

        public delegate void CreateCallbackDelegate();
        public CreateCallbackDelegate OnCreateCallback;
        
        private bool IsSearching => !string.IsNullOrEmpty(_searchString);
        
        private static Styles _styles;
        private string _searchString = string.Empty;
        private bool _selectChildren;
        private bool _acceptNull;
        private Vector2 _scrollPosition;
        private Object _root;
        private Type _type;
        private Dictionary<Object, List<Object>> _selectableObjects;

        public static void ShowWindow<T>(Rect buttonRect,
                                         SelectCallbackDelegate selectCallback,
                                         CreateCallbackDelegate createCallback,
                                         bool acceptNull = false)
            => ShowWindow(buttonRect, typeof(T), selectCallback, createCallback, acceptNull);

        public static void ShowWindow(Rect buttonRect,
                                      Type type,
                                      SelectCallbackDelegate selectCallback,
                                      CreateCallbackDelegate createCallback,
                                      bool acceptNull = false)
        {
            ObjectPickerWindow window = CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window._type = type;
            window.BuildSelectableObjects(type);
            window.OnSelectCallback = selectCallback;
            window.OnCreateCallback = createCallback;
            window._acceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        public static void ShowWindow(Rect buttonRect,
                                      Type type,
                                      Dictionary<Object, List<Object>> selectableObjects,
                                      SelectCallbackDelegate selectCallback,
                                      CreateCallbackDelegate createCallback,
                                      bool acceptNull = false)
        {
            ObjectPickerWindow window = CreateInstance<ObjectPickerWindow>();
            buttonRect = GUIToScreenRect(buttonRect);
            window._selectableObjects = selectableObjects;
            window._type = type;
            window._selectChildren = true;
            window.OnSelectCallback = selectCallback;
            window.OnCreateCallback = createCallback;
            window._acceptNull = acceptNull;
            window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 200f));
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            _styles ??= new Styles();

            GUILayout.Space(5f);
            _searchString = SearchField(_searchString);
            Header();

            DrawSelectableObjects();

            if(Event.current.type == EventType.Repaint){
                _styles.Background.Draw(new Rect(0, 0, position.width, position.height), false,
                                                            false, false, false);
            }
        }

        private void Header()
        {
            GUIContent content = new GUIContent(_root == null
                                                    ? "Select " + ObjectNames.NicifyVariableName(_type.Name)
                                                    : _root.name);
            Rect headerRect = GUILayoutUtility.GetRect(content, _styles.Header);

            if(GUI.Button(headerRect, content, _styles.Header)){
                _root = null;
            }
        }

        private void DrawSelectableObjects()
        {
            List<Object> selectableObjects =
                _root == null ? _selectableObjects.Keys.ToList() : _selectableObjects[_root];

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach(Object obj in selectableObjects){
                if(!SearchMatch(obj))
                    continue;

                Color backgroundColor = GUI.backgroundColor;
                Color textColor = _styles.ElementButton.normal.textColor;
                int padding = _styles.ElementButton.padding.left;
                GUIContent label = new GUIContent(obj.name);
                Rect rect = GUILayoutUtility.GetRect(label, _styles.ElementButton,
                                                     GUILayout.Height(20f));
                GUI.backgroundColor = (rect.Contains(Event.current.mousePosition)
                                           ? GUI.backgroundColor
                                           : new Color(0, 0, 0, 0.0f));
                _styles.ElementButton.normal.textColor =
                    (rect.Contains(Event.current.mousePosition) ? Color.white : textColor);

                Texture2D icon = EditorGUIUtility.LoadRequired("d_ScriptableObject Icon") as Texture2D;
                IconAttribute iconAttribute = obj.GetType().GetCustomAttribute<IconAttribute>();

                if(iconAttribute != null){
                    if(iconAttribute.type != null){
                        icon = AssetPreview.GetMiniTypeThumbnail(iconAttribute.type);
                    }
                    else{
                        icon = Resources.Load<Texture2D>(iconAttribute.path);
                    }
                }

                _styles.ElementButton.padding.left = (icon != null ? 22 : padding);

                if(GUI.Button(rect, label, _styles.ElementButton)){
                    if(_root != null && _selectableObjects[_root].Count > 0){
                        OnSelectCallback?.Invoke(obj);
                        Close();
                    }

                    _root = obj;

                    if(!_selectChildren){
                        OnSelectCallback?.Invoke(_root);
                        Close();
                    }
                }

                GUI.backgroundColor = backgroundColor;
                _styles.ElementButton.normal.textColor = textColor;
                _styles.ElementButton.padding.left = padding;

                if(icon != null){
                    GUI.Label(new Rect(rect.x, rect.y, 20f, 20f), icon);
                }
            }

            if(_root == null){
                if(_acceptNull){
                    GUIContent nullContent = new GUIContent("Null");
                    Rect rect2 = GUILayoutUtility.GetRect(nullContent, _styles.ElementButton,
                                                          GUILayout.Height(20f));
                    GUI.backgroundColor = (rect2.Contains(Event.current.mousePosition)
                                               ? GUI.backgroundColor
                                               : new Color(0, 0, 0, 0.0f));

                    if(GUI.Button(rect2, nullContent, _styles.ElementButton)){
                        OnSelectCallback?.Invoke(null);
                        Close();
                    }

                    GUI.Label(new Rect(rect2.x, rect2.y, 20f, 20f),
                              EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);
                }

                GUIContent createContent = new GUIContent("Create New " + _type.Name);
                Rect rect1 = GUILayoutUtility.GetRect(createContent, _styles.ElementButton,
                                                      GUILayout.Height(20f));
                GUI.backgroundColor = (rect1.Contains(Event.current.mousePosition)
                                           ? GUI.backgroundColor
                                           : new Color(0, 0, 0, 0.0f));

                if(GUI.Button(rect1, createContent, _styles.ElementButton)){
                    OnCreateCallback?.Invoke();
                    Close();
                }

                GUI.Label(new Rect(rect1.x, rect1.y, 20f, 20f),
                          EditorGUIUtility.LoadRequired("d_ScriptableObject On Icon") as Texture2D);
            }

            EditorGUILayout.EndScrollView();
        }

        private void BuildSelectableObjects(Type type)
        {
            _selectableObjects = new Dictionary<Object, List<Object>>();

            string[] guids = AssetDatabase.FindAssets("t:" + type.Name);

            foreach(string guid in guids){
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath(path, type);
                _selectableObjects.Add(obj, new List<Object>());
            }
        }

        private bool SearchMatch(Object element)
            => !IsSearching || (element != null && element.name.ToLower().Contains(_searchString.ToLower()));

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

            if(!string.IsNullOrEmpty(before))
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

        private class Styles
        {
            public readonly GUIStyle Header = new("DD HeaderStyle");
            public GUIStyle RightArrow = "AC RightArrow";
            public GUIStyle LeftArrow = "AC LeftArrow";
            public readonly GUIStyle ElementButton = new GUIStyle("MeTransitionSelectHead");
            public readonly GUIStyle Background = "grey_border";

            public Styles()
            {
                Header.stretchWidth = true;
                Header.margin = new RectOffset(1, 1, 0, 4);

                ElementButton.alignment = TextAnchor.MiddleLeft;
                ElementButton.padding.left = 22;
                ElementButton.margin = new RectOffset(1, 1, 0, 0);
                ElementButton.normal.textColor = EditorGUIUtility.isProSkin
                                                     ? new Color(0.788f, 0.788f, 0.788f, 1f)
                                                     : new Color(0.047f, 0.047f, 0.047f, 1f);
            }
        }
    }
}