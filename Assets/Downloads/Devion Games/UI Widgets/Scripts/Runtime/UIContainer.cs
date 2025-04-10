﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace DevionGames.UIWidgets
{
	public class UIContainer<T> : UIWidget where T : class
	{

        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks){
                    "OnAddItem",
                    "OnRemoveItem"
                };
                return callbacks.ToArray();
            }
        }

        /// <summary>
        /// Sets the container as dynamic. Slots are instantiated at runtime.
        /// </summary>
        [Header ("Behaviour")]
        [SerializeField]
        protected bool m_DynamicContainer;
        /// <summary>
        /// The parent transform of slots. 
        /// </summary>
        [SerializeField]
        protected Transform m_SlotParent;
        /// <summary>
        /// The slot prefab. This game object should contain the Slot component or a child class of Slot. 
        /// </summary>
        [SerializeField]
        protected GameObject m_SlotPrefab;

        protected List<UISlot<T>> m_Slots = new();
        /// <summary>
        /// Collection of slots this container is holding
        /// </summary>
        public ReadOnlyCollection<UISlot<T>> Slots => m_Slots.AsReadOnly();

        protected List<T> m_Collection;
  
        protected override void OnAwake ()
		{
			base.OnAwake ();
            m_Collection = new List<T>();
			RefreshSlots ();
		}

        /// <summary>
        /// Adds a new item to a free or dynamicly created slot in this container.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>True if item was added.</returns>
        public virtual bool AddItem(T item)
        {
            UISlot<T> slot;
            if (CanAddItem(item, out slot, true))
            {
                
                ReplaceItem(slot.Index, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at index. Sometimes an item requires more then one slot(two-handed weapon), this will remove the item with the extra slots.
        /// </summary>
        /// <param name="index">The slot index where to remove the item.</param>
        /// <returns>Returns true if the item was removed.</returns>
        public virtual bool RemoveItem(int index)
        {
            if (index < m_Slots.Count)
            {
                UISlot<T> slot = m_Slots[index];
                T item = slot.ObservedItem;

                if (item != null)
                {
                    m_Collection.Remove(item);
                    slot.ObservedItem = null;
                  
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Replaces the items at index and returns the previous item.
        /// </summary>
        /// <param name="index">Index of slot to repalce.</param>
        /// <param name="item">Item to replace with.</param>
        /// <returns></returns>
        public virtual T ReplaceItem(int index, T item)
        {
            
            if (index < m_Slots.Count)
            {
                UISlot<T> slot = m_Slots[index];
                if (!slot.CanAddItem(item)) {
                    return item;
                }
                
                if (item != null)
                {
                    m_Collection.Add(item);

                    T current = slot.ObservedItem;
                    if (current != null)
                    {
                        RemoveItem(slot.Index);
                    }
                    slot.ObservedItem = item;
                    return current;
                }
            }
            return item;
        }

        /// <summary>
        /// Checks if the item can be added to this container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <param name="slot">Required or next free slot</param>
        /// <param name="createSlot">Should a slot be created if the container is dynamic.</param>
        /// <returns>Returns true if the item can be added.</returns>
        public virtual bool CanAddItem(T item, out UISlot<T> slot, bool createSlot = false)
        {
            slot = null;
            if (item == null) { return true; }

            for (int i = 0; i < m_Slots.Count; i++)
            {
                if (m_Slots[i].IsEmpty && m_Slots[i].CanAddItem(item))
                {
                    slot = m_Slots[i];
                    return true;
                }
            }

            if (m_DynamicContainer)
            {
                if (createSlot)
                {
                    slot = CreateSlot();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Refreshs the slot list and reorganize indices
        /// </summary>
        public void RefreshSlots()
        {
            if (m_DynamicContainer && m_SlotParent != null)
            {
                m_Slots = m_SlotParent.GetComponentsInChildren<UISlot<T>>(true).ToList();
                m_Slots.Remove(m_SlotPrefab.GetComponent<UISlot<T>>());
            }
            else
            {
                m_Slots = GetComponentsInChildren<UISlot<T>>(true).ToList();
            }

            for (int i = 0; i < m_Slots.Count; i++)
            {
                UISlot<T> slot = m_Slots[i];
                slot.Index = i;
                slot.Container = this;
            }
        }

        /// <summary>
        /// Creates a new slot
        /// </summary>
        protected virtual UISlot<T> CreateSlot()
        {
            if (m_SlotPrefab != null && m_SlotParent != null)
            {
                GameObject go = Instantiate(m_SlotPrefab, m_SlotParent, false);
                go.SetActive(true);
                UISlot<T> slot = go.GetComponent<UISlot<T>>();
                m_Slots.Add(slot);
                slot.Index = Slots.Count - 1;
                slot.Container = this;
                return slot;
            }
            Debug.LogWarning("Please ensure that the slot prefab and slot parent is set in the inspector.");
            return null;
        }

        /// <summary>
        /// Destroy the slot and reorganize indices.
        /// </summary>
        /// <param name="slotID">Slot I.</param>
        protected virtual void DestroySlot(int slotID)
        {
            if (slotID < m_Slots.Count)
            {
                DestroyImmediate(m_Slots[slotID].gameObject);
                RefreshSlots();
            }
        }
    }
}