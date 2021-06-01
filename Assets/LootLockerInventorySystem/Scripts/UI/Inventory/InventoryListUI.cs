using System;
using System.Linq;
using Cradaptive.ServerRequests;
using LootLocker.Requests;

namespace LootLocker.InventorySystem
{
    public class InventoryListUI : UIScreen, IConsumeSRPResponse
    {
        public InventoryData inventoryData;
        public CharacterData characterData;
        public ListPopulator listPopulator;


        public override void Open(Action onOpen = null)
        {
            if (listPopulator)
            {
                IListData[] inventoryWithoutEquippedItems = inventoryData.Inventory.Where(
                    item => !characterData.HasItemEquipped(item.instance_id)).ToArray();

                listPopulator.PopulateList(inventoryWithoutEquippedItems, clearList: false);
            }

            base.Open(onOpen);
        }


        public void ConsumeResponse(object response)
        {
            ///we know this is the class that comes from the background dispatcher. This contains the previous request as well as the response gotten
            ///Now we can cast to the kind of response or request this class can handle, and apply it appropraitely.
            ///We know if its an equip request, it can be handled here, if its not we dont care about the response
            ///Ofcourse unequip will probably be handled by the inventory 
            BackgroundRequestsResponse mainResponse = response as BackgroundRequestsResponse;
            //now we try to see if this is a response this class is familiar with
            EquipAssetToCharacterLoadoutResponse mainData = mainResponse.jsonResponse as EquipAssetToCharacterLoadoutResponse;
            ///now i know this is a response I can handle, but I still dont know if its an equip ot unequip request. This will determine the final way
            ///I can use the response data
            if (mainData != null)
            {
                try
                {
                    //now i want to check if its an equip call
                    UnequipItemRequest equipItemRequest = (UnequipItemRequest)mainResponse.requestResponse;

                    //TODO:find a better way to check if its null or just make a class that is nullable
                    if (!string.IsNullOrEmpty(equipItemRequest.itemInstanceID))
                    {
                        //now i know this is a valid unequip request. I need to find the item that is currently in the loadout and move it here 

                    }
                }
                catch (Exception e)
                {

                }


            }
        }
    }
}