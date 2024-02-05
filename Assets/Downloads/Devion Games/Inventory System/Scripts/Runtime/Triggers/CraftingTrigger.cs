using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class CraftingTrigger : Trigger
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks){
                    "OnCraftStart",
                    "OnFailedCraftStart",
                    "OnCraftItem",
                    "OnFailedToCraftItem",
                    "OnCraftStop"
                };
                return callbacks.ToArray();
            }
        }

        [Header("Crafting")]
        [SerializeField]
        protected string m_RequiredIngredientsWindow = "Inventory";
        [SerializeField]
        protected string m_ResultStorageWindow = "Inventory";
        [SerializeField]
        protected string m_CraftingProgressbar = "CraftingProgress";

        protected static void Execute(ITriggerFailedCraftStart handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedCraftStart(item, player, failureCause);
        }

        protected static void Execute(ITriggerCraftStart handler, Item item, GameObject player)
        {
            handler.OnCraftStart(item,player);
        }

        protected static void Execute(ITriggerCraftItem handler, Item item, GameObject player)
        {
            handler.OnCraftItem(item, player);
        }

        protected static void Execute(ITriggerFailedToCraftItem handler, Item item, GameObject player, FailureCause failureCause)
        {
            handler.OnFailedToCraftItem(item, player, failureCause);
        }

        protected static void Execute(ITriggerCraftStop handler, Item item, GameObject player)
        {
            handler.OnCraftStop(item, player);
        }

        private ItemContainer m_ResultStorageContainer;
        private ItemContainer m_RequiredIngredientsContainer;
        private bool m_IsCrafting;
        private float m_ProgressDuration;
        private float m_ProgressInitTime;
        private Progressbar m_Progressbar;
        private Spinner m_AmountSpinner;

        private Coroutine coroutine;

        protected override void Start()
        {
            base.Start();
            m_ResultStorageContainer = WidgetUtility.Find<ItemContainer>(m_ResultStorageWindow);
            m_RequiredIngredientsContainer = WidgetUtility.Find<ItemContainer>(m_RequiredIngredientsWindow);
            m_Progressbar = WidgetUtility.Find<Progressbar>(m_CraftingProgressbar);
        }


        public override bool OverrideUse(Slot slot, Item item)
        {

            if (currentUsedWindow == item.Container && !slot.MoveItem())
            {
                m_AmountSpinner = currentUsedWindow.GetComponentInChildren<Spinner>();
                if (m_AmountSpinner != null)
                {
                    m_AmountSpinner.min = 1;
                    m_AmountSpinner.max = int.MaxValue;
                    StartCrafting(item, (int)m_AmountSpinner.current);
                }else {
                    StartCrafting(item, 1);
                }
            }
            return true;
        }

        protected override void Update()
        {
            base.Update();
            if (m_IsCrafting)
            {
                m_Progressbar.SetProgress(GetCraftingProgress());
            }
            
        }

        protected override void OnTriggerInterrupted()
        {
            StopAllCoroutines();
            m_IsCrafting = false;
            m_Progressbar.SetProgress(0f);
            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
                user.SendMessage("SetControllerActive", true, SendMessageOptions.DontRequireReceiver);

            LoadCachedAnimatorStates();
    
        }

        private float GetCraftingProgress()
        {
            if (Time.time - m_ProgressInitTime < m_ProgressDuration)
            {
                return Mathf.Clamp01(((Time.time - m_ProgressInitTime) / m_ProgressDuration));
            }
            return 1f;
        }

        public void StartCrafting(Item item, int amount)
        {
            if (item == null) {
                InventoryManager.Notifications.selectItem.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.FurtherAction);
                return;
            }
            if (m_IsCrafting) {
                InventoryManager.Notifications.alreadyCrafting.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.InUse);
                return;
            }

            if (item.UseCraftingSkill)
            {
                ItemContainer skills = WidgetUtility.Find<ItemContainer>(item.SkillWindow);
                if (skills != null)
                {
                    Skill skill = (Skill)skills.GetItems(item.CraftingSkill.Id).FirstOrDefault();
                    if (skill == null)
                    {
                        InventoryManager.Notifications.missingSkillToCraft.Show(item.DisplayName);
                        return;
                    }

                    if (skill.CurrentValue < item.MinCraftingSkillValue)
                    {
                        InventoryManager.Notifications.requiresHigherSkill.Show(item.DisplayName, skill.DisplayName);
                        return;
                    }
                }
                else {
                    Debug.LogWarning("Item is set to use a skill but no skill window with name "+item.SkillWindow+" found!");
                }
            }

            if (!HasIngredients(m_RequiredIngredientsContainer, item))
            {
                InventoryManager.Notifications.missingIngredient.Show();
                ExecuteEvent<ITriggerFailedCraftStart>(Execute, item, FailureCause.Requirement);
                return;
            }

            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if (user != null)
            {
                user.SendMessage("SetControllerActive", false, SendMessageOptions.DontRequireReceiver);

                Animator animator = InventoryManager.current.PlayerInfo.animator;
                if(animator != null)
                    animator.CrossFadeInFixedTime(Animator.StringToHash(item.CraftingAnimatorState), 0.2f);

            }

            coroutine= StartCoroutine(CraftItems(item, amount));
            ExecuteEvent<ITriggerCraftStart>(Execute, item);

        }

        private bool HasIngredients(ItemContainer container, Item item) {
            for (int i = 0; i < item.ingredients.Count; i++)
            {
                if (!container.HasItem(item.ingredients[i].item, item.ingredients[i].amount))
                {
                    return false;
                }
            }
            return true;
        }

        public void StopCrafting(Item item)
        {
            m_IsCrafting = false;
            GameObject user = InventoryManager.current.PlayerInfo.gameObject;
            if(user != null)
                user.SendMessage("SetControllerActive", true, SendMessageOptions.DontRequireReceiver);

            LoadCachedAnimatorStates();
            StopCoroutine("CraftItems");
            ExecuteEvent<ITriggerCraftStop>(Execute, item);
            m_Progressbar.SetProgress(0f);
        }

        private IEnumerator CraftItems(Item item, int amount)
        {
            m_IsCrafting = true;
            for (int i = 0; i < amount; i++)
            {
                if (HasIngredients(m_RequiredIngredientsContainer,item))
                {

                    yield return StartCoroutine(CraftItem(item));
                }
                else
                {
                    InventoryManager.Notifications.missingIngredient.Show();
                    ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.Requirement);
                    break;
                }
            }
            StopCrafting(item);
            m_IsCrafting = false;
        }

        private IEnumerator CraftItem(Item item)
        {
            m_ProgressDuration = item.CraftingDuration;
            m_ProgressInitTime = Time.time;
            yield return new WaitForSeconds(item.CraftingDuration);
            if (item.UseCraftingSkill) {
                ItemContainer skills = WidgetUtility.Find<ItemContainer>(item.SkillWindow);
                Skill skill = (Skill)skills.GetItems(item.CraftingSkill.Id).FirstOrDefault();
                if (skill == null) { Debug.LogWarning("Skill not found in " + item.SkillWindow + "."); }
                if (!skill.CheckSkill()) {
                    InventoryManager.Notifications.failedToCraft.Show(item.DisplayName);
                    if (item.RemoveIngredientsWhenFailed) {
                        for (int i = 0; i < item.ingredients.Count; i++)
                        {
                            m_RequiredIngredientsContainer.RemoveItem(item.ingredients[i].item, item.ingredients[i].amount);
                        }
                    }
                    yield break;
                }  

            }

            Item craftedItem = Instantiate(item);
            craftedItem.Stack = 1;
            craftedItem.CraftingModifier.Modify(craftedItem);



            if (m_ResultStorageContainer.StackOrAdd(craftedItem))
            {
                for (int i = 0; i < item.ingredients.Count; i++)
                {
                    m_RequiredIngredientsContainer.RemoveItem(item.ingredients[i].item, item.ingredients[i].amount);
                }
                InventoryManager.Notifications.craftedItem.Show(UnityTools.ColorString(craftedItem.Name, craftedItem.Rarity.Color));
                ExecuteEvent<ITriggerCraftItem>(Execute, craftedItem);
            }
            else
            {
                InventoryManager.Notifications.containerFull.Show(m_ResultStorageContainer.Name);
                ExecuteEvent<ITriggerFailedToCraftItem>(Execute, item, FailureCause.ContainerFull);
                StopCrafting(item);
            }
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            m_CallbackHandlers.Add(typeof(ITriggerFailedCraftStart), "OnFailedCraftStart");
            m_CallbackHandlers.Add(typeof(ITriggerCraftStart), "OnCraftStart");
            m_CallbackHandlers.Add(typeof(ITriggerCraftItem), "OnCraftItem");
            m_CallbackHandlers.Add(typeof(ITriggerFailedToCraftItem),"OnFailedToCraftItem");
            m_CallbackHandlers.Add(typeof(ITriggerCraftStop), "OnCraftStop");
        }
    }
}