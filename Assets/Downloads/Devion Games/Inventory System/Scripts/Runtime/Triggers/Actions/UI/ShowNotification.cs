using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Canvas))]
    [ComponentMenu("UI/Show Notification")]
    [System.Serializable]
    public class ShowNotification : Action
    {
        [SerializeField]
        private string m_WidgetName = "Notification";
        [SerializeField]
        private NotificationOptions m_Notification;

        public override ActionStatus OnUpdate()
        {
            Notification widget = WidgetUtility.Find<Notification>(m_WidgetName);
            if (widget == null)
            {
                Debug.LogWarning("Missing notification widget " + m_WidgetName + " in scene!");
                return ActionStatus.Failure;
            }
            return widget.AddItem(m_Notification)?ActionStatus.Success:ActionStatus.Failure;
        }
    }
}