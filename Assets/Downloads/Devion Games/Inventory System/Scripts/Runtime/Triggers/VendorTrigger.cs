using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class VendorTrigger : Trigger, ITriggerUnUsedHandler
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks){
                    "OnSelectSellItem",
                    "OnSoldItem",
                    "OnFaildToSellItem",
                    "OnSelectBuyItem",
                    "OnBoughtItem",
                    "OnFaildToBuyItem"
                };
                return callbacks.ToArray();
            }
        }

        [Header("Vendor")]
        [Range(0f,10f)]
        [SerializeField]
        protected float m_BuyPriceFactor = 1.0f;
        [Range(0f, 10f)]
        [SerializeField]
        protected float m_SellPriceFactor = 1.0f;
        [SerializeField]
        protected string m_PurchasedStorageWindow = "Inventory";
        [SerializeField]
        protected string m_PaymentWindow = "Inventory";
        [SerializeField]
        protected bool m_RemoveItemAfterPurchase;

        [Header("Buy & Sell Dialog")]
        [SerializeField]
        private string m_BuySellDialogName = "BuySellDialog";
        [SerializeField]
        private bool m_DisplaySpinner = true;

        [Header("Buy")]
        [SerializeField]
        private string m_BuyDialogTitle = "Buy";
        [SerializeField]
        private string m_BuyDialogText= "How many items do you want to buy?";
        [SerializeField]
        private string m_BuyDialogButton = "Buy";

        [Header("Sell")]
        [SerializeField]
        private string m_SellDialogTitle = "Sell";
        [SerializeField]
        private string m_SellSingleDialogText = "Are you sure you want to sell this item?";
        [SerializeField]
        private string m_SellMultipleDialogText = "How many items do you want to sell?";
        [SerializeField]
        private string m_SellDialogButton = "Sell";

        private DialogBox m_BuySellDialog;
        private Spinner m_AmountSpinner;
        private ItemContainer m_PriceInfo;

        private ItemContainer m_PurchasedStorageContainer;
        private ItemContainer m_PaymentContainer;

        protected static void Execute(ITriggerSelectSellItem handler, Item item, GameObject player)
        {
            handler.OnSelectSellItem(item, player);
        }

        protected static void Execute(ITriggerSoldItem handler, Item item, GameObject player)
        {
            handler.OnSoldItem(item, player);
        }

        protected static void Execute(ITriggerFailedToSellItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToSellItem(item, player, failureCause);
        }

        protected static void Execute(ITriggerSelectBuyItem handler, Item item, GameObject player)
        {
            handler.OnSelectBuyItem(item, player);
        }

        protected static void Execute(ITriggerBoughtItem handler, Item item, GameObject player)
        {
            handler.OnBoughtItem(item, player);
        }

        protected static void Execute(ITriggerFailedToBuyItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToBuyItem(item, player, failureCause);
        }

        protected override void Start()
        {
            base.Start();
            m_BuySellDialog = WidgetUtility.Find<DialogBox>(m_BuySellDialogName);
            if (m_BuySellDialog != null) {
               m_AmountSpinner = m_BuySellDialog.GetComponentInChildren<Spinner>();
               m_PriceInfo = m_BuySellDialog.GetComponentInChildren<ItemContainer>();
            }
            m_PurchasedStorageContainer= WidgetUtility.Find<ItemContainer>(m_PurchasedStorageWindow);
            m_PaymentContainer = WidgetUtility.Find<ItemContainer>(m_PaymentWindow);
        }

        public override bool OverrideUse(Slot slot, Item item)
        {
            
            if (slot.Container.CanSellItems)
            {
                if (!item.IsSellable)
                {
                    InventoryManager.Notifications.cantSellItem.Show(item.DisplayName);
                    return true;
                }
                SellItem(item, item.Stack, true);
            }
            else if(currentUsedWindow == slot.Container)
            {
              
                BuyItem(item, 1);
            }
            return true;
        }

        public void BuyItem(Item item,int amount, bool showDialog = true) {
           
            if (showDialog)
            {
                m_AmountSpinner.gameObject.SetActive(m_DisplaySpinner);

                m_AmountSpinner.onChange.RemoveAllListeners();
                m_AmountSpinner.current = 1;
                m_AmountSpinner.min = 1;
                ObjectProperty property = item.FindProperty("BuyBack");
                m_AmountSpinner.max = (m_RemoveItemAfterPurchase || property != null && property.boolValue)?item.Stack:int.MaxValue;
                m_AmountSpinner.onChange.AddListener(delegate (float value)
                {
                    Currency price = Instantiate(item.BuyCurrency);
                    price.Stack = Mathf.RoundToInt(m_BuyPriceFactor * item.BuyPrice * value);
                    m_PriceInfo.RemoveItems();
                    m_PriceInfo.StackOrAdd(price);
                });
                m_AmountSpinner.onChange.Invoke(m_AmountSpinner.current);

                ExecuteEvent<ITriggerSelectBuyItem>(Execute, item);
                m_BuySellDialog.Show(m_BuyDialogTitle, m_BuyDialogText, item.Icon, delegate (int result)
                {
                    if (result == 0){
                        BuyItem(item, Mathf.RoundToInt(m_AmountSpinner.current), false);
                    }
                }, m_BuyDialogButton, "Cancel");
            }else {
                if (m_PurchasedStorageContainer == null || m_PaymentContainer == null){
                    return;
                }
                Rarity rarity = item.Rarity;
                Item instance = Instantiate(item);

                instance.Rarity = rarity;
                instance.Stack = amount;
                Currency price = Instantiate(instance.BuyCurrency);
                price.Stack = Mathf.RoundToInt(m_BuyPriceFactor*instance.BuyPrice * amount);

                if ( m_PaymentContainer.RemoveItem(price,price.Stack))
                {

                    if (amount > instance.MaxStack)
                    {
                        int stack = instance.Stack;
                        Currency singlePrice = Instantiate(instance.BuyCurrency);
                        singlePrice.Stack = Mathf.RoundToInt(instance.BuyPrice*m_BuyPriceFactor);
                        int purchasedStack = 0;
                        for (int i = 0; i < stack; i++)
                        {
                            Item singleItem = Instantiate(instance);
                            singleItem.Rarity = instance.Rarity;
                            singleItem.Stack = 1;
                            if (!m_PurchasedStorageContainer.StackOrAdd(singleItem))
                            {
                                m_PaymentContainer.StackOrAdd(singlePrice);
                                InventoryManager.Notifications.containerFull.Show(m_PurchasedStorageWindow);
                                ExecuteEvent<ITriggerFailedToBuyItem>(Execute, instance, FailureCause.ContainerFull);
                                break;
                            }
                            purchasedStack += 1;
                            ExecuteEvent<ITriggerBoughtItem>(Execute, singleItem);
                        }
                        if (m_RemoveItemAfterPurchase)
                        {
                            item.Container.RemoveItem(item, purchasedStack);
                        }
                        InventoryManager.Notifications.boughtItem.Show(purchasedStack.ToString()+"x"+instance.DisplayName, singlePrice.Stack*purchasedStack + " " + price.Name);
                    }
                    else
                    {
                        Item itemInstance = Instantiate(instance);
                        itemInstance.Rarity = instance.Rarity;

                        if (!m_PurchasedStorageContainer.StackOrAdd(itemInstance))
                        {
                            m_PaymentContainer.StackOrAdd(price);
                            InventoryManager.Notifications.containerFull.Show(m_PurchasedStorageWindow);
                            ExecuteEvent<ITriggerFailedToBuyItem>(Execute, instance, FailureCause.ContainerFull);
                        }else {
                            ObjectProperty property = item.FindProperty("BuyBack");
    
                            if (m_RemoveItemAfterPurchase || property != null && property.boolValue)
                            {
                                item.RemoveProperty("BuyBack");
                                item.Container.RemoveItem(item, amount);
                            }
                            InventoryManager.Notifications.boughtItem.Show(itemInstance.Name, price.Stack+" "+price.Name);
                            ExecuteEvent<ITriggerBoughtItem>(Execute, itemInstance);
                        }
                    }
                }
                else
                {
                    InventoryManager.Notifications.noCurrencyToBuy.Show(item.DisplayName, price.Stack + " " + price.Name);
                    ExecuteEvent<ITriggerFailedToBuyItem>(Execute, item, FailureCause.NotEnoughCurrency);
                }
            }
        }

        public void SellItem(Item item, int amount, bool showDialog = true)
        {
            if (showDialog)
            {
                m_AmountSpinner.gameObject.SetActive(m_DisplaySpinner);
                 if (item.Stack > 1)
                    {

                        m_AmountSpinner.onChange.RemoveAllListeners();
                        m_AmountSpinner.current = amount;
                        m_AmountSpinner.min = 1;
                        m_AmountSpinner.max = item.Stack;
                        m_AmountSpinner.onChange.AddListener(delegate (float value)
                        {
                            Currency price = Instantiate(item.SellCurrency);
                            price.Stack = Mathf.RoundToInt(m_SellPriceFactor * item.SellPrice * value);
                            m_PriceInfo.RemoveItems();
                            m_PriceInfo.StackOrAdd(price);
                        });
                        m_AmountSpinner.onChange.Invoke(m_AmountSpinner.current);
                    }else {
                        m_AmountSpinner.current = 1;
                        m_AmountSpinner.gameObject.SetActive(false);
                        Currency price = Instantiate(item.SellCurrency);
                        price.Stack = Mathf.RoundToInt(m_SellPriceFactor * item.SellPrice);
                        m_PriceInfo.RemoveItems();
                        m_PriceInfo.StackOrAdd(price);
                    }

                ExecuteEvent<ITriggerSelectSellItem>(Execute, item);
                m_BuySellDialog.Show(m_SellDialogTitle, item.Stack>1?m_SellMultipleDialogText:m_SellSingleDialogText, item.Icon, delegate (int result)
                {
                    if (result == 0)
                    {
                        SellItem(item, Mathf.RoundToInt(m_AmountSpinner.current), false);
                    }
                   
                }, m_SellDialogButton, "Cancel");
            }
            else
            {
                Currency price = Instantiate(item.SellCurrency);
                price.Stack = Mathf.RoundToInt(m_SellPriceFactor * item.SellPrice * amount);
                
                if (item.Container.RemoveItem(item, amount))
                {
                    ExecuteEvent<ITriggerSoldItem>(Execute, item);
                    m_PaymentContainer.StackOrAdd(price);
                    if (item.CanBuyBack)
                    {
                        item.AddProperty("BuyBack", true);
                        currentUsedWindow.AddItem(item);
                    }
                    InventoryManager.Notifications.soldItem.Show((amount>1?amount.ToString()+"x":"")+item.Name, price.Stack+" "+price.Name);
                }
                else {
                    ExecuteEvent<ITriggerFailedToSellItem>(Execute, item, FailureCause.Remove);
                }
            }
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (currentUsedTrigger == this && m_BuySellDialog.IsVisible) {
                m_BuySellDialog.Close();
            }
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            m_CallbackHandlers.Add(typeof(ITriggerSelectSellItem), "OnSelectSellItem");
            m_CallbackHandlers.Add(typeof(ITriggerSoldItem), "OnSoldItem");
            m_CallbackHandlers.Add(typeof(ITriggerFailedToSellItem), "OnFailedToSellItem");
            m_CallbackHandlers.Add(typeof(ITriggerSelectBuyItem), "OnSelectBuyItem");
            m_CallbackHandlers.Add(typeof(ITriggerBoughtItem), "OnBoughtItem");
            m_CallbackHandlers.Add(typeof(ITriggerFailedToBuyItem), "OnFailedToBuyItem");
        }
    }
}