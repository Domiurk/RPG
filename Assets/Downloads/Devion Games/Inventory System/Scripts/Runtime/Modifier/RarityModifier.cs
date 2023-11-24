using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CreateAssetMenu(fileName ="SimpleRarityModifier",menuName = "Devion Games/Inventory System/Modifiers/Rarity")]
    [System.Serializable]
    public class RarityModifier : ItemModifier
    {
        [RarityPicker(true)]
        [SerializeField]
        protected List<Rarity> m_Rarities = new();

        private System.Random rnd = new();
        private static Rarity emptyRarity;

        public override void Modify(Item item)
        {
            item.Rarity = SelectRarity(m_Rarities);
            ApplyPropertyMultiplier(item, item.Rarity.Multiplier);
            
        }

        protected virtual void ApplyPropertyMultiplier(Item item, float multiplier) {
            ObjectProperty[] properties = item.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                ObjectProperty property = properties[i];
                object value = property.GetValue();
                if (UnityTools.IsNumeric(value) && property.show)
                {
                    float cur = System.Convert.ToSingle(property.GetValue()) * multiplier;
                    if (value is float)
                    {
                        property.SetValue(cur);
                    }
                    else if (value is int)
                    {
                        property.SetValue(Mathf.RoundToInt(cur));
                    }
                }
            }
        }

        protected virtual Rarity SelectRarity(List<Rarity> items)
        {
            int poolSize = 0;
            for (int i = 0; i < items.Count; i++)
            {
                poolSize += items[i].Chance;
            }
            int randomNumber = rnd.Next(0, poolSize) + 1;

            int accumulatedProbability = 0;
            for (int i = 0; i < items.Count; i++)
            {
                accumulatedProbability += items[i].Chance;
                if (randomNumber <= accumulatedProbability)
                    return items[i];
            }
            if (emptyRarity is null)
            {
                emptyRarity = CreateInstance<Rarity>();
                emptyRarity.Color = Color.grey;
                emptyRarity.Chance = 100;
                emptyRarity.Multiplier = 1.0f;
            }
            return emptyRarity;
        }
    }
}