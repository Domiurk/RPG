﻿using System.Collections.Generic;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class ItemModifierList
    {
        public List<ItemModifier> modifiers = new();

        public void Modify(Item item) {
            for (int i = 0; i < modifiers.Count; i++) {
                modifiers[i].Modify(item);
            }
        }
    }
}