using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Add Item")]
    [System.Serializable]
    public class AddItem : ItemAction
    {
        [SerializeField]
        private string m_WindowName = "Inventory";
        [ItemPicker(true)]
        [SerializeField]
        private Item m_Item;
        [Range(1, 200)]
        [SerializeField]
        private int m_Amount = 1;

        public override ActionStatus OnUpdate()
        {
            Item instance = Object.Instantiate(m_Item);
            instance.Stack = m_Amount;
            if (m_Item.IsCraftable)
            {
                for (int j = 0; j < m_Item.ingredients.Count; j++)
                {
                    instance.ingredients[j].item = Object.Instantiate(m_Item.ingredients[j].item);
                    instance.ingredients[j].item.Stack = m_Item.ingredients[j].amount;
                }
            }
            if (ItemContainer.AddItem(m_WindowName, instance)) {
                return ActionStatus.Success;
            }
            return ActionStatus.Failure;

        }
    }
}