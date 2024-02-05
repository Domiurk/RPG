using UnityEngine;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemCollectionPopulator : MonoBehaviour
    {
        [ItemGroupPicker]
        [SerializeField]
        public ItemGroup m_ItemGroup;

        private void Start() {}
    }
}