using System.Collections.Generic;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public interface IDropChecker
    {
        string checkName { get; }
    }

    public class ItemListPopulator : ListPopulator
    {
        public Transform parent;
        public GameObject prefab;


        protected override void Awake()
        {
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out ListItem item))
                {
                    AddItem(item);
                }
            }

            base.Awake();
        }

        /// <summary>
        /// Adds new items to the list 
        /// </summary>
        /// <param name="newData"></param>
        protected override void AddNewData(IReadOnlyCollection<IListData> newData)
        {
            foreach (IListData itemData in newData)
            {
                ListItem listItem = Instantiate(prefab, parent)?.GetComponent<ListItem>();
                listItem?.InitItem(itemData);

                AddItem(listItem);
            }
        }
    }
}