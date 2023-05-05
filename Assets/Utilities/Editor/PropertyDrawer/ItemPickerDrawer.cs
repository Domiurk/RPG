using System.Collections.Generic;
using Items;
using Runtime.Items;
using UnityEditor;
using UnityEngine;
using Utilities.Runtime.Attributes;

namespace Utilities.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(ItemPickerAttribute))]
    public class ItemPickerDrawer : PickerDrawer<Item>
    {
        protected override List<Item> GetItems(ItemDatabase database)
        {
            return database.Items;
        }
    }
}