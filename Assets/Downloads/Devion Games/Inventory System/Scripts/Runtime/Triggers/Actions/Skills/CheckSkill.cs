using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Check Skill")]
    public class CheckSkill : Action, ICondition
    {
        [Tooltip("The name of the window to lock.")]
        [SerializeField]
        private string m_WindowName = "Skills";

        [ItemPicker(true)]
        [SerializeField]
        private Skill m_Skill;
        [SerializeField]
        private NotificationOptions m_SuccessNotification;
        [SerializeField]
        private NotificationOptions m_FailureNotification;

        private ItemContainer m_ItemContainer;

        public override void OnStart()
        {
            m_ItemContainer = WidgetUtility.Find<ItemContainer>(m_WindowName);
        }

        public override ActionStatus OnUpdate()
        {
            if (m_ItemContainer == null)
            {
                Debug.LogWarning("Missing window " + m_WindowName + " in scene!");
                return ActionStatus.Failure;
            }

            Skill current = (Skill)m_ItemContainer.GetItems(m_Skill.Id).FirstOrDefault();
            if(current != null){
                if (!current.CheckSkill()) {
                    if (m_FailureNotification != null && !string.IsNullOrEmpty(m_FailureNotification.text))
                        m_FailureNotification.Show();
                    return ActionStatus.Failure;
                }
            }
            if (m_SuccessNotification != null && !string.IsNullOrEmpty(m_SuccessNotification.text))
                m_SuccessNotification.Show();
            return ActionStatus.Success;
        }
    }
}
