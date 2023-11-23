using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames
{
	public class CopyComponentEditor : EditorWindow
	{
		private GameObject m_Source;
		private GameObject m_Destination;

		[MenuItem("Tools/Devion Games/Internal/Copy Components", false)]
		public static void ShowWindow()
		{
			CopyComponentEditor window = GetWindow<CopyComponentEditor>("Copy Components");
			Vector2 size = new Vector2(300f, 80f);
			window.minSize = size;
			window.wantsMouseMove = true;
		}

		private Vector2 m_ScrollPosition;
		private Dictionary<Component, bool> m_ComponentMap;

        private void OnGUI()
        {
			m_Source = EditorGUILayout.ObjectField("Source",m_Source, typeof(GameObject),true) as GameObject;
			m_Destination= EditorGUILayout.ObjectField("Destination",m_Destination, typeof(GameObject), true) as GameObject;
			if (m_Source == null || m_Destination == null)
				return;

			if (m_ComponentMap == null)
			{
				m_ComponentMap = new Dictionary<Component, bool>();
				Component[] components = m_Source.GetComponents<Component>().Where(x => x.hideFlags == HideFlags.None).ToArray();
				for (int i = 0; i < components.Length; i++)
				{
					if (ComponentUtility.CopyComponent(components[i]))
					{
						m_ComponentMap.Add(components[i],true);
					}
				}
			}
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
			GUILayout.Label("Components", EditorStyles.boldLabel);
			for (int i = 0; i < m_ComponentMap.Count; i++) {
				m_ComponentMap[m_ComponentMap.ElementAt(i).Key] = EditorGUILayout.Toggle(m_ComponentMap.ElementAt(i).Key.GetType().Name, m_ComponentMap.ElementAt(i).Value);
			}
			EditorGUILayout.EndScrollView();

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Copy Components")) {
				foreach(KeyValuePair<Component,bool> kvp in m_ComponentMap)
				{
					if (kvp.Value && ComponentUtility.CopyComponent(kvp.Key))
					{
						Component component = m_Destination.AddComponent(kvp.Key.GetType()) as Component;
						ComponentUtility.PasteComponentValues(component);
					}
				}
				Selection.activeObject = m_Destination;
			}

		}

		
	}
}