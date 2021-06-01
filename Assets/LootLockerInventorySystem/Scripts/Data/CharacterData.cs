using System;
using System.Collections.Generic;
using System.Linq;
using Cradaptive.ServerRequests;
using Cradaptive.MultipleTextureDownloadSystem;
using LootLocker;
using LootLocker.Requests;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Cradaptive.TagNotificationSystem;

namespace LootLocker.InventorySystem
{
    /// <summary>
    /// If you would like to use interfaces for your data. A good way to acheive this is to create your own data class and pass lootlocker 
    /// data results into it during your code
    /// </summary>
    public class LootLockerCharacterData : IListData, ICradaptiveTextureOwner
    {
        public readonly List<int> equippedItemIDs = new List<int>();
        public CradaptiveLootLockerContext[] equippableContexts;

        public LootLockerCharacter character;

        public string name { get => string.IsNullOrEmpty(character.name) ? character.type : character.name; }

        public string url => character.type;

        public Action<Sprite> OnTextureAvailable { get; set; }
        public int downloadAttempts { get; set; }

        public bool Equals(IListData other)
        {
            return Equals((object)other);
        }
    }

    public class CradaptiveLootLockerContext : LootLockerContext, IListData, ICradaptiveTextureOwner, IDropChecker, ITaggedData, IListFriendlyName
    {
        public string url => name + "Empty";
        public Action<Sprite> OnTextureAvailable { get; set; }
        public int downloadAttempts { get; set; }

        public string checkName => name;

        public EventNotificationTypes tag => new EventNotificationTypes { category = "Slots", tag = name };

        bool IEquatable<IListData>.Equals(IListData other)
        {
            return ReferenceEquals(this, other);
        }
        string IListData.name => name;

        public string friendlyName => friendly_name;
    }

    public class CradaptiveLootLockerCharacter_Types : LootLockerCharacter_Types, ICradaptiveTextureOwner, IListData
    {
        public string url => name;

        public Action<Sprite> OnTextureAvailable { get; set ; }

        string IListData.name => name;

        public bool Equals(IListData other)
        {
            return true;
        }
    }


    [CreateAssetMenu(fileName = "CharactersData", menuName = "InventorySystem/CharactersData", order = 2)]
    public class CharacterData : ScriptableObject
    {
        public LootLockerCharacterData CurrentCharacter { get; private set; }

        /// <summary>
        /// This gets the characters from lootlocker, it has an action callback for when results return from the server
        /// </summary>
        /// <param name="onCompleted"></param>
        public void GetCharacters(Action<LootLockerCharacterData[]> onCompleted)
        {
            var request = new CriticalServerGetRequest<LootLockerCharacterLoadoutResponse, LootLockerCharacterData[]>()
            {
                makeRequest = onResponseReceived => LootLockerSDKManager.GetCharacterLoadout(onResponseReceived),
                responseIsSuccesful = response => response.success,
                extractResult = response =>
                {
                    string charactersJSON = JsonConvert.SerializeObject(response.loadouts);
                    return JsonConvert.DeserializeObject<LootLockerCharacterData[]>(charactersJSON);
                }
            };

            CriticalServerRequestDispatcher.Execute(request, onCompleted);
        }

        /// <summary>
        /// This returns the character classes from lootlocker, it has a callback to let you know when the results are back
        /// from the server
        /// </summary>
        /// <param name="onCompleted"></param>
        public void GetCharacterClasses(Action<LootLockerCharacter_Types[]> onCompleted)
        {
            ///It is possible that the server call might fail. This is why we have this system built on top of the lootlocker server call
            ///This makes it possible for calls to be cached or retried without you knowing or doing too much 
            var request = new CriticalServerGetRequest<LootLockerListCharacterTypesResponse, LootLockerCharacter_Types[]>()
            {
                makeRequest = onResponseReceived => LootLockerSDKManager.ListCharacterTypes(onResponseReceived),
                responseIsSuccesful = response => response.success,
                extractResult = response => response.character_types
            };
            ///This is responsible for creating a server request that continues to retry in the background. You can check out the settings in 
            ///the path LootLockerInventorySystem\Scripts\ServerRequestsPersistSystem\Resources\CradaptiveSRPSConfig 
            CriticalServerRequestDispatcher.Execute(request, onCompleted);
        }


        /// <summary>
        /// Creates a character on the lootlocker server and lets you know once it has been done 
        /// </summary>
        /// <param name="characterClassID"></param>
        /// <param name="characterName"></param>
        /// <param name="isDefault"></param>
        /// <param name="onCompleted"></param>
        public void CreateCharacter(string characterClassID, string characterName, bool isDefault, Action onCompleted)
        {
            var request = new CriticalServerRequest<LootLockerCharacterLoadoutResponse>()
            {
                makeRequest = onResponseReceived => LootLockerSDKManager.CreateCharacter(characterClassID, characterName, isDefault, onResponseReceived),
                responseIsSuccesful = response => response.success
            };

            CriticalServerRequestDispatcher.Execute(request, onCompleted);
        }

