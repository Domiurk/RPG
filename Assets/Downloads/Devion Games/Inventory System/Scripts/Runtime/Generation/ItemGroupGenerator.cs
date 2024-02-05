using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemGroupGenerator : MonoBehaviour, IGenerator
    {
        [ItemGroupPicker]
        [SerializeField]
        private ItemGroup m_From;
        [SerializeField]
        private List<ScriptableObject> m_Filters = new();
        [SerializeField]
        private int m_MinStack = 1;
        [SerializeField]
        private int m_MaxStack = 1;
        [SerializeField]
        private int m_MinAmount = 1;
        [SerializeField]
        private int m_MaxAmount = 1;

        [Range(0f,1f)]
        [SerializeField]
        private float m_Chance = 1.0f;

        [SerializeField]
        private ItemModifierList m_Modifiers = new();


        private void Start()
        {
            List<Item> items = GenerateItems();
            ItemCollection collection = GetComponent<ItemCollection>();
            collection.Add(items.ToArray());
        }


        public List<Item> GenerateItems() {
            List<Item> items = InventoryManager.Database.items;
            if (m_From != null) {
                items = new List<Item>(m_From.Items);
            }


            for (int i = 0; i < m_Filters.Count; i++) {
                if (m_Filters[i] is Category) {
                    items = items.Where(x => x.Category != null &&( x.Category.Name == (m_Filters[i] as Category).Name)).ToList();
                }
                if (m_Filters[i] is Rarity)
                {
                    items = items.Where(x => x.Rarity.Name == (m_Filters[i] as Rarity).Name).ToList();
                }
            }
            

            List<Item> generatedItems = new List<Item>();
            if (items.Count < 1) { return generatedItems; }

            int amount = Random.Range(m_MinAmount, m_MaxAmount+1);

            for (int i = 0; i < amount; i++) {
                if (Random.value > m_Chance) {
                    continue;
                }

                Item item = items[Random.Range(0, items.Count)];
                int stack = Random.Range(m_MinStack,m_MaxStack);
                item = Instantiate(item);
                item.Stack = stack;

                m_Modifiers.Modify(item);

                if (item.IsCraftable)
                {
                    for (int j = 0; j < item.ingredients.Count; j++)
                    {
                        item.ingredients[j].item = Instantiate(item.ingredients[j].item);
                        item.ingredients[j].item.Stack = item.ingredients[j].amount;
                    }
                }
                generatedItems.Add(item);


            }
            return generatedItems;
        }
    }
}