using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Slot),true)]
    public class SlotInspector : CallbackHandlerInspector
    {

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();
            for (int i = 0; i < m_DrawInspectors.Count; i++)
            {
                m_DrawInspectors[i].Invoke();
            }

            DrawPropertiesExcluding(serializedObject, m_PropertiesToExcludeForChildClasses);
            if (EditorTools.RightArrowButton(new GUIContent("Restrictions", "Slot restrictions")))
            {
                AssetWindow.ShowWindow("Slot Restrictions", serializedObject.FindProperty("restrictions"));
            }
            TriggerGUI();
            serializedObject.ApplyModifiedProperties();
            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Repaint();
            }
        }
    }
}