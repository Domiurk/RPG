﻿using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class ItemGroup : ScriptableObject, INameable
    {
        [SerializeField]
        private string m_GroupName="New Group";
        public string Name
        {
            get => this.m_GroupName;
            set => this.m_GroupName = value;
        }

        [ItemPicker(true)]
        [SerializeField]
        private Item[] m_Items=new Item[0];
        public Item[] Items => this.m_Items;

        [SerializeField]
        protected int[] m_Amounts = new int[0];
        public int[] Amounts => this.m_Amounts;

        [SerializeField]
        protected List<ItemModifierList> m_Modifiers = new List<ItemModifierList>();

        public List<ItemModifierList> Modifiers => this.m_Modifiers;
    }
}