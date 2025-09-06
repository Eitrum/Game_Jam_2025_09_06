using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public interface IStackable : IItemData {
        int Count { get; }
        int Size { get; }

        bool Add(int amount);
        bool Remove(int amount);
    }

    public class Stackable : BaseItemData, IStackable {

        #region Variables

        public int Count { get; set; } = 1;
        public int Size { get; set; } = 1;

        #endregion

        #region Constructor

        public Stackable() {

        }

        public Stackable(int count, int size) {
            Count = count;
            Size = size;
        }

        #endregion

        #region Add / Remove

        public bool Add(int amount) {
            if(amount + Count > Size)
                return false;
            Count += amount;
            return true;
        }

        public bool Remove(int amount) {
            if(Count < amount)
                return false;
            Count -= amount;
            return true;
        }

        #endregion

        #region Serialization

        public override void Serialize(TMLNode node) {
            node.AddProperty("count", Count);
            node.AddProperty("size", Size);
        }

        public override void Deserialize(TMLNode node) {
            Count = node.GetInt("count", Count);
            Size = node.GetInt("size", Size);
        }

        #endregion
    }

    public static class StackableExtensions {

        private const string TAG = "[Toolkit.Inventory.StackableExtensions] - ";

        #region Util

        public static int GetSpace(this IStackable stackable) {
            return stackable.Size - stackable.Count;
        }

        #endregion

        #region Fill

        public static bool Fill(this IStackable stackable, ref int count) {
            var space = stackable.GetSpace();
            if(space <= 0)
                return false;

            var toAdd = Mathf.Min(space, count);
            if(!stackable.Add(toAdd))
                return false;

            count -= toAdd;
            return true;
        }

        public static bool Fill(this IStackable stackable, IStackable takeFromOther) {
            var count = takeFromOther.Count;
            var ogStack = stackable.Count;
            if(!Fill(stackable, ref count))
                return false;

            if(!takeFromOther.Set(count)) {
                // This not supposed to happen
                Debug.LogWarning(TAG + $"Fill operation could not set the second item to '{count}'. Duplicated item stack! Attempting to reverse operation!");
                if(!stackable.Set(ogStack)) {
                    Debug.LogError(TAG + "Fill operation failed to reverse operation! Duplication might have occurred");
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Remove

        public static bool Remove(this IStackable stackable, ref int count) {
            if(count == 0)
                return false;
            var toRemove = Mathf.Min(stackable.Count, count);
            if(toRemove == 0)
                return false;
            if(!stackable.Remove(toRemove)) {
                return false;
            }
            count -= toRemove;
            return true;
        }

        #endregion

        #region Set

        public static bool SetStack(this Item item, int count) {
            if(!item.TryGetData(out IStackable stackable))
                return false;
            return Set(stackable, count);
        }

        public static bool Set(this IStackable stackable, int count) {
            if(count > stackable.Size)
                return false;
            if(count < 0)
                return false;

            var diff = count - stackable.Count;
            if(diff == 0)
                return true;

            return (diff > 0 ? stackable.Add(diff) : stackable.Remove(-diff));
        }

        #endregion

        #region Is Stackable

        public static bool IsStackable(this Item item) {
            return item.TryGetData(out IStackable stackable);
        }

        public static bool IsStackable(this Item item, out IStackable stackable) {
            return item.TryGetData(out stackable);
        }

        #endregion

        #region Split

        public static bool Split(this Item inputItem, int count, out Item item) {
            return Split(inputItem, count, System.Guid.NewGuid(), out item);
        }

        public static bool Split(this Item inputItem, int count, System.Guid newStackGuid, out Item item) {
            if(!IsStackable(inputItem, out var stackable)) {
                item = null;
                return false;
            }
            if(count <= 0) {
                item = null;
                return false;
            }
            if(stackable.Count >= count) {
                item = null;
                return false;
            }
            if(!Clone(inputItem, newStackGuid, out item)) {
                return false;
            }
            if(!IsStackable(item, out var stackable2)) {
                return false;
            }

            if(!stackable2.Set(count))
                return false;

            if(!stackable.Remove(count))
                return false;

            if(inputItem.TryGetData(out IDurability durability) && !durability.RestoreFull()) {
                Debug.LogWarning(TAG + "Failed to restore durability after splitting item");
            }

            return true;
        }

        #endregion

        #region Clone // TODO Fix lazy copy

        private static bool Clone(this Item item, out Item cloned) {
            try {
                var node = item.GetTMLNode();
                cloned = new Item(item.UID);
                cloned.Deserialize(node, true);
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                cloned = null;
                return false;
            }
        }

        private static bool Clone(this Item item, System.Guid newCloneGuid, out Item cloned) {
            try {
                var node = item.GetTMLNode();
                cloned = new Item(item.UID, newCloneGuid);
                cloned.Deserialize(node, true);
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                cloned = null;
                return false;
            }
        }

        #endregion

        #region Formatted

        public static string Formatted(this IStackable stackable) {
            if(stackable == null)
                return $"1/1";
            return $"{stackable.Count}/{stackable.Size}";
        }

        #endregion
    }

    public class inventoryTest {

        public void Add(Item item) {
            if(item.IsStackable(out var stackable)) {

            }
        }
    }
}
