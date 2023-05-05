using System.Collections.Generic;
using Runtime.Items;
using UnityEditor;
using Utilities.Runtime;

namespace Utilities.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(BonePickerAttribute))]
    public class BonePickerDrawer : PickerDrawer<Bone>
    {
        protected override List<Bone> GetItems(ItemDatabase database)
            => database.Bones;
    }
}