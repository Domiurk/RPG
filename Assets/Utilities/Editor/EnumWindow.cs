using System;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor
{
    public class EnumWindow : EditorWindow
    {
        public event Action<int> ChangeKeyEvent;
        private bool IsSearching => !string.IsNullOrEmpty(_searchString);
        
        private string _searchString = string.Empty;
        private SerializedProperty _property;
        private Type _type;
        private Vector2 _scrollView;
        private Rect _rect;

        public static void Show<T>(Rect rect, Action<int> callback, SerializedProperty property) where T : Enum
        {
            Show(rect, typeof(T), callback, property);
        }

        public static void Show(Rect rect, Type type, Action<int> callback, SerializedProperty property)
        {
            EnumWindow window = CreateInstance<EnumWindow>();
            rect = GUIToScreenRect(rect);
            window.position = rect;
            window.ChangeKeyEvent = callback;
            window._type = type;
            window._property = property;
            window._rect = rect;
            // window.ShowAsDropDown(rect, new Vector2(rect.width, 200f));
            float ySize = Mathf.Clamp(rect.height * (property.enumDisplayNames.Length + 2), 1, 200);
            window.ShowAsDropDown(rect, new Vector2(rect.width, ySize));
        }

        private void OnGUI()
        {
            _searchString = SearchField(_searchString);
            Body();
        }

        private void Body()
        {
            _scrollView = EditorGUILayout.BeginScrollView(_scrollView);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            Array values = Enum.GetValues(_type);

            for(int i = 0; i < values.Length; i++){
                object value = values.GetValue(i);

                if(!SearchMatch(value.ToString()))
                    continue;

                if(GUI.Button(EditorGUILayout.GetControlRect(GUILayout.Width(_rect.width / 1.5f), GUILayout.ExpandWidth(true)),
                              value.ToString())){
                    _property.enumValueIndex = i;
                    ChangeKeyEvent?.Invoke(_property.enumValueIndex);
                    _property.serializedObject.ApplyModifiedProperties();
                    Close();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private static Rect GUIToScreenRect(Rect guiRect)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
            guiRect.x = vector.x;
            guiRect.y = vector.y;
            return guiRect;
        }

        private bool SearchMatch(string element)
            => !IsSearching || (element != null &&
                                (element.ToLower().Contains(this._searchString.ToLower()) &&
                                 _filterType == SearchType.aa ||
                                 element.Contains(_searchString) && _filterType == SearchType.Aa));

        private string SearchField(string search, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            string before = search;

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.textField, options);
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

            float buttonWidth = 50;
            float space = 5;
            rect.width -= buttonWidth + space * 2;
            string after = EditorGUI.TextField(rect, "", before, style);
            Rect newButtonRect = new Rect(rect.x + rect.width + space, rect.y, buttonWidth, rect.height);
            if(GUI.Button(newButtonRect, _filterType.ToString(), GUI.skin.button)){
                _filterType = _filterType switch{
                    SearchType.Aa => SearchType.aa,
                    SearchType.aa => SearchType.Aa,
                    _ => SearchType.aa
                };
            }

            EditorGUI.FocusTextInControl("SearchTextFieldFocus");

            EditorGUILayout.EndHorizontal();
            return after;
        }

        private SearchType _filterType;

        private enum SearchType
        {
            aa,
            Aa
        }
    }
}