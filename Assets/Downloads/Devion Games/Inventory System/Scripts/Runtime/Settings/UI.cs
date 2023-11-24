using DevionGames.UIWidgets;
using UnityEngine.Assertions;

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class UI : Settings
    {
        public override string Name => "UI";

        [InspectorLabel("Context Menu","Name of ContextMenu widget.")]
        public string contextMenuName = "ContextMenu";
        [InspectorLabel("Tooltip", "Name of Tooltip widget.")]
        public string tooltipName = "Tooltip";
        [InspectorLabel("Price Tooltip", "Name of sell price tooltip widget.")]
        public string sellPriceTooltipName = "Sell Price Tooltip";
        [InspectorLabel("Stack", "Name of Stack widget.")]
        public string stackName = "Stack";
        [InspectorLabel("Notification", "Name of Notification widget.")]
        public string notificationName = "Notification";

        private Notification m_Notification;
        public Notification notification {
            get {
                if (m_Notification == null) {
                    m_Notification = WidgetUtility.Find<Notification>(notificationName);
                }
                Assert.IsNotNull(m_Notification, "Notification widget with name "+notificationName+" is not present in scene.");
                return m_Notification;
            }
        }

        private Tooltip m_Tooltip;
        public Tooltip tooltip
        {
            get
            {
                if (m_Tooltip == null)
                {
                    m_Tooltip = WidgetUtility.Find<Tooltip>(tooltipName);
                }
                Assert.IsNotNull(m_Tooltip, "Tooltip widget with name " + tooltipName + " is not present in scene.");
                return m_Tooltip;
            }
        }

        private ItemContainer m_SellPriceTooltip;
        public ItemContainer sellPriceTooltip
        {
            get
            {
                if (m_SellPriceTooltip == null)
                {
                    m_SellPriceTooltip = WidgetUtility.Find<ItemContainer>(sellPriceTooltipName);
                }
                return m_SellPriceTooltip;
            }
        }

        private Stack m_Stack;
        public Stack stack
        {
            get
            {
                if (m_Stack == null)
                {
                    m_Stack = WidgetUtility.Find<Stack>(stackName);
                }
                Assert.IsNotNull(m_Stack, "Stack widget with name " + stackName + " is not present in scene.");
                return m_Stack;
            }
        }

        private ContextMenu m_ContextMenu;
        public ContextMenu contextMenu
        {
            get
            {
                if (m_ContextMenu == null)
                {
                    m_ContextMenu = WidgetUtility.Find<ContextMenu>(contextMenuName);
                }
                Assert.IsNotNull(m_ContextMenu, "ConextMenu widget with name " + contextMenuName + " is not present in scene.");
                return m_ContextMenu;
            }
        }
    }
}