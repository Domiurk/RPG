using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Item")]
    [ComponentMenu("Inventory System/Pickup Item")]
    [RequireComponent(typeof(ItemCollection))]
    public class Pickup : Action
    {
        [SerializeField]
        private readonly string m_WindowName = "Inventory";
        [SerializeField]
        private readonly bool m_DestroyWhenEmpty = true;
        [SerializeField]
        private readonly int m_Amount = -1;

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            m_ItemCollection = gameObject.GetComponent<ItemCollection>();
            m_ItemCollection.onChange.AddListener(delegate () {
                if (m_ItemCollection.IsEmpty && m_DestroyWhenEmpty)
                {
                    Object.Destroy(gameObject,0.1f);
                }
            });

        }

        public override ActionStatus OnUpdate()
        {
            return PickupItems() ;
        }

        private ActionStatus  PickupItems()
        {
            if (m_ItemCollection.Count == 0) {
                InventoryManager.Notifications.empty.Show(gameObject.name.Replace("(Clone)", "").ToLower());
                return ActionStatus.Failure;
            }
            ItemContainer[] windows = WidgetUtility.FindAll<ItemContainer>(m_WindowName);
            List<Item> items = new List<Item>();
            if (m_Amount < 0)
            {
                items.AddRange(m_ItemCollection);
            }
            else
            {
                for (int i = 0; i < m_Amount; i++)
                {
                    Item item = m_ItemCollection[Random.Range(0, m_ItemCollection.Count)];
                    items.Add(item);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (windows.Length > 0)
                {
                    for (int j = 0; j < windows.Length; j++)
                    {
                        ItemContainer current = windows[j];

                        if (current.StackOrAdd(item))
                        {
                            m_ItemCollection.Remove(item);
                            break;
                        }
                    }
                }
                else
                {
                    DropItem(item);
                    m_ItemCollection.Remove(item);
                }
            }

            return ActionStatus.Success;
        }

        private void DropItem(Item item)
        {
            GameObject prefab = item.OverridePrefab != null ? item.OverridePrefab : item.Prefab;
            float angle = Random.Range(0f, 360f);
            float x = (float)(InventoryManager.DefaultSettings.maxDropDistance * Mathf.Cos(angle * Mathf.PI / 180f)) + gameObject.transform.position.x;
            float z = (float)(InventoryManager.DefaultSettings.maxDropDistance * Mathf.Sin(angle * Mathf.PI / 180f)) + gameObject.transform.position.z;
            Vector3 position = new Vector3(x, gameObject.transform.position.y, z);

            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit)) {
                position = hit.point+ Vector3.up;
            }

            GameObject go = InventoryManager.Instantiate(prefab, position, Random.rotation);
            ItemCollection collection = go.GetComponent<ItemCollection>();
            if (collection != null)
            {
                collection.Clear();
                collection.Add(item);
            }
        }
    }
}