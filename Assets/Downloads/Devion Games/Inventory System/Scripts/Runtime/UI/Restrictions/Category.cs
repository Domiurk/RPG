using UnityEngine;

namespace DevionGames.InventorySystem.Restrictions
{
    public class Category : Restriction
    {
        [CategoryPicker(true)]
        [SerializeField]
        private DevionGames.InventorySystem.Category[] m_Categories;
        [SerializeField]
        private bool invert;
        public override bool CanAddItem(Item item)
        {
            for (int i = 0; i < m_Categories.Length; i++)
            {
                if (m_Categories[i].IsAssignable(item.Category))
                {
                    return !invert;
                }
            }

            return invert;
        }
    }
}