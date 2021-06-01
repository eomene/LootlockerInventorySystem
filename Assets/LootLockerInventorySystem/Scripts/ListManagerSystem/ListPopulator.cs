using LootLocker.InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LootLocker.InventorySystem
{
    public abstract class ListPopulator : MonoBehaviour
    {
        [SerializeField, Tooltip("Useful for automatically removing placeholder entries.")]
        bool clearOnAwake;
        public IReadOnlyCollection<ListItem> Items => _items;
        readonly List<ListItem> _items = new List<ListItem>();
        public event System.Action<int> OnItemCountChanged;


        protected virtual void Awake()
        {
            if (clearOnAwake)
            {
                ClearList();
            }
        }


        public virtual void PopulateList(IReadOnlyCollection<IListData> data, bool clearList = true)
        {
            RemoveDestroyedItems();

            IReadOnlyCollection<IListData> newData;
            if (clearList)
            {
                ClearList();
                newData = data.ToArray();   // If we're clearing the list first, there's no need to filter it for new items - everything is new.
            }
            else
            {
                newData = GetNewDataOnly(data);
            }

            AddNewData(newData);
        }


        protected void AddItem(ListItem item)
        {
            _items.Add(item);
            OnItemCountChanged?.Invoke(_items.Count);
        }

        protected void RemoveItem(ListItem item)
        {
            _items.Remove(item);
            OnItemCountChanged?.Invoke(_items.Count);
        }


        protected abstract void AddNewData(IReadOnlyCollection<IListData> data);
        protected virtual void OnClearList() { }


        void RemoveDestroyedItems()
        {
            _items.RemoveAll(item => item == null);
        }

        IReadOnlyCollection<IListData> GetNewDataOnly(IReadOnlyCollection<IListData> data)
        {
            if (_items.Count > 0)
            {
                return data.Where(newData => !_items.Any(existingItem => existingItem.listData.Equals(newData))).ToArray();
            }
            else
            {
                return data.ToArray();
            }
        }

        void ClearList()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                ListItem item = _items[i];

                RemoveItem(item);
                Destroy(item.gameObject);
            }

            OnClearList();  // OnClearList is intentionally a separate method instead of making ClearList virtual. That way, the inheritor can't forget to call base.ClearList().
        }
    }

    public interface IListData : System.IEquatable<IListData>
    {
        string name { get; }
    }

    public interface IListFriendlyName 
    {
        string friendlyName { get; }
    }

    public interface IServerErrorHandler 
    {
        Action onServerSyncFailed { get; set; }
    }

    public interface ISyncToServerControl
    {
        bool syncToServer { get; set; }
    }
}