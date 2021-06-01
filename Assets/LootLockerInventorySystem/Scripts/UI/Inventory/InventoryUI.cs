using System;

namespace LootLocker.InventorySystem
{
    public class InventoryUI : UIScreen, IDragItemParent
    {
        public ItemsListUI itemsListUI;
        public CharacterLoadoutListUI characterLoadoutListUI;
        public InventoryListUI inventoryListUI;

        public override void Open(Action onOpen = null)
        {
            ShowInventoryList();
            ShowCharacterList();

            base.Open(onOpen);
        }

        public void ShowInventoryList()
        {
            inventoryListUI.Open();
        }

        public void ShowCharacterList()
        {
            characterLoadoutListUI?.Open();
        }
    }
}