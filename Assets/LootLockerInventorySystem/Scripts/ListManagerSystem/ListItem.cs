using Cradaptive.MultipleTextureDownloadSystem;
using Cradaptive.TagNotificationSystem;
using UnityEngine;
using UnityEngine.UI;

namespace LootLocker.InventorySystem
{
    public interface IListItemCallBack
    {
        void OnButtonClicked();
        void ListItemInitialised(IListData listData);
    }


    public class ListItem : MonoBehaviour
    {
        [UnityEngine.Serialization.FormerlySerializedAs("tittle")] public Text title;
        public Image icon;
        [HideInInspector] public Button listButton;

        IListItemCallBack listItemCallBack;

        public ListSlot listSlot { get; private set; }
        public IListData listData { get; private set; }


        private void Awake()
        {
            listItemCallBack = GetComponent<IListItemCallBack>();
            if (listButton != null)
                listButton.onClick.AddListener(OnButtonClicked);
        }

        public void SetSlot(ListSlot listSlots)
        {
            this.listSlot = listSlots;
        }

        public void InitItem(IListData listData, ListSlot listSlots = null)
        {
            this.listData = listData;
            if (listSlots != null)
                this.listSlot = listSlots;

            gameObject.name = listData.name;

            if (listData is ICradaptiveTextureOwner textureOwner)
            {
                textureOwner.OnTextureAvailable = SetImage;
                CradaptiveTexturesSaver.QueueForDownload(textureOwner);
            }

            listItemCallBack?.ListItemInitialised(listData);
            if (title != null)
            {
                IListFriendlyName listFriendlyName = listData as IListFriendlyName;
                title.text = listFriendlyName == null ? listData.name : listFriendlyName.friendlyName;
            }

            ITaggedData taggedData = listData as ITaggedData;
            CradaptiveNotificationReceiver cradaptiveNotificationReceiver = GetComponent<CradaptiveNotificationReceiver>();
            if (taggedData != null)
            {
                if (cradaptiveNotificationReceiver != null)
                    cradaptiveNotificationReceiver.currentNotificationTag = taggedData.tag;
            }

            if (GetComponentInParent<LoadoutSlot>() != null)
            {
                cradaptiveNotificationReceiver.receiveEvents = true;
            }
        }

        public void SetImage(Sprite image)
        {
            if (icon != null)
                icon.sprite = image;
        }

        public void OnButtonClicked()
        {
            listItemCallBack?.OnButtonClicked();
        }
    }
}