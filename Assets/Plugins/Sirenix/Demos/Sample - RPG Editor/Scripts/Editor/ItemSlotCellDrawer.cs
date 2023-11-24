#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using UnityEngine;
    using Editor.Drawers;
    using Sirenix.Utilities.Editor;
    using Utilities;
    using UnityEditor;

    internal sealed class ItemSlotCellDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, ItemSlot>
        where TArray : System.Collections.IList
    {
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = true,
                HideRowIndices = true,
                ResizableColumns = false
            };
        }

        protected override ItemSlot DrawElement(Rect rect, ItemSlot value)
        {
            int id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, value.Item ? value.Item.Icon : null, null, id);

            if (value.Item != null)
            {
                Rect countRect = rect.Padding(2).AlignBottom(16);
                value.ItemCount = EditorGUI.IntField(countRect, Mathf.Max(1, value.ItemCount));
                GUI.Label(countRect, "/ " + value.Item.ItemStackSize, SirenixGUIStyles.RightAlignedGreyMiniLabel);
            }

            value = DragAndDropUtilities.DropZone(rect, value);
            value.Item = DragAndDropUtilities.DropZone(rect, value.Item);
            value = DragAndDropUtilities.DragZone(rect, value, true, true);

            return value;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);

            Rect rect = GUILayoutUtility.GetRect(0, 40).Padding(2);
            int id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, null as Object, null, id);
            DragAndDropUtilities.DropZone(rect, new ItemSlot(), false, id);
        }
    }

}
#endif