        /// <summary>
        /// This sets a character as a default character and lets you know once it is successful on the backend
        /// </summary>
        /// <param name="currentCharacterAndLoadout"></param>
        /// <param name="onCompleted"></param>
        public void SetCurrentCharacter(LootLockerCharacterData currentCharacterAndLoadout, Action onCompleted)
        {
            CurrentCharacter = currentCharacterAndLoadout;

            string characterID = currentCharacterAndLoadout.character.id.ToString();
            string characterName = currentCharacterAndLoadout.character.name;

            var setCharacterRequest = new CriticalServerGetRequest<LootLockerCharacterLoadoutResponse, LootLockerLoadouts[]>()
            {
                makeRequest = onResponseReceived => LootLockerSDKManager.UpdateCharacter(characterID, characterName, isDefault: true, onResponseReceived),
                responseIsSuccesful = response => response.success,
                extractResult = response => response.loadouts.First(
                    characterLoadout => characterLoadout.character.id.ToString() == characterID).loadout
            };

            CriticalServerRequestDispatcher.Execute(setCharacterRequest, (LootLockerLoadouts[] loadout) =>
            {
            // Get the character's loadout
            CurrentCharacter.equippedItemIDs.Clear();
                CurrentCharacter.equippedItemIDs.AddRange(loadout.Select(item => item.instance_id));

                var getEquippableContextRequest = new CriticalServerGetRequest<LootLockerContextResponse, CradaptiveLootLockerContext[]>()
                {
                    makeRequest = onResponseReceived => LootLockerSDKManager.GetEquipableContextToDefaultCharacter(onResponseReceived),
                    responseIsSuccesful = response => response.success,
                    extractResult = response =>
                    {
                        string contextsJSON = JsonConvert.SerializeObject(response.contexts);
                        return JsonConvert.DeserializeObject<CradaptiveLootLockerContext[]>(contextsJSON);
                    }
                };

            // Get the character's loadout slots
            CriticalServerRequestDispatcher.Execute(getEquippableContextRequest, contexts =>
                {
                    BackgroundServerRequestDispatcher.StartQueueExecution();
                    CurrentCharacter.equippableContexts = contexts;
                    onCompleted.Invoke();
                });
            });
        }

        /// <summary>
        /// This allows you to equip an asset to a character by passing the instance id gotten from the inventory item.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="syncToServer"></param>
        public void EquipItem(int instanceID, bool syncToServer = true)
        {
            CurrentCharacter.equippedItemIDs.Add(instanceID);
            if (!syncToServer) return;
            BackgroundServerRequestDispatcher.Queue(new EquipItemRequest()
            {
                itemInstanceID = instanceID.ToString(),
                characterID = CurrentCharacter.character.id.ToString()
            });
        }


        /// <summary>
        /// This allows you to unequip an asset from a character. This calls are not critical, so it can be done in the background 
        /// without you knowing 
        /// </summary>
        /// <param name="instanceID"></param>
        public void UnequipItem(int instanceID)
        {
            CurrentCharacter.equippedItemIDs.Remove(instanceID);

            BackgroundServerRequestDispatcher.Queue(new UnequipItemRequest()
            {
                itemInstanceID = instanceID.ToString()
            });
        }

        /// <summary>
        /// Checks if an item is currently equipped to a default character on lootlocker
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public bool HasItemEquipped(int instanceID)
        {
            return CurrentCharacter.equippedItemIDs.Any(equippedItemID => equippedItemID == instanceID);
        }
    }

    /// <summary>
    /// Struct responsible for storing information about a background server request. Each Background server requests requires one
    /// </summary>
    struct EquipItemRequest : IBackgroundServerRequest<EquipAssetToCharacterLoadoutResponse>
    {
        public string itemInstanceID;
        public string characterID;

        public void MakeRequest(Action<EquipAssetToCharacterLoadoutResponse> onResponseReceived)
        {
            LootLockerSDKManager.EquipIdAssetToDefaultCharacter(itemInstanceID, onResponseReceived);
        }

        //not sure this is necessary
        public bool ResponseIsSuccesful(EquipAssetToCharacterLoadoutResponse response)
        {
            return response.success;
        }
    }

    struct UnequipItemRequest : IBackgroundServerRequest<EquipAssetToCharacterLoadoutResponse>
    {
        public string itemInstanceID;
        public void MakeRequest(Action<EquipAssetToCharacterLoadoutResponse> onResponseReceived)
        {
            LootLockerSDKManager.UnEquipIdAssetToDefaultCharacter(itemInstanceID, onResponseReceived);
        }

        public bool ResponseIsSuccesful(EquipAssetToCharacterLoadoutResponse response)
        {
            return response.success;
        }

    }
}