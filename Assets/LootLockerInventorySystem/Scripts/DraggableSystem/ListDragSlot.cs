using Cradaptive.PopUps;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    [RequireComponent(typeof(ListSlot))]
    public class ListDragSlot : DragSlot
    {
        [SerializeField] bool enableItemReplacement = true;


        /// <summary>
        /// This is a special type of drag slot that allows a list item to be equipped to it
        /// </summary>
        /// <param name="dropped"></param>
        protected override void OnDrop(GameObject dropped)
        {
            ///First we check if compenent of type list item was dropped in the slot
            if (dropped.TryGetComponent(out ListItem listItem))
            {
                ///Now we check if this drag slot also has a list slot
                ListSlot thisSlot = GetComponent<ListSlot>();

                ///if this list slot where we are currently has an item and this drag slot doesnt allow items to be replaced
                ///Then we want to return back the dropped item.
                
                if(thisSlot.IsFilled && !enableItemReplacement)
                {
                    ReturnToPreviousPosition(dropped);
                    //Lets show a popup letting them know what happened
                    PopUpManager.ShowPopUp(new PopUpData()
                    {
                        text = "Please Unequip before trying to equip new item"
                    });
                    return;
                }
                ///Now we get the slot of the item that was dropped,i.e where the item used to be
                ListSlot previousSlot = listItem.listSlot;

                ///Now we checked if our list slot can accept the item that was dropped on it. Maybe its a different context for 
                ///example
                if (thisSlot.AcceptsItem(listItem, out string thisSlotRejectReason))
                {
                    ///if it can accept, thats good. but now we want to know if our slot is filled
                    if (thisSlot.IsFilled)
                    {
                        ///We also check if the slot of the item that was dropped is valid
                        if (previousSlot)
                        {
                            ///now we also check if the slot of the item that was dropped can accept th item currently in our slot
                            if (previousSlot.AcceptsItem(thisSlot.SlotContent, out string otherSlotRejectReason))
                            {
                                ///if its possible. Then we swap slots so our item can go to the drop of the new item that was dropped
                                SwapContents(thisSlot, previousSlot);
                            }
                            else
                            {
                                ///otherwise, we retun the item back to where it was and let the user know why
                                ReturnToPreviousPosition(dropped);
                                PopUpManager.ShowPopUp(new PopUpData()
                                {
                                    text = otherSlotRejectReason
                                });
                            }
                        }
                        else
                        {
                            ReturnToPreviousPosition(dropped);
                        }
                    }
                    else
                    {
                        //if our slot is not filled then we can easily free the slot of the item that was dropped 
                        previousSlot?.FreeSlot();
                        //and add the item to our slot
                        thisSlot.AddToSlot(listItem);
                    }
                }
                else
                {
                    //If we cant add this slot, then we let them know we cant and return it back to where it came from
                    PopUpManager.ShowPopUp(new PopUpData()
                    {
                        text = thisSlotRejectReason
                    });

                    ReturnToPreviousPosition(dropped);
                }
            }
        }

        /// <summary>
        /// Alows us to swap items in slots
        /// </summary>
        /// <param name="slotA"></param>
        /// <param name="slotB"></param>
        static void SwapContents(ListSlot slotA, ListSlot slotB)
        {
            ListItem slotAContent = slotA.SlotContent;
            slotA.AddToSlot(slotB.SlotContent);
            slotB.AddToSlot(slotAContent);
        }

        /// <summary>
        /// returns a drag item to its previous slot
        /// </summary>
        /// <param name="dropped"></param>
        static void ReturnToPreviousPosition(GameObject dropped)
        {
            if (dropped.TryGetComponent(out DragItem dragItem))
            {
                dragItem.ReturnToPreviousPosition();
            }
        }
    }
}