using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Lock")]
    public class Lock : Action
    {
        [Tooltip("The name of the window to lock.")]
        [SerializeField]
        private readonly string m_WindowName = "Loot";
        [SerializeField]
        private readonly bool m_State = true;
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

            m_ItemContainer.Lock(m_State);
           
            return ActionStatus.Success;
        }
    }
}
