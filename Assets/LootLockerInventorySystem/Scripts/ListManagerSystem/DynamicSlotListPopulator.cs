using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public class DynamicSlotListPopulator : SlotListPopulator
    {
        [SerializeField] Transform parent;
        [SerializeField] ListSlot slotPrefab;
        [SerializeField, Min(1)] int slotCountIncrement;

        protected override List<ListSlot> Slots => _slots;
        readonly List<ListSlot> _slots = new List<ListSlot>();


        protected override void Awake()
        {
            // Get the preexisting slots before initialising
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out ListSlot slot))
                {
                    InitializeSlot(slot);
                }
            }

            base.Awake();
        }

        /// <summary>
        /// Adds a list of new data to this slot 
        /// </summary>
        /// <param name="newData"></param>
        protected override void AddNewData(IReadOnlyCollection<IListData> newData)
        {

            SetSlotCount(GetDesiredSlotCount(Items.Count + newData.Count, slotCountIncrement));

            base.AddNewData(newData);
        }

        /// <summary>
        /// Gets us the desired count of the slots. This is because, we might want the slots to grow. For example, the inventory slots 
        /// grow by increament size. i.e when more inventory items come than we currently have slots for, we increase the number of 
        /// available slots 
        /// </summary>
        /// <param name="dataEntryCount"></param>
        /// <param name="incrementSize"></param>
        /// <returns></returns>
        static int GetDesiredSlotCount(int dataEntryCount, int incrementSize)
        {
            int incrementCount = dataEntryCount / incrementSize + 1;
            return incrementCount * incrementSize;
        }

        /// <summary>
        /// This allows the slots to grow based on the amount we pass in, making sure we have more slots than the inventory items we have
        /// </summary>
        /// <param name="desiredSlotCount"></param>
        void SetSlotCount(int desiredSlotCount)
        {
            RefreshFreeSlots();

            ///Get the number of new slots we need
            int requiredNewSlotCount = desiredSlotCount - _slots.Count;

            for (int i = 0; i < requiredNewSlotCount; i++)
            {
                //now lets create thos new slots and added them to our free slots
                FreeSlots.Add(InstantiateSlot());
            }

            ///If we somehow created more than we needed
            int excessSlotCount = _slots.Count - desiredSlotCount;
            //Then lets destroy those slots
            for (int i = 0; i < excessSlotCount; i++)
            {
                ListSlot slotToDestroy = FreeSlots.Count > 0 ? FreeSlots[FreeSlots.Count - 1] : _slots[_slots.Count - 1];
                FreeSlots.Remove(slotToDestroy);
                DestroySlot(slotToDestroy);
            }
        }

        /// <summary>
        /// creates new slots and adds them to the slots we have available
        /// </summary>
        /// <returns></returns>
        ListSlot InstantiateSlot()
        {
            ListSlot newSlot = Instantiate(slotPrefab, parent);
            InitializeSlot(newSlot);

            return newSlot;
        }

        /// <summary>
        /// Initialise the slots
        /// </summary>
        /// <param name="slot"></param>
        void InitializeSlot(ListSlot slot)
        {
            ///Add this slot to the list of slots for the list
            _slots.Add(slot);
            ///Subscribe this slot so the list can know when it a new item was added or removed from it
            SubscribeToSlotContentEvents(slot);
        }

        /// <summary>
        /// Destroys or frees a slot
        /// </summary>
        /// <param name="slot"></param>
        void DestroySlot(ListSlot slot)
        {
            Destroy(slot);
            _slots.Remove(slot);
        }
    }
}