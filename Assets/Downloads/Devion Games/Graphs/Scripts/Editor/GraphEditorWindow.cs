using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevionGames.Graphs
{
	[Serializable]
    public class GraphEditorWindow : EditorWindow
    {
		public static GraphEditorWindow ShowWindow()
		{
			GraphEditorWindow window = GetWindow<GraphEditorWindow>(false, "Graph Editor");
			window.minSize = new Vector2(500f, 100f);
			return window;
		}

		[SerializeReference]
		private IGraphView m_GraphView;
		private UnityEngine.Object m_TargetObject;
		private IGraphProvider m_Behavior;
		private string[] commandNames = new string[] {"OnStartDrag", "OnEndDrag" };


		private void OnEnable()
        {
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private void OnDisable()
		{
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
		}


		protected virtual void OnPlayModeStateChanged(PlayModeStateChange state)
        {
			if (m_TargetObject is IGraphProvider)
			{
				Load(m_GraphView.GetType(), m_TargetObject as IGraphProvider, m_TargetObject);
			}else {
				Close();
			}
		}

		protected virtual void OnAfterAssemblyReload()
		{
			if (m_TargetObject is IGraphProvider)
			{
				Load(m_GraphView.GetType(), m_TargetObject as IGraphProvider, m_TargetObject);
			}
			else
			{
				Close();
			}
		}

		private void Update()
        {
			Repaint();
        }

        private void OnGUI()
		{
			m_GraphView.OnGUI(new Rect(0, 0, position.width, position.height));
			Event currentEvent = Event.current;
			if (currentEvent.type == EventType.ValidateCommand && commandNames.Contains(currentEvent.commandName))
			{
				currentEvent.Use();
			}
			if (currentEvent.type == EventType.ExecuteCommand)
			{
				string command = currentEvent.commandName;
				switch (command)
				{
					case "OnStartDrag":
						break;
					case "OnEndDrag":
						GraphUtility.Save(m_Behavior.GetGraph());
						PrefabUtility.RecordPrefabInstancePropertyModifications(m_TargetObject);
						break;
				}
			}

			if (Event.current.type == EventType.MouseMove)
				Repaint();
		}


		public void Load<T>(IGraphProvider behavior, UnityEngine.Object target)
		{
			Load(typeof(T), behavior, target);
		}

		public void Load(Type type,IGraphProvider behavior, UnityEngine.Object target)
		{
			if (behavior == null)
				Close();
			m_TargetObject = target;
			m_Behavior = behavior;
			m_GraphView = Activator.CreateInstance(type, this, behavior.GetGraph(), m_TargetObject) as IGraphView;
			EditorUtility.SetDirty(this);
			titleContent = new GUIContent(ObjectNames.NicifyVariableName(behavior.GetGraph().GetType().Name) + " Editor");
			m_GraphView.CenterGraphView();
			Repaint();
		}

	}
}