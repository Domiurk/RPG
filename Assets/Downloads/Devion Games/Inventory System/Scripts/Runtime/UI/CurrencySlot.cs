using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class CurrencySlot : Slot
    {
        /// <summary>
        /// Currency this slot is holding
        /// </summary>
        [CurrencyPicker(true)]
        [SerializeField]
        protected Currency m_Currency;
        /// <summary>
        /// Hides the empty currency slot
        /// </summary>
        [SerializeField]
        protected bool m_HideEmptySlot;

        public Currency GetDefaultCurrency()
        {
            Currency currency = Instantiate(m_Currency);
            currency.Stack = 0;
            return currency;
        }

        public override void Repaint()
        {
            base.Repaint();

            if (m_HideEmptySlot)
            {
                
                gameObject.SetActive(!(ObservedItem == null || ObservedItem.Stack == 0));

            }

        }

        public override bool CanAddItem(Item item )
        {
            return base.CanAddItem(item) && typeof(Currency).IsAssignableFrom(item.GetType());
        }

        public override bool CanUse()
        {
            return false;
        }

    }
}