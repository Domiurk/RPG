using DevionGames.UIWidgets;
using UnityEngine;


namespace DevionGames.InventorySystem
{
    public class DisplayTriggerTooltip : MonoBehaviour, ITriggerWentOutOfRange, ITriggerUsedHandler
    {
        [SerializeField]
        protected string m_Title;
        [SerializeField]
        protected string m_Instruction = "Pickup";

        protected TriggerTooltip m_Tooltip;
        protected BaseTrigger m_Trigger;


        private void Start()
        {
            m_Trigger = GetComponentInChildren<BaseTrigger>(true);
            m_Tooltip = WidgetUtility.Find<TriggerTooltip>("Trigger Tooltip");
        }

        private void Update()
        {

            if (!m_Trigger.InUse && m_Trigger.InRange && m_Trigger.IsBestTrigger())
            {
                DoDisplayTooltip(true);
            }
        }

        protected virtual void DoDisplayTooltip(bool state)
        {
            if (m_Tooltip == null) return;

            if (state)
            {
                m_Tooltip.Show(m_Title, m_Instruction);
            }
            else
            {
                m_Tooltip.Close();
            }
        }
   
        private void OnDestroy()
        {
            DoDisplayTooltip(false);
        }

        public void OnWentOutOfRange(GameObject player)
        {
            DoDisplayTooltip(false);
        }

        public void OnTriggerUsed(GameObject player)
        {
            DoDisplayTooltip(false);
        }
    }
}