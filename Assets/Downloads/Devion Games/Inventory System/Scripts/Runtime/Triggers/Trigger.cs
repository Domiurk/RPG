using UnityEngine;
using UnityEngine.EventSystems;


namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [DisallowMultipleComponent]
    public class Trigger : BehaviorTrigger
	{
        public enum FailureCause {
            Unknown,
            FurtherAction,
            NotEnoughCurrency,
            Remove,
            ContainerFull,
            InUse,
            Requirement
        }

        public override PlayerInfo PlayerInfo => InventoryManager.current.PlayerInfo;

        public static ItemContainer currentUsedWindow;
        protected delegate void ItemEventFunction<T>(T handler, Item item, GameObject player);
        protected delegate void FailureItemEventFunction<T>(T handler, Item item, GameObject player, FailureCause failureCause);

        public void StartUse() {
            Use();
        }


        public void StartUse(ItemContainer window)
        {
            if (window.IsVisible)
            {
                currentUsedWindow = window;
                Use();
            }
        }

        public void StopUse() {
            InUse = false;
        }

        public virtual bool OverrideUse(Slot slot, Item item) {
            return false;
        }

        protected override void DisplayInUse()
        {
            InventoryManager.Notifications.inUse.Show();
        }

        protected override void DisplayOutOfRange()
        {
            InventoryManager.Notifications.toFarAway.Show();
        }

        protected void ExecuteEvent<T>(ItemEventFunction<T> func, Item item, bool includeDisabled = false) where T : ITriggerEventHandler
        {
            for (int i = 0; i < m_TriggerEvents.Length; i++)
            {
                ITriggerEventHandler handler = m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, item, PlayerInfo.gameObject);
                }
            }

            string eventID = string.Empty;
            if (m_CallbackHandlers.TryGetValue(typeof(T), out eventID))
            {
                CallbackEventData triggerEventData = new CallbackEventData();
                triggerEventData.AddData("Trigger",this);
                triggerEventData.AddData("Player",PlayerInfo.gameObject);
                triggerEventData.AddData("EventData", new PointerEventData(EventSystem.current));
                triggerEventData.AddData("Item", item);
                base.Execute(eventID, triggerEventData);
            }
        }

        protected void ExecuteEvent<T>(FailureItemEventFunction<T> func, Item item, FailureCause failureCause , bool includeDisabled = false) where T : ITriggerEventHandler
        {
            for (int i = 0; i < m_TriggerEvents.Length; i++)
            {
                ITriggerEventHandler handler = m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, item, InventoryManager.current.PlayerInfo.gameObject, failureCause);
                }
            }

            string eventID = string.Empty;
            if (m_CallbackHandlers.TryGetValue(typeof(T), out eventID))
            {
                CallbackEventData triggerEventData = new CallbackEventData();
                triggerEventData.AddData("Trigger", this);
                triggerEventData.AddData("Player", PlayerInfo.gameObject);
                triggerEventData.AddData("EventData", new PointerEventData(EventSystem.current));
                triggerEventData.AddData("Item", item);
                triggerEventData.AddData("FailureCause", failureCause);
                base.Execute(eventID, triggerEventData);
            }
        }
	}
}