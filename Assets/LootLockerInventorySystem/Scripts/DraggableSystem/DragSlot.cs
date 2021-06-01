using UnityEngine;
using UnityEngine.EventSystems;

namespace LootLocker.InventorySystem
{
    public abstract class DragSlot : MonoBehaviour, IDropHandler
    {
        /// <summary>
        /// This lets us know an item has been dropped in a slot
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.selectedObject)
            {
                OnDrop(eventData.selectedObject);
            }
        }

        protected abstract void OnDrop(GameObject dropped);
    }
}