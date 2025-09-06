using System;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public static class InventoryExtensions {

        #region Variables

        private const string TAG = "[Toolkit.Inventory.InventoryExtensions] - ";
        private static FastPool<List<Item>> pool = new FastPool<List<Item>>();
        private static FastPool<List<(int, IContainer)>> priorityPool = new FastPool<List<(int, IContainer)>>();

        #endregion

        #region Init

        static InventoryExtensions() {
            pool.ClearOnReset();
            priorityPool.ClearOnReset();
        }

        #endregion

        #region TryGetContainer

        public static bool TryGetContainer(this IInventory inventory, int index, out IContainer container) {
            container = default;
            if(inventory == null)
                return false;
            if(index < 0)
                return false;
            if(index >= inventory.ContainerCount)
                return false;
            container = inventory[index];
            return container != null;
        }

        public static bool TryGetContainer<T>(this IInventory inventory, int index, out T container) where T : IContainer {
            container = default;
            if(inventory == null)
                return false;
            if(index < 0)
                return false;
            if(index >= inventory.ContainerCount)
                return false;
            if(inventory[index] is T con) {
                container = con;
                return true;
            }
            return false;
        }

        public static bool TryGetContainer<T>(this IInventory inventory, out T container) where T : IContainer {
            for(int i = 0, len = inventory.ContainerCount; i < len; i++) {
                if(inventory[i] is T con) {
                    container = con;
                    return true;
                }
            }
            container = default;
            return false;
        }

        #endregion

        #region Update

        /// <summary>
        /// For manual inventory updates
        /// </summary>
        public static void UpdateInventory(this IInventory inventory, float dt) {
            foreach(var c in inventory.Containers)
                c.UpdateContainer(dt);
        }

        #endregion

        #region Add

        public static bool AddItem(this IInventory inventory, Item item)
            => AddItem(inventory, item, false);

        public static bool AddItem(this IInventory inventory, Item item, bool ignorePriority) {
            if(inventory.ContainerCount == 0) {
                Debug.LogError(TAG + $"No container found!");
                return false;
            }

            if(item.IsStackable(out IStackable stackable)) {
                // Find existing stackable item
                var list = pool.Get();
                TryFindItemsByUID(inventory, item.UID, list);

                foreach(var i in list) {
                    if(stackable.Count <= 0)
                        break;
                    if(i.IsStackable(out IStackable otherStackable))
                        otherStackable.Fill(stackable);
                }

                pool.Return(list);

                if(stackable.Count == 0)
                    return true;
            }

            // Add by priority
            if(!ignorePriority) {
                var priolist = priorityPool.Get();
                foreach(var c in inventory.Containers) {
                    var prio = c.InputPriority(item);
                    priolist.Add((prio, c));
                }
                priolist.Sort(Sort.ByLargest);
                foreach(var c in priolist) {
                    if(c.Item2.Add(item))
                        return true;
                }
                priorityPool.Return(priolist);
                return false;
            }

            // Add item first possible container
            foreach(var c in inventory.Containers) {
                if(c.Add(item))
                    return true;
            }

            return false;
        }

        #endregion

        #region Remove

        public static bool Remove(this IInventory inventory, Item item) {
            for(int i = inventory.ContainerCount - 1; i >= 0; i--) {
                if(inventory.Containers[i].Remove(item)) {
                    return true;
                }
            }
            return false;
        }

        public static bool Remove(this IInventory inventory, int uid, int amount) {
            for(int i = inventory.ContainerCount - 1; i >= 0; i--) {
                var con = inventory.Containers[i];
                for(int j = con.Count - 1; j >= 0; j--) {
                    con.Remove(uid, ref amount);
                    if(amount == 0)
                        return true;
                }
            }
            return false;
        }

        public static bool Remove(this IInventory inventory, ItemBlueprint blueprint, int amount) {
            return Remove(inventory, blueprint.UID_Hashed, amount);
        }

        public static bool Remove(this IInventory inventory, string blueprintUID, int amount) {
            return Remove(inventory, ItemUIDDatabase.NameToId(blueprintUID), amount);
        }

        #endregion

        #region Clear

        public static void ClearAll(this IInventory inventory) {
            for(int i = inventory.ContainerCount - 1; i >= 0; i--)
                inventory[i].Clear();
        }

        public static void ClearAll(this IInventory inventory, out Item[] allItems) {
            allItems = inventory.GetAllItems().ToArray();
            for(int i = inventory.ContainerCount - 1; i >= 0; i--)
                inventory[i].Clear();
        }

        #endregion

        #region FindItem

        public static bool TryFindItem(this IInventory inventory, Func<Item, bool> search, out Item item) {
            foreach(var c in inventory.Containers) {
                if(c?.TryFindItem(search, out item) ?? false)
                    return true;
            }

            item = default;
            return false;
        }

        public static bool TryFindItem(this IInventory inventory, Func<Item, bool> search, out IContainer container, out Item item) {
            foreach(var c in inventory.Containers) {
                if(c?.TryFindItem(search, out item) ?? false) {
                    container = c;
                    return true;
                }
            }

            container = default;
            item = default;
            return false;
        }

        #endregion

        #region TryFindItems

        public static int TryFindItems(this IInventory inventory, Func<Item, bool> search, Item[] items) {
            int index = 0;
            return TryFindItems(inventory, search, items, ref index);
        }

        public static int TryFindItems(this IInventory inventory, Func<Item, bool> search, Item[] items, ref int index) {
            int start = index;
            foreach(var c in inventory.Containers)
                c.TryFindItems(search, items, ref index);
            return index - start;
        }

        public static void TryFindItems(this IInventory inventory, Func<Item, bool> search, List<Item> results)
            => TryFindItems(inventory, search, results, true);

        public static void TryFindItems(this IInventory inventory, Func<Item, bool> search, List<Item> results, bool clear) {
            if(clear)
                results.Clear();
            foreach(var c in inventory.Containers)
                c.TryFindItems(search, results, false);
        }

        #endregion

        #region FindItemByUID

        public static bool TryFindItemByUID(this IInventory inventory, string uidName, out Item item) {
            return TryFindItemByUID(inventory, ItemUIDDatabase.NameToId(uidName), out item);
        }

        public static bool TryFindItemByUID(this IInventory inventory, int uid, out Item item) {
            return TryFindItem(inventory, (item) => item.UID == uid, out item);
        }

        // Multiple List

        public static void TryFindItemsByUID(this IInventory inventory, string uidName, List<Item> items)
            => TryFindItemsByUID(inventory, ItemUIDDatabase.NameToId(uidName), items);

        public static void TryFindItemsByUID(this IInventory inventory, int uid, List<Item> items) {
            TryFindItems(inventory, (item) => item.UID == uid, items);
        }

        public static void TryFindItemsByUID(this IInventory inventory, string uidName, List<Item> items, bool clear)
            => TryFindItemsByUID(inventory, ItemUIDDatabase.NameToId(uidName), items, clear);

        public static void TryFindItemsByUID(this IInventory inventory, int uid, List<Item> items, bool clear) {
            TryFindItems(inventory, (item) => item.UID == uid, items, clear);
        }

        // Multiple Array

        public static int TryFindItemsByUID(this IInventory inventory, string uidName, Item[] items)
            => TryFindItemsByUID(inventory, ItemUIDDatabase.NameToId(uidName), items);

        public static int TryFindItemsByUID(this IInventory inventory, int uid, Item[] items) {
            return TryFindItems(inventory, (item) => item.UID == uid, items);
        }

        public static int TryFindItemsByUID(this IInventory inventory, string uidName, Item[] items, ref int index)
            => TryFindItemsByUID(inventory, ItemUIDDatabase.NameToId(uidName), items, ref index);

        public static int TryFindItemsByUID(this IInventory inventory, int uid, Item[] items, ref int index) {
            return TryFindItems(inventory, (item) => item.UID == uid, items, ref index);
        }

        #endregion

        #region Weight

        public static float CalculateWeight(this IInventory inventory) {
            float weight = 0f;
            foreach(var c in inventory.Containers)
                weight += c?.CalculateWeight() ?? 0f;
            return weight;
        }

        #endregion

        #region Sort

        public static void SortContainers(this IInventory inventory, Comparison<Item> sortFunction) {
            foreach(var c in inventory.Containers)
                c?.Sort(sortFunction);
        }

        #endregion

        #region GetAll

        public static IEnumerable<Item> GetAllItems(this IInventory inventory) {
            foreach(var con in inventory.Containers)
                foreach(var i in con.Items)
                    yield return i;
        }

        #endregion

        #region Serialize

        public static TMLNode Serialize(this IInventory inventory) {
            TMLNode node = new TMLNode("inventory");
            int containerIndex = 0;
            foreach(var c in inventory.Containers) {
                var connode = node.AddChild($"container {containerIndex++}");
                foreach(var i in c.Items)
                    i.Serialize(connode);
            }
            return node;
        }

        public static void Serialize(this IInventory inventory, TMLNode parent) {
            parent.AddNode(Serialize(inventory));
        }

        #endregion
    }
}
