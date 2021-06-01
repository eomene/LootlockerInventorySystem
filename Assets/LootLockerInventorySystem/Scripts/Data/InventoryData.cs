using System;
using System.Collections.Generic;
using Cradaptive.ServerRequests;
using Cradaptive.MultipleTextureDownloadSystem;
using LootLocker.Requests;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Cradaptive.TagNotificationSystem;

namespace LootLocker.InventorySystem
{
    public interface IContainsInstanceID
    {
        public int instance_id { get; }
    }

    /// <summary>
    /// Since we want to use interfaces for logic, we created a new class that implements the interfaces we need but inherits from the response
    /// gotten from lootlocker 
    /// </summary>
    public class CradaptiveLootLockerInventoryItem : LootLockerInventory, IListData, ICradaptiveTextureOwner, IDropChecker, IContainsInstanceID, ISyncToServerControl, ITaggedData, IServerErrorHandler
    {
        public string name => asset.name;
        public GameObject mainObject { get; set; }
        public string url => asset?.files != null && asset.files.Length > 0 ? asset.files[0].url : "";
        public Image preview { get; set; }
        public int downloadAttempts { get; set; }

        public string checkName => asset.context;

        public bool syncToServer { get; set; }

        public EventNotificationTypes tag => new EventNotificationTypes { category = "Slots", tag = asset.context };

        public Action<Sprite> OnTextureAvailable { get; set; }
        public Action onServerSyncFailed { get; set; }

        bool IEquatable<IListData>.Equals(IListData other)
        {
            if (other is IContainsInstanceID otherInstance)
            {
                return this.instance_id == otherInstance.instance_id;
            }
            else
            {
                return false;
            }
        }
    }


    [CreateAssetMenu(fileName = "InventoryData", menuName = "InventorySystem/InventoryData", order = 1)]
    public class InventoryData : ScriptableObject
    {
        public IReadOnlyCollection<CradaptiveLootLockerInventoryItem> Inventory => _inventory;
        CradaptiveLootLockerInventoryItem[] _inventory = Array.Empty<CradaptiveLootLockerInventoryItem>();
        public event Action OnInventoryChanged;

        /// <summary>
        /// Gets the inventory data from the server. Tells us when the response has been successfuly gotten. Api used can be found here 
        /// https://docs.lootlocker.io/game-api/#get-inventory-list
        /// </summary>
        /// <param name="onFinishedFetch"></param>
        public void FetchInventoryFromServer(Action onFinishedFetch)
        {
            var request = new CriticalServerGetRequest<LootLockerInventoryResponse, CradaptiveLootLockerInventoryItem[]>()
            {

                makeRequest = onResponseReceived => LootLockerSDKManager.GetInventory(onResponseReceived),
                responseIsSuccesful = response => response.success,
                extractResult = response =>
                {
                    string inventoryJSON = JsonConvert.SerializeObject(response.inventory);
                    return JsonConvert.DeserializeObject<CradaptiveLootLockerInventoryItem[]>(inventoryJSON);
                }
            };

            CriticalServerRequestDispatcher.Execute(request, inventory =>
            {
                _inventory = inventory;
                OnInventoryChanged?.Invoke();
                onFinishedFetch();
            });
        }

        /// <summary>
        /// Allows you to add items to the inventory using a trigger. More information about triggers can be found here 
        /// https://docs.lootlocker.io/game-api/#trigger-events 
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="onSaveCompleted"></param>
        public void AddToInventory(string itemName, Action onSaveCompleted)
        {
            var request = new CriticalServerRequest<LootLockerTriggerAnEventResponse>
            {
                // Note: Triggers are set in the lootlocker dashboard, for every item you might want the user to collect
                // a trigger should exist for this
                makeRequest = onResponseReceived => LootLockerSDKManager.TriggeringAnEvent(itemName, onResponseReceived),
                responseIsSuccesful = response => response.success
            };

            CriticalServerRequestDispatcher.Execute(request, onCompleted: () =>
            {
                FetchInventoryFromServer(onFinishedFetch: onSaveCompleted);
            });
        }
    }
}
