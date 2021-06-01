using UnityEngine;
using UnityEngine.Serialization;

namespace LootLocker.InventorySystem
{
    public class ListSlot : MonoBehaviour
    {
        [HideInInspector] public ListItem SlotContent => _slotContent;
        [HideInInspector] [FormerlySerializedAs("_listItem"), SerializeField] ListItem _slotContent;
        [HideInInspector] public ListPopulator owner;

        public bool IsFilled => _slotContent != null;


        public event System.Action<ListItem> OnRemovedContent;
        public event System.Action<ListItem> OnAddedContent;

        private void Awake()
        {
            owner = GetComponentInParent<ListPopulator>();
        }


        public virtual bool AcceptsItem(ListItem item, out string rejectionReason)
        {
            rejectionReason = null;
            return true;
        }

        public virtual void AddToSlot(ListItem newListItem)
        {
            if (IsFilled)
            {
                FreeSlot();
            }

            if (newListItem != null)
            {
                _slotContent = newListItem;
                _slotContent.SetSlot(this);

                _slotContent.transform.SetParent(this.transform);
                _slotContent.transform.localPosition = Vector3.zero;
                _slotContent.transform.localScale = Vector3.one;
                _slotContent.gameObject.SetActive(true);

                OnAddedToSlot(newListItem.listData);
                OnAddedContent?.Invoke(_slotContent);

                ListSlot listSlot = newListItem.GetComponent<ListSlot>();
                if (listSlot != null)
                    listSlot.owner = owner;
            }
        }

        public virtual void FreeSlot()
        {
            ListItem removedContent = _slotContent;
            _slotContent = null;

            if (removedContent != null)
            {
                OnRemovedContent?.Invoke(removedContent);
            }
        }


        protected virtual void OnAddedToSlot(IListData newData) { }
    }
}