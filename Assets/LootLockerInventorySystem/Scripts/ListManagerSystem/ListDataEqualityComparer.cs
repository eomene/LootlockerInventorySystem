using System.Collections.Generic;
namespace LootLocker.InventorySystem
{
    public class ListDataEqualityComparer : IEqualityComparer<IListData>
    {
        public bool Equals(IListData x, IListData y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IListData obj)
        {
            // We can't know how the implementation defines equality, so we make so that the hash code is always the same to avoid breaking any comparisons.
            // If performance becomes an issue, this would have to be reconsidered.
            return 0;
        }
    }
}