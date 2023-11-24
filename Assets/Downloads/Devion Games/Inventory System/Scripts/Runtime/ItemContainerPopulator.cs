using DevionGames.UIWidgets;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class ItemContainerPopulator : MonoBehaviour
    {
        [SerializeField]
        protected List<Entry> m_Entries = new();


        protected virtual void Start() {
            if (!InventoryManager.HasSavedData()) {
                for (int i = 0; i < m_Entries.Count; i++) {
                    ItemContainer container = WidgetUtility.Find<ItemContainer>(m_Entries[i].name);
                    if (container != null) {
                        Item[] groupItems = InventoryManager.CreateInstances(m_Entries[i].group);
                        for (int j = 0; j < groupItems.Length; j++) {
                            container.StackOrAdd(groupItems[j]);
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class Entry {
            public string name = "Inventory";
            [ItemGroupPicker]
            public ItemGroup group;
        }
    }
}