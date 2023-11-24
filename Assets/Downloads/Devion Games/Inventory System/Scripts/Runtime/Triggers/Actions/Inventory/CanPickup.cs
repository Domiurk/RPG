using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [Icon("Condition Item")]
    [ComponentMenu("Inventory System/Can Pickup")]
    public class CanPickup : Action, ICondition
    {
        [SerializeField]
        private readonly string m_WindowName = "Inventory";

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            m_ItemCollection = gameObject.GetComponent<ItemCollection>();
        }

        public override ActionStatus OnUpdate()
        {
            bool result = ItemContainer.CanAddItems(m_WindowName, m_ItemCollection.ToArray());
            if (!result) {
                InventoryManager.Notifications.containerFull.Show(m_WindowName);
            }
            return result? ActionStatus.Success: ActionStatus.Failure;
        }
    }

}