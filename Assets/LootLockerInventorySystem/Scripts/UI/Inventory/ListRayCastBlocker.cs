using LootLocker.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LootLocker.InventorySystem
{
    public class OnDragStarted : UnityEvent<ListRayCastBlocker> { }

    public class ListRayCastBlocker : MonoBehaviour, IDragCallBacksHolder
    {
        public static OnDragStarted onDragStarted = new OnDragStarted();
        public static OnDragStarted onDragEnded = new OnDragStarted();

        public DragItem dragger { get; set; }
        CanvasGroup canvasGroup;


        private void Awake()
        {
            dragger = GetComponent<DragItem>();
            onDragStarted.AddListener(DragStarted);
            onDragEnded.AddListener(DragEnded);
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(Transform prevParent)
        {
            onDragStarted?.Invoke(this);
        }

        public void DragStarted(ListRayCastBlocker inventoryListItem)
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
        }

        public void DragEnded(ListRayCastBlocker inventoryListItem)
        {
            canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
        }
        public void OnEndDrag()
        {
            onDragEnded?.Invoke(this);
        }
    }
}