using UnityEngine;
using System.Collections.Generic;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class EquipmentItem : UsableItem
    {
        [SerializeField]
        protected GameObject m_OverrideEquipPrefab;
        public GameObject EquipPrefab
        {
            get{
                if(m_OverrideEquipPrefab != null)
                    return m_OverrideEquipPrefab;
                return Prefab;
            }
        }

        [EquipmentPicker(true)]
        [SerializeField]
        private List<EquipmentRegion> m_Region = new();
        public List<EquipmentRegion> Region
        {
            get => m_Region;
            set => m_Region = value;
        }
    }
}