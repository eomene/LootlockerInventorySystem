using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LootLocker.InventorySystem
{
    public abstract class SlotListPopulator : ListPopulator
    {
        protected abstract List<ListSlot> Slots { get; }
        protected List<ListSlot> FreeSlots { get; } = new List<ListSlot>(); // Not guaranteed to be up to date, call RefreshFreeSlots() first if needed.
        [SerializeField] ListItem itemPrefab;
        [SerializeField] bool emptySlotsWhenNewItemsAreAdded;

        protected override void Awake()
        {
            // Get the preexisting slot contents before initialising
            foreach (ListSlot filledSlot in Slots.Where(slot => slot.IsFilled))
            {
                AddItem(filledSlot.SlotContent);
            }

            base.Awake();

            RefreshFreeSlots();
        }

        public ListSlot GetSlotByContentName(string name)
        {
            return Slots.FirstOrDefault(slot => slot.IsFilled &&
                name.Equals(slot.SlotContent.name, System.StringComparison.OrdinalIgnoreCase)
            );
        }


        protected override void AddNewData(IReadOnlyCollection<IListData> newData)
        {
            if (emptySlotsWhenNewItemsAreAdded)
                FreeAllSlots();

            RefreshFreeSlots();

            int nextFreeSlotIndex = 0;
            foreach (IListData dataEntry in newData)
            {
                if (nextFreeSlotIndex < FreeSlots.Count)
                {
                    FreeSlots[nextFreeSlotIndex++].AddToSlot(CreateItem(dataEntry));
                }
                else
                {
                  //  UnityEngine.Debug.LogWarning($"SlotListPopulator at {name} ran out of free slots while populating.", this);
                    break;
                }
            }

            RefreshFreeSlots();
        }

        protected override void OnClearList()
        {
            RefreshFreeSlots();
        }


        protected void RefreshFreeSlots()
        {
            FreeSlots.Clear();
            FreeSlots.AddRange(Slots.Where(slot => !slot.IsFilled));
        }

        protected void SubscribeToSlotContentEvents(ListSlot slot)
        {
            slot.OnAddedContent += AddItem;
            slot.OnRemovedContent += RemoveItem;
        }


        ListItem CreateItem(IListData data)
        {
            ListItem item = Instantiate(itemPrefab);
            item.InitItem(data);

            // We mustn't AddItem() here. It will be called when it gets added to a slot.

            return item;
        }

        public void FreeAllSlots()
        {
            for (int i = 0; i < Slots.Count; i++)
                Slots[i].FreeSlot();
        }
    }
}