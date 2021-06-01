using System;

namespace LootLocker.InventorySystem
{
    public class ItemsListUI : UIScreen
    {
        public InventoryUI inventoryUI;
        public InventoryData inventoryData;

        public override void Open(Action onOpen = null)
        {
            base.Open(onOpen);
        }


        public void AddItemToInventory(string item)
        {
            // LoadingManager.ShowLoadingScreen();

            inventoryData.AddToInventory(item, onSaveCompleted: () =>
            {
                inventoryUI.ShowInventoryList();
            //  LoadingManager.HideLoadingScreen();
        });
        }
    }
}