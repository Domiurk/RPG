using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Lock All")]
    public class LockAll : Action
    {
        [SerializeField]
        private readonly bool m_State = true;

        public override ActionStatus OnUpdate()
        {
            ItemContainer[] containers = Object.FindObjectsOfType<ItemContainer>();
            for (int i = 0; i < containers.Length; i++)
            {
                containers[i].Lock(m_State);
            }

            return ActionStatus.Success;
        }
    }
}
