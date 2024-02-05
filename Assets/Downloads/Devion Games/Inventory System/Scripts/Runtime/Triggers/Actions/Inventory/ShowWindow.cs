using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Show Window")]
    public class ShowWindow : Action, ITriggerUnUsedHandler
    {
        [Tooltip("The name of the window to show.")]
        [SerializeField]
        private readonly string m_WindowName = "Loot";
        [SerializeField]
        private readonly bool m_DestroyWhenEmpty = false;


        private ItemContainer m_ItemContainer;
        private ItemCollection m_ItemCollection;
        private ActionStatus m_WindowStatus= ActionStatus.Inactive;

        public override void OnSequenceStart()
        {
            m_WindowStatus = ActionStatus.Inactive;
            m_ItemContainer = WidgetUtility.Find<ItemContainer>(m_WindowName);
            if (m_ItemContainer != null) {
                m_ItemContainer.RegisterListener("OnClose",(CallbackEventData _)=>{ m_WindowStatus = ActionStatus.Success;  });
            }
            m_ItemCollection = gameObject.GetComponent<ItemCollection>();
            if (m_ItemCollection != null)
            {
                m_ItemCollection.onChange.AddListener(delegate ()
                {
                    if (m_ItemCollection.IsEmpty && m_DestroyWhenEmpty)
                    {
                        InventoryManager.Destroy(gameObject);
                    }
                });
            }
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (m_ItemContainer != null) {
                m_ItemContainer.Close();
                Trigger.currentUsedWindow = null;
            }
        }

        public override ActionStatus OnUpdate()
        {
            if (m_ItemContainer == null)
            {
                Debug.LogWarning("Missing window " + m_WindowName + " in scene!");
                return ActionStatus.Failure;
            }

            if (m_WindowStatus == ActionStatus.Inactive)
            {
                Trigger.currentUsedWindow = m_ItemContainer;
                if (m_ItemCollection == null) {
                    m_ItemContainer.Show();
                }else{
                    m_ItemContainer.Collection = m_ItemCollection;
                    m_ItemContainer.Show();
                
                }
                m_WindowStatus = ActionStatus.Running;
            }
            return m_WindowStatus;
        }

       
    }
}
