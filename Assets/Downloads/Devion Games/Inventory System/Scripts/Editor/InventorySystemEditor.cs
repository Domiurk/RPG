using UnityEngine;
using UnityEditor;

namespace DevionGames.InventorySystem
{
	public class InventorySystemEditor : EditorWindow
	{

		private InventorySystemInspector m_InventorySystemInspector;

		public static void ShowWindow ()
		{
	
			InventorySystemEditor[] objArray = Resources.FindObjectsOfTypeAll<InventorySystemEditor> ();
			InventorySystemEditor editor = (objArray.Length <= 0 ? CreateInstance<InventorySystemEditor> () : objArray [0]);

			editor.hideFlags = HideFlags.HideAndDontSave;
			editor.minSize = new Vector2 (690, 300);
			editor.titleContent = new GUIContent ("Inventory System");

			editor.Show();
		}

		private void OnEnable()
		{
			m_InventorySystemInspector = new InventorySystemInspector();
			m_InventorySystemInspector.OnEnable();
		}

		private void OnDisable()
		{
			m_InventorySystemInspector.OnDisable();
		}

		private void OnDestroy()
		{
			m_InventorySystemInspector.OnDestroy();
		}

		private void Update()
		{
			if (mouseOverWindow == this)
				Repaint();
		}

		private void OnGUI()
		{
			m_InventorySystemInspector.OnGUI(position);
		}

	}
}