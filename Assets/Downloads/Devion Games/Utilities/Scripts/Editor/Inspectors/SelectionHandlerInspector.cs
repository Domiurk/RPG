using UnityEditor;
using UnityEditor.AnimatedValues;

namespace DevionGames
{
    [CustomEditor(typeof(SelectionHandler),true)]
    public class SelectionHandlerInspector : CallbackHandlerInspector
    {

        private SerializedProperty m_SelectionType;
        private SerializedProperty m_SelectionDistance;
        private SerializedProperty m_SelectionKey;
        private SerializedProperty m_RaycastOffset;
        private SerializedProperty m_LayerMask;
        private AnimBool m_RaycastOffsetOptions;
        private AnimBool m_SelectionKeyOptions;

        private SerializedProperty m_DeselectionType;
        private SerializedProperty m_DeselectionKey;
        private SerializedProperty m_DeselectionDistance;
        private AnimBool m_DeselectionKeyOptions;
        private AnimBool m_DeselectionDistanceOptions;

        protected override void OnEnable()
        {
            base.OnEnable();
         
            m_SelectionType = serializedObject.FindProperty("selectionType");
            m_SelectionDistance = serializedObject.FindProperty("m_SelectionDistance");
            m_SelectionKey = serializedObject.FindProperty("m_SelectionKey");
            m_RaycastOffset = serializedObject.FindProperty("m_RaycastOffset");
            m_LayerMask = serializedObject.FindProperty("m_LayerMask");
            if (m_RaycastOffsetOptions == null)
            {
                m_RaycastOffsetOptions = new AnimBool((target as SelectionHandler).selectionType.HasFlag<SelectionHandler.SelectionInputType>(SelectionHandler.SelectionInputType.Raycast));
                m_RaycastOffsetOptions.valueChanged.AddListener(Repaint);
            }

            if (m_SelectionKeyOptions == null)
            {
                m_SelectionKeyOptions = new AnimBool((target as SelectionHandler).selectionType.HasFlag<SelectionHandler.SelectionInputType>(SelectionHandler.SelectionInputType.Key));
                m_SelectionKeyOptions.valueChanged.AddListener(Repaint);
            }

            m_DeselectionType = serializedObject.FindProperty("deselectionType");
            m_DeselectionKey = serializedObject.FindProperty("m_DeselectionKey");
            m_DeselectionDistance = serializedObject.FindProperty("m_DeselectionDistance");
            if (m_DeselectionKeyOptions == null)
            {
                m_DeselectionKeyOptions = new AnimBool((target as SelectionHandler).deselectionType.HasFlag<SelectionHandler.DeselectionInputType>(SelectionHandler.DeselectionInputType.Key));
                m_DeselectionKeyOptions.valueChanged.AddListener(Repaint);
            }
            if (m_DeselectionDistanceOptions == null)
            {
                m_DeselectionDistanceOptions = new AnimBool((target as SelectionHandler).deselectionType.HasFlag<SelectionHandler.DeselectionInputType>(SelectionHandler.DeselectionInputType.Distance));
                m_DeselectionDistanceOptions.valueChanged.AddListener(Repaint);
            }
        }

        private void DrawInspector()
        {
            EditorGUILayout.PropertyField(m_SelectionType);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(m_LayerMask);
            EditorGUILayout.PropertyField(m_SelectionDistance);

            m_SelectionKeyOptions.target = (target as SelectionHandler).selectionType.HasFlag<SelectionHandler.SelectionInputType>(SelectionHandler.SelectionInputType.Key);
            if (EditorGUILayout.BeginFadeGroup(m_SelectionKeyOptions.faded))
            {
                EditorGUILayout.PropertyField(m_SelectionKey);
            }
            EditorGUILayout.EndFadeGroup();

            m_RaycastOffsetOptions.target = (target as SelectionHandler).selectionType.HasFlag<SelectionHandler.SelectionInputType>(SelectionHandler.SelectionInputType.Raycast);
            if (EditorGUILayout.BeginFadeGroup(m_RaycastOffsetOptions.faded))
            {
                EditorGUILayout.PropertyField(m_RaycastOffset);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel -= 1;


            EditorGUILayout.PropertyField(m_DeselectionType);
            EditorGUI.indentLevel += 1;
            m_DeselectionKeyOptions.target = (target as SelectionHandler).deselectionType.HasFlag<SelectionHandler.DeselectionInputType>(SelectionHandler.DeselectionInputType.Key);
            if (EditorGUILayout.BeginFadeGroup(m_DeselectionKeyOptions.faded))
            {
                EditorGUILayout.PropertyField(m_DeselectionKey);
            }
            EditorGUILayout.EndFadeGroup();

            m_DeselectionDistanceOptions.target = (target as SelectionHandler).deselectionType.HasFlag<SelectionHandler.DeselectionInputType>(SelectionHandler.DeselectionInputType.Distance);
            if (EditorGUILayout.BeginFadeGroup(m_DeselectionDistanceOptions.faded))
            {
                EditorGUILayout.PropertyField(m_DeselectionDistance);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel -= 1;
        }
    }
}