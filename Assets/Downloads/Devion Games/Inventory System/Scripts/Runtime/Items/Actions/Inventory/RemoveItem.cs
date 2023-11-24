using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Remove Item")]
    [System.Serializable]
    public class RemoveItem : ItemAction
    {
        [SerializeField]
        private string m_WindowName = "Inventory";
        [ItemPicker(true)]
        [SerializeField]
        private Item m_Item;
        [Range(1,200)]
        [SerializeField]
        private int m_Amount = 1;

        public override ActionStatus OnUpdate()
        {
            if (ItemContainer.RemoveItem(m_WindowName, m_Item,m_Amount)) {
                return ActionStatus.Success;
            }
            return ActionStatus.Failure;

        }
    }
}