using System;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public static class ContainerExtensions {

        #region Variables

        private const string TAG = "[Toolkit.Inventory.ContainerExtensions] - ";

        #endregion

        #region Add

        public static bool Add(this IContainer container, Item item) {
            return container.Add(item, -1);
        }

        public static bool AddBefore(this IContainer container, Item item, Item target) {
            if(!container.TryGetIndexOf(target, out var index))
                return Add(container, item);
            return container.Add(item, index);
        }

        public static bool AddAfter(this IContainer container, Item item, Item target) {
            if(!container.TryGetIndexOf(target, out var index))
                return Add(container, item);
            return container.Add(item, index + 1);
        }

        #endregion

        #region Remove

        public static bool Remove(this IContainer container, Item item) {
            if(!container.TryGetIndexOf(item, out int index))
                return false;
            return container.Remove(index);
        }

        public static bool Remove(this IContainer container, int uid, ref int amount) {
            for(int i = container.Count - 1; i >= 0; i--) {
                var item = container[i];
                if(item.UID != uid)
                    continue;
                if(item.IsStackable(out var stackable)) {
                    if(stackable.Remove(ref amount) && stackable.Count == 0)
                        container.Remove(i);
                }
                else {
                    container.Remove(i);
                    amount--;
                }
            }
            return amount == 0;
        }

        #endregion

        #region Clear

        public static void Clear(this IContainer container) {
            for(int i = container.Count - 1; i >= 0; i--)
                container.Remove(i);
        }

        public static void Extract(this IContainer container, List<Item> items) {
            foreach(var c in container.Items)
                if(c != null)
                    items.Add(c);
            for(int i = container.Count - 1; i >= 0; i--)
                container.Remove(i);
        }

        #endregion

        #region IndexOf

        public static int IndexOf(this IContainer container, Item item)
            => TryGetIndexOf(container, item, out int index) ? index : -1;

        public static bool TryGetIndexOf(this IContainer container, Item item, out int index) {
            for(int i = 0, len = container.Count; i < len; i++) {
                if(container[i] == item) {
                    index = i;
                    return true;
                }
            }
            index = default;
            return false;
        }

        #endregion

        #region Swap

        public static bool Swap(this IContainer container, int index0, int index1) {
            if(index0 == index1)
                return false;
            var item0 = container.Items[index0];
            var item1 = container.Items[index1];
            return Swap(container, item0, index0, item1, index1);
        }

        public static bool Swap(this IContainer container, Item item0, Item item1) {
            if(item0 == item1)
                return false;
            if(!container.TryGetIndexOf(item0, out int index0))
                return false;
            if(!container.TryGetIndexOf(item1, out int index1))
                return false;
            return Swap(container, item0, index0, item1, index1);
        }

        private static bool Swap(this IContainer container, Item item0, int index0, Item item1, int index1) {
            if(index0 == index1)
                return false;

            if(index0 < index1) {
                if(!container.Move(item0, index1))
                    return false;
                if(!container.Move(item1, index0)) {
                    Debug.LogError(TAG + $"Failed to move the second item during a swap operation");
                    return false;
                }
                return true;
            }
            else {
                if(!container.Move(item1, index0))
                    return false;
                if(!container.Move(item0, index1)) {
                    Debug.LogError(TAG + $"Failed to move the second item during a swap operation");
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region Move

        public static bool MoveBefore(this IContainer container, Item item, Item otherItem) {
            if(!container.TryGetIndexOf(otherItem, out int index))
                return false;

            return container.Move(item, index);
        }

        public static bool MoveAfter(this IContainer container, Item item, Item otherItem) {
            if(!container.TryGetIndexOf(otherItem, out int index))
                return false;

            return container.Move(item, index + 1);
        }

        #endregion

        #region Find Item

        public static bool TryFindItem(this IContainer container, Func<Item, bool> search, out Item result) {
            foreach(var i in container.Items) {
                if(search(i)) {
                    result = i;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static int TryFindItems(this IContainer container, Func<Item, bool> search, Item[] results) {
            int index = 0;
            return TryFindItems(container, search, results, ref index);
        }

        public static int TryFindItems(this IContainer container, Func<Item, bool> search, Item[] results, ref int index) {
            int count = 0;
            foreach(var i in container.Items) {
                if(search(i)) {
                    count++;
                    results[index++] = i;
                    if(index > results.Length)
                        break;
                }
            }
            return count;
        }

        public static void TryFindItems(this IContainer container, Func<Item, bool> search, List<Item> results)
           => TryFindItems(container, search, results, true);

        public static void TryFindItems(this IContainer container, Func<Item, bool> search, List<Item> results, bool clear) {
            if(clear)
                results.Clear();
            foreach(var i in container.Items) {
                if(search(i))
                    results.Add(i);
            }
        }

        #endregion

        #region Weight

        public static float CalculateWeight(this IContainer container) {
            float weight = 0f;
            foreach(var item in container.Items)
                weight += ItemCalculations.CalculateWeight(item);
            return weight;
        }

        #endregion
    }
}
