using Cradaptive.TagNotificationSystem;
using LootLocker.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListSlot : ListSlot
{
    public override void AddToSlot(ListItem newListItem)
    {
        base.AddToSlot(newListItem);
        CradaptiveNotificationReceiver cradaptiveNotificationReceiver = newListItem.GetComponent<CradaptiveNotificationReceiver>();
        if (cradaptiveNotificationReceiver)
            cradaptiveNotificationReceiver.receiveEvents = false;
    }
}
