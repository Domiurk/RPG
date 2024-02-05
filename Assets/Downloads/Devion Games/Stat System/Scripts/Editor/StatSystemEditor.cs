using UnityEngine;
using UnityEditor;

namespace DevionGames.StatSystem
{
	public class StatSystemEditor : EditorWindow
	{

		private StatSystemInspector m_StatSystemInspector;

		public static void ShowWindow()
		{

			StatSystemEditor[] objArray = Resources.FindObjectsOfTypeAll<StatSystemEditor>();
			StatSystemEditor editor = (objArray.Length <= 0 ? CreateInstance<StatSystemEditor>() : objArray[0]);

			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2(690, 300);
			editor.titleContent = new GUIContent("Stat System");

			editor.Show();
		}

		private void OnEnable()
		{
			m_StatSystemInspector = new StatSystemInspector();
			m_StatSystemInspector.OnEnable();
		}

		private void OnDisable()
		{
			m_StatSystemInspector.OnDisable();
		}

		private void OnDestroy()
		{
			m_StatSystemInspector.OnDestroy();
		}

		private void Update()
		{
			if (mouseOverWindow == this)
				Repaint();
		}

		private void OnGUI()
		{
			m_StatSystemInspector.OnGUI(position);
		}

	}
}