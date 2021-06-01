using System.Collections.Generic;
using UnityEngine;
namespace LootLocker.InventorySystem
{
    public class StaticSlotListPopulator : SlotListPopulator
    {
        protected override List<ListSlot> Slots => _slots;
        [SerializeField] List<ListSlot> _slots;


        protected override void Awake()
        {
            foreach (ListSlot slot in _slots)
            {
                SubscribeToSlotContentEvents(slot);
            }
        }
    }
}