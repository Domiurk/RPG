using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [RequireComponent(typeof(ItemCollection))]
    public class ItemGenerator : MonoBehaviour, IGenerator
    {
        [SerializeField]
        private List<ItemGeneratorData> m_ItemGeneratorData=new();
        [SerializeField]
        private int m_MaxAmount = 1;

        private void Start()
        {
            ItemCollection collection = GetComponent<ItemCollection>();
            collection.Add(GenerateItems().ToArray());
        }

        private List<Item> GenerateItems() {
            List<Item> generatedItems = new List<Item>();
            IEnumerable<int> indices = Enumerable.Range(0, m_ItemGeneratorData.Count).OrderBy(_=> rng.Next());

            foreach (int index in indices) {
                if (generatedItems.Count >= m_MaxAmount){
                    break;
                }
                ItemGeneratorData data = m_ItemGeneratorData[index];
                if (Random.value > data.chance){
                    continue;
                }
                Item item = data.item;
                int stack = Random.Range(data.minStack, data.maxStack + 1);
                stack = Mathf.Clamp(stack, item.Stack, item.MaxStack);
                item = Instantiate(item);
                item.Stack = stack;

                data.modifiers.Modify(item);

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

        private readonly System.Random rng = new();

     
    }


}