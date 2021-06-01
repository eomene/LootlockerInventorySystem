using Cradaptive.TagNotificationSystem;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    [RequireComponent(typeof(ListItem))]
    public class LoadoutSlot : ListSlot
    {
        public CharacterData characterData;

        ListItem loadoutListItem;


        void Awake()
        {
            loadoutListItem = GetComponent<ListItem>();
        }


        public override bool AcceptsItem(ListItem item, out string rejectReason)
        {
            IDropChecker inventory = item.listData as IDropChecker;
            IDropChecker context = loadoutListItem.listData as IDropChecker;

            bool accepted = item.listData is IContainsInstanceID && context != null &&
                context.checkName.ToLower() == inventory.checkName.ToLower();

            rejectReason = accepted ? null : "Item did not match the loadout slot type.";
            if (accepted)
            {
                if ((item.listData as ISyncToServerControl) != null)
                    (item.listData as ISyncToServerControl).syncToServer = true;
            }

            return accepted;
        }

        protected override void OnAddedToSlot(IListData newData)
        {
            //TODO: cache this so its not called twice
            int assetInstanceID = (newData as IContainsInstanceID) == null ? -1 : (newData as IContainsInstanceID).instance_id;
            bool syncToServer = (newData as ISyncToServerControl) == null ? true : (newData as ISyncToServerControl).syncToServer;
            if (!characterData.HasItemEquipped(assetInstanceID))
            {
                characterData.EquipItem(assetInstanceID, syncToServer);
            }
        }

        public override void FreeSlot()
        {
            int assetInstanceID = (SlotContent.listData as IContainsInstanceID).instance_id;

            if (characterData.HasItemEquipped(assetInstanceID))
            {
                characterData.UnequipItem(assetInstanceID);
            }

            base.FreeSlot();
        }

        public override void AddToSlot(ListItem newListItem)
        {
            base.AddToSlot(newListItem);
            CradaptiveNotificationReceiver cradaptiveNotificationReceiver = newListItem.GetComponent<CradaptiveNotificationReceiver>();
            if (cradaptiveNotificationReceiver)
                cradaptiveNotificationReceiver.receiveEvents = true;
        }
    }
}