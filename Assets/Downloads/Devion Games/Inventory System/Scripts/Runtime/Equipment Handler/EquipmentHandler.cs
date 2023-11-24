using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class EquipmentHandler : MonoBehaviour
    {
        [SerializeField]
        private string m_WindowName = "Equipment";

        [SerializeField]
        private ItemDatabase m_Database;
        [SerializeField]
        private List<EquipmentBone> m_Bones= new();
        public List<EquipmentBone> Bones
        {
            get => m_Bones;
            set => m_Bones = value;
        }

        [SerializeField]
        private List<VisibleItem> m_VisibleItems= new();
        public List<VisibleItem> VisibleItems {
            get => m_VisibleItems;
            set => m_VisibleItems = value;
        }


        private ItemContainer m_EquipmentContainer;

        private void Start()
        {
            m_EquipmentContainer = WidgetUtility.Find<ItemContainer>(m_WindowName);
            if (m_EquipmentContainer != null)
            {
                for (int i = 0; i < m_VisibleItems.Count; i++)
                {
                    m_VisibleItems[i].enabled = false;
                }
                m_EquipmentContainer.OnAddItem += OnAddItem;
                m_EquipmentContainer.OnRemoveItem += OnRemoveItem;
                UpdateEquipment();
                if (InventoryManager.current != null) {
                    InventoryManager.current.onDataLoaded.AddListener(UpdateEquipment);
                }
            }
        }

        private void OnAddItem(Item item, Slot slot)
        {
            Debug.Log("OnEquip");
            if (item != null && item is EquipmentItem)
            {
                EquipItem(item as EquipmentItem);
            }
        }

        private void OnRemoveItem(Item item, int amount, Slot slot)
        {
            if (item != null && item is EquipmentItem)
            {
                UnEquipItem(item as EquipmentItem);
            }
        }

        public void EquipItem(EquipmentItem item)
        {
            foreach (ObjectProperty property in item.GetProperties())
            {
                if (property.SerializedType == typeof(int) || property.SerializedType == typeof(float))
                {
                    float value = System.Convert.ToSingle(property.GetValue());
                    SendMessage("AddModifier", new object[] { property.Name, value, (value <= 1f && value >= -1f) ? 1 : 0, item }, SendMessageOptions.DontRequireReceiver);
                }
            }

            for (int i = 0; i < m_VisibleItems.Count; i++) {
                VisibleItem visibleItem = m_VisibleItems[i];
                if (visibleItem.item.Id == item.Id) {
                    visibleItem.OnItemEquip(item);
                    return;
                }
            }

            StaticItem staticItem = gameObject.AddComponent<StaticItem>();
            staticItem.item = InventoryManager.Database.items.Find(x=>x.Id== item.Id);
            VisibleItem.Attachment attachment = new VisibleItem.Attachment();
            attachment.prefab = item.EquipPrefab;
            attachment.region = item.Region[0];
            staticItem.attachments = new VisibleItem.Attachment[1] { attachment};
            staticItem.OnItemEquip(item);
        }

        public void UnEquipItem(EquipmentItem item)
        {
            foreach (ObjectProperty property in item.GetProperties())
            {
                if (property.SerializedType == typeof(int) || property.SerializedType == typeof(float))
                {
                    SendMessage("RemoveModifiersFromSource", new object[] { property.Name, item }, SendMessageOptions.DontRequireReceiver);
                }
            }
            for (int i = 0; i < m_VisibleItems.Count; i++)
            {
                VisibleItem visibleItem = m_VisibleItems[i];
                if (visibleItem.item.Id == item.Id)
                {
                    visibleItem.OnItemUnEquip(item);
                    break;
                }
            }
        }

        private void UpdateEquipment()
        {
            EquipmentItem[] containerItems = m_EquipmentContainer.GetItems<EquipmentItem>();
            foreach (EquipmentItem item in containerItems)
            {
                EquipItem(item);
            }

        }

        public Transform GetBone(EquipmentRegion region) {
            EquipmentBone bone = Bones.Find(x => x.region == region);
            if (bone == null || bone.bone == null) {
                Debug.LogWarning("Missing Bone Map configuration: "+gameObject.name);
                return null;
            }
            return bone.bone.transform;
        }

        [System.Serializable]
        public class EquipmentBone{
            [EquipmentPicker(true)]
            public EquipmentRegion region;
            public GameObject bone;
        }
      
    }
}