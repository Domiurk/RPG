using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace DevionGames.UIWidgets
{
    [CustomEditor(typeof(UIWidget), true)]
    public class UIWidgetInspector : CallbackHandlerInspector
    {
        protected CanvasGroup canvasGroup;

        private string[] m_WidgetPropertiesToExcludeForDefaultInspector;
        private AnimBool m_ShowAndHideOptions;
        private SerializedProperty m_ShowAndHideCursor;
        private SerializedProperty m_CloseOnMove;
        private SerializedProperty m_Deactivate;
        private SerializedProperty m_FocusPlayer;

        protected override void OnEnable()
        {
            base.OnEnable();
            canvasGroup = ((UIWidget)target).GetComponent<CanvasGroup>();

            m_ShowAndHideCursor = serializedObject.FindProperty("m_ShowAndHideCursor");
            m_CloseOnMove = serializedObject.FindProperty("m_CloseOnMove");
            m_Deactivate = serializedObject.FindProperty("m_Deactivate");
            m_FocusPlayer = serializedObject.FindProperty("m_FocusPlayer");

            m_ShowAndHideOptions = new AnimBool(m_ShowAndHideCursor.boolValue);
            m_ShowAndHideOptions.valueChanged.AddListener(Repaint);

            m_WidgetPropertiesToExcludeForDefaultInspector = new[]{
                m_ShowAndHideCursor.propertyPath,
                m_CloseOnMove.propertyPath,
                m_Deactivate.propertyPath,
                m_FocusPlayer.propertyPath
            };
        }

        private void DrawInspector()
        {
            DrawTypePropertiesExcluding(typeof(UIWidget), m_WidgetPropertiesToExcludeForDefaultInspector);
            EditorGUILayout.PropertyField(m_ShowAndHideCursor);
            m_ShowAndHideOptions.target = m_ShowAndHideCursor.boolValue;

            if(EditorGUILayout.BeginFadeGroup(m_ShowAndHideOptions.faded)){
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_Deactivate);
                EditorGUILayout.PropertyField(m_CloseOnMove);
                EditorGUILayout.PropertyField(m_FocusPlayer);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }

            EditorGUILayout.EndFadeGroup();
        }

        protected virtual void OnSceneGUI()
        {
            if(canvasGroup == null){
                return;
            }

            Handles.BeginGUI();
            Rect rect = Camera.current.pixelRect;

            if(GUI.Button(new Rect(rect.width - 110f, rect.height - 30f, 100f, 20f),
                          canvasGroup.alpha > 0.1f ? "Hide" : "Show")){
                if(canvasGroup.alpha > 0.1f){
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
                else{
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }

                EditorUtility.SetDirty(canvasGroup);
            }

            Handles.EndGUI();
        }
    }
}