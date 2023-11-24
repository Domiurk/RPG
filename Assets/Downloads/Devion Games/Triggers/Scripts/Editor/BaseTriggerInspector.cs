﻿using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace DevionGames
{
    [CustomEditor(typeof(BaseTrigger), true)]
    public class BaseTriggerInspector : CallbackHandlerInspector
    {
        private SerializedProperty m_UseDistance;
        private SerializedProperty m_TriggerInputType;
        private SerializedProperty m_TriggerKey;
        private AnimBool m_KeyOptions;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_UseDistance = serializedObject.FindProperty("useDistance");
            m_TriggerInputType = serializedObject.FindProperty("triggerType");
            m_TriggerKey = serializedObject.FindProperty("key");
            if (m_KeyOptions == null)
            {
                m_KeyOptions = new AnimBool((target as BaseTrigger).triggerType.HasFlag<BaseTrigger.TriggerInputType>(BaseTrigger.TriggerInputType.Key));
                m_KeyOptions.valueChanged.AddListener(Repaint);
            }
        }

        private void DrawInspector()
        {
            EditorGUILayout.PropertyField(m_UseDistance);
            EditorGUILayout.PropertyField(m_TriggerInputType);

            m_KeyOptions.target = (target as BaseTrigger).triggerType.HasFlag<BaseTrigger.TriggerInputType>(BaseTrigger.TriggerInputType.Key);
            if (EditorGUILayout.BeginFadeGroup(m_KeyOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_TriggerKey);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorApplication.isPlaying) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("In Range", (target as BaseTrigger).InRange);
                EditorGUILayout.Toggle("In Use", (target as BaseTrigger).InUse);
                EditorGUI.EndDisabledGroup();
                Repaint();
            }
        }

        protected virtual void OnSceneGUI()
        {
            BaseTrigger trigger = (BaseTrigger)target;
            if (!trigger.isActiveAndEnabled) return;

            Vector3 position = trigger.transform.position;

            Collider collider = trigger.GetComponent<Collider>();
            if (collider != null)
            {

                position = collider.bounds.center;
                position.y = (collider.bounds.center.y - collider.bounds.extents.y);
            }

            Color color = Handles.color;
            Color green = Color.green;
            green.a = 0.05f;
            Handles.color = green;
            Handles.DrawSolidDisc(position, Vector3.up, trigger.useDistance);
            Handles.color = Color.white;
            Handles.DrawWireDisc(position, Vector3.up, trigger.useDistance);
            Handles.color = color;
        }

    }
}