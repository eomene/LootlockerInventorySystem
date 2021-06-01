using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class BlankDragSlot : DragSlot
    {
        protected override void OnDrop(GameObject dropped)
        {
            if (dropped.TryGetComponent(out DragItem dragItem))
            {
                dragItem.ReturnToPreviousPosition();
            }
        }
    }
}