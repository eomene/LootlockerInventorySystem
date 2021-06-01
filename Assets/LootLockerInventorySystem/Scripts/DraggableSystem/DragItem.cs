using Cradaptive.TagNotificationSystem;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LootLocker.InventorySystem
{
    public interface IDragCallBacksHolder
    {
        DragItem dragger { get; set; }
        void OnBeginDrag(Transform parent);
        void OnEndDrag();
    }

    public interface IDragItemParent
    {
        Transform transform { get; }
    }

    public class DragItem : MonoBehaviour, IDragHandler, IDropHandler, IEndDragHandler, IBeginDragHandler
    {
        IDragCallBacksHolder dragCallbackHolder;
        CanvasGroup canvasGroup;
        RectTransform rectTransform;
        EventNotificationTypes sendTag;
        DragData currentDrag;
        bool allowDrag;


        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            dragCallbackHolder = GetComponent<IDragCallBacksHolder>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ///We create a new drag data for the characteristics of this object being dragged for future purposes
            currentDrag = new DragData()
            {
                prevParent = transform.parent,
                previousLocalPosition = transform.localPosition,
                canvas = GetComponentInParent<Canvas>()
            };

            ///We prevent this item from blocking raycasts, so we cn know where it is dropped
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
            ///Lets others that would like to know that this item is getting dragged. Those are basically components that implement this
            ///interface
            dragCallbackHolder?.OnBeginDrag(currentDrag.prevParent);

            ///if we have a drag parent, then we should parent it to that. This is necessary to have it show in front of other items
            IDragItemParent dragItemParent = GetComponentInParent<IDragItemParent>();
            if (dragItemParent != null)
            {
                transform.SetParent(dragItemParent.transform);
            }
            ///if it has list data, we want to tell others its tag. Others might want to know which type of item is being dragged
            ///This allows us to make the potential slots red if it cant fit in or not if it can
            IListData listData = GetComponent<ListItem>()?.listData;
            ITaggedData taggedData = listData as ITaggedData;

            sendTag = taggedData != null ? taggedData.tag : null;

            CradaptiveSender.SendNotificationToTags(sendTag);

            allowDrag = true;
        }


        /// <summary>
        /// Something happend and we want this item to go back to its previous position. Remember we already know all this from the drag
        /// data we saved when the drag started
        /// </summary>
        public void ReturnToPreviousPosition()
        {
            transform.SetParent(currentDrag.prevParent);
            transform.localPosition = currentDrag.previousLocalPosition;
        }


        /// <summary>
        /// Item is currently being dragged, lets make it follow the mouse position
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            //  Debug.LogError($"Mouse Y:{Input.mousePosition.y} //Screen Height: {Screen.height} // Mouse X:{Input.mousePosition.x} //Screen Height: {Screen.width}");

            if (CheckScreen())
            {
                OnEndDrag(null);
                ReturnToPreviousPosition();
                allowDrag = false;
                return;
            }

            if (allowDrag)
                rectTransform.anchoredPosition += eventData.delta / currentDrag.canvas.scaleFactor;
        }

        private bool CheckScreen()
        {
            return (Input.mousePosition.y <= 0 || Input.mousePosition.y > Screen.height || Input.mousePosition.x <= 0 || Input.mousePosition.x > Screen.width);
        }

        /// <summary>
        /// item has been dropped
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {

        }

        /// <summary>
        /// The item is no longer being dragged, we want to tell whoever is listening the the tag of the item that was dropped 
        /// and also all the raycast to be blocked again so we can be picked up
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
            dragCallbackHolder?.OnEndDrag();

            if (sendTag != null)
                CradaptiveSender.EndNotificationToTags(sendTag);
        }


        struct DragData
        {
            public Transform prevParent;
            public Vector3 previousLocalPosition;

            public Canvas canvas;
        }
    }
}