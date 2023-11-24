using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Canvas))]
    [ComponentMenu("UI/Show Progressbar")]
    [System.Serializable]
    public class ShowProgressbar : Action
    {
        [SerializeField]
        private string m_WidgetName = "General Progressbar";
        [SerializeField]
        private string m_Title = "";
        [SerializeField]
        private float  m_Duration = 1f;

        private float m_Time;

        private Progressbar m_Widget;

        public override void OnStart()
        {
            m_Time = 0f;
            m_Widget = WidgetUtility.Find<Progressbar>(m_WidgetName);
            if (m_Widget == null)
            {
                Debug.LogWarning("Missing progressbar widget " + m_WidgetName + " in scene!");
                return;
            }
            m_Widget.Show(m_Title);
        }

        public override ActionStatus OnUpdate()
        {

            if (m_Widget == null) {
                Debug.LogWarning("Missing progressbar widget " + m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }

            m_Time += Time.deltaTime;
            if (m_Time > m_Duration)
            {
                m_Widget.Close();
                return ActionStatus.Success;
            }
            m_Widget.SetProgress(m_Time / m_Duration);
            return ActionStatus.Running;
        }

        public override void OnInterrupt()
        {
            if (m_Widget != null)
                m_Widget.Close();
        }
    }
}