using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Raycast Notify")]
    public class RaycastNotify : Raycast
    {
        [SerializeField]
        protected NotificationOptions m_SuccessNotification;
        [SerializeField]
        protected NotificationOptions m_FailureNotification;


        public override ActionStatus OnUpdate()
        {
            if (DoRaycast()) {
                if (m_SuccessNotification != null && !string.IsNullOrEmpty(m_SuccessNotification.text))
                    m_SuccessNotification.Show();
                return ActionStatus.Success;
            }
            if (m_FailureNotification != null && !string.IsNullOrEmpty(m_FailureNotification.text))
                m_FailureNotification.Show();
            return ActionStatus.Failure;
        }

    }
}