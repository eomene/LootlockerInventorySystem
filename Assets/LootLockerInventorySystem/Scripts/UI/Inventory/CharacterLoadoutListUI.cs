using System;
using System.Linq;
using LootLocker.InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Cradaptive.ServerRequests;
using LootLocker.Requests;
using Newtonsoft.Json;
using Cradaptive.MultipleTextureDownloadSystem;


namespace LootLocker.InventorySystem
{

    public class CharacterLoadoutListUI : UIScreen, IConsumeSRPResponse, IConsumeSRPPendingRequests
    {
        [SerializeField] CharacterData characterData;
        [SerializeField] InventoryData inventoryData;

        [FormerlySerializedAs("listPopulator"), SerializeField] SlotListPopulator loadoutSlotPopulator;

        [SerializeField] Text characterNameField;
        [SerializeField] Text characterTypeField;
        [SerializeField] Image characterImage;

        [SerializeField] ListItem itemPrefab;

        public override void Open(Action onOpen = null)
        {
            loadoutSlotPopulator.PopulateList(characterData.CurrentCharacter.equippableContexts);

            var equippedItems = inventoryData.Inventory.Where(item => characterData.HasItemEquipped(item.instance_id));
            foreach (CradaptiveLootLockerInventoryItem equippedItem in equippedItems)
            {
                equippedItem.syncToServer = true;
                EquipItemToSlot(equippedItem.asset.context, equippedItem);
            }

            characterNameField.text = characterData.CurrentCharacter.character.name;
            characterTypeField.text = characterData.CurrentCharacter.character.type;

            characterData.CurrentCharacter.OnTextureAvailable = SetCharacterImage;
            CradaptiveTexturesSaver.QueueForDownload(characterData.CurrentCharacter);

            base.Open(onOpen);
        }

        public void EquipItemToSlot(string context, IListData equippedItem)
        {
            ListSlot contextSlot = loadoutSlotPopulator.GetSlotByContentName(context);
            ListSlot mainSlot = contextSlot.SlotContent.GetComponent<ListSlot>();

            ListItem item = Instantiate(itemPrefab);
            item.transform.localScale = Vector3.one;
            item.InitItem(equippedItem);

            mainSlot.AddToSlot(item);

        }

        /// <summary>
        /// if we want to act only when the right response comes from the server. Then we can listen for this event
        /// </summary>
        /// <param name="respose"></param>
        public void ConsumeResponse(object respose)
        {
            //Important, we should be able to check if this is already equipped, so its not repeated
            //commenting out for now 
            //remmeber to turn on the config bool that allows this callback to be fired

            /////we know this is the class that comes from the background dispatcher. This contains the previous request as well as the response gotten
            /////Now we can cast to the kind of response or request this class can handle, and apply it appropraitely.
            /////We know if its an equip request, it can be handled here, if its not we dont care about the response
            /////Ofcourse unequip will probably be handled by the inventory 
            //BackgroundRequestsResponse mainResponse = respose as BackgroundRequestsResponse;
            ////now we try to see if this is a response this class is familiar with
            //EquipAssetToCharacterLoadoutResponse mainData = mainResponse.jsonResponse as EquipAssetToCharacterLoadoutResponse;
            /////now i know this is a response I can handle, but I still dont know if its an equip ot unequip request. This will determine the final way
            /////I can use the response data
            //if (mainData != null)
            //{
            //    try
            //    {
            //        //now i want to check if its an equip call
            //        EquipItemRequest equipItemRequest = (EquipItemRequest)mainResponse.requestResponse;

            //        //TODO:find a better way to check if its null or just make a class that is nullable
            //        if (!string.IsNullOrEmpty(equipItemRequest.itemInstanceID))
            //        {
            //            //Now i know this is an equip request
            //            LootLockerLoadouts loadouts = mainData.loadout.FirstOrDefault(x => x.instance_id.ToString() == equipItemRequest.itemInstanceID);

            //            CradaptiveLootLockerInventoryItem newItem = new CradaptiveLootLockerInventoryItem();
            //            newItem.instance_id = loadouts.instance_id;
            //            newItem.asset = loadouts.asset;
            //            //Added a new interface that can be used to check if this should be synced to the server, otherwise we will get an infinite server
            //            //loop, we are telling it not to try to sync this again.
            //            //Only drag and drop calls should be synced not the ones we are applying
            //            newItem.syncToServer = false;
            //            //Very important we figure out how to handle if an item is already on the slot
            //            //My quick idea now is, if the item there is exactly what we got from this request, then ofcourse we can ignore it
            //            //if it is a different item, then we can unequip that item to the inventory and equip this instead.
            //            //this item can be placed anywhere in the inventory slot. and maybe the inventory code handles the unequipping.
            //            EquipItemToSlot(loadouts.asset.context, newItem);
            //        }
            //    }
            //    catch (Exception e)
            //    {

            //    }


            //}
        }

        //if we would rather act on the requests themselves. So we do not care whether or not a successfull response has been made
        //then lets act on the response itself, I think this might be the safer route. Since we expect to get the current state of the inventory
        //the item is currently either in the inventory if we previously equipped it or in the loadout if we unequipped. We can act either way with this
        //info, the only downside is we might have multiple unequip/equip sessions based on the lenght of data stored
        //maybe we can have a threshold in the config after which we give the user an error. Maybe after 20 requests or something.
        public void ConsumeRequests(object srpresponse)
        {
            // throw new NotImplementedException();
        }

        public void SetCharacterImage(Sprite image)
        {
            characterImage.sprite = image;
        }
    }
}