using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Container (Simple)")]
    public class ContainerBehaviour : MonoBehaviour, IContainer, IContainerEvents
    {
        private List<IItem> items = new List<IItem>();

        public IReadOnlyList<IItem> Items => items;
        public int Count => items.Count;

        public event OnContainerUpdateCallback OnContainerUpdate;
        public event OnItemAddedCallback OnItemAdded;
        public event OnItemRemovedCallback OnItemRemoved;

        public bool AddItem(IItem item) {
            if(!CanAddItem(item))
                return false;

            if(item is IStackable stackable) {
                for(int i = 0, length = items.Count; i < length; i++) {
                    var tItem = items[i];
                    if(tItem.IsEqual(item) && tItem is IStackable tStackable) {
                        var slots = tStackable.MaxStackSize - tStackable.CurrentStackSize;
                        var toAdd = Mathf.Min(stackable.CurrentStackSize, slots);
                        if(tStackable.Add(toAdd)) {
                            stackable.Remove(toAdd);
                            if(stackable.CurrentStackSize <= 0) {
                                item.DestroyItem();
                                OnContainerUpdate?.Invoke();
                                return true;
                            }
                        }
                    }
                }
            }
            items.Add(item);
            OnItemAdded?.Invoke(item);
            OnContainerUpdate?.Invoke();
            return true;
        }

        public bool CanAddItem(IItem item) {
            return !items.Contains(item);
        }

        public IItem GetItem(int index) {
            if(index >= 0 && index < items.Count)
                return items[index];
            return null;
        }

        public T GetItem<T>(int index) where T : IItem {
            if(GetItem(index) is T tItem)
                return tItem;
            return default;
        }

        public int GetItems<T>(IList<T> otherItemArray) where T : IItem {
            if(otherItemArray.IsReadOnly) {
                int index = 0;
                int length = this.items.Count;
                int otherIndex = 0;
                int otherLength = otherItemArray.Count;
                while(index < length && otherIndex < otherLength) {
                    if(items[index++] is T tItem) {
                        otherItemArray[otherIndex++] = tItem;
                    }
                }
                return otherIndex;
            }
            else {
                otherItemArray.Clear();
                for(int i = 0, length = items.Count; i < length; i++) {
                    if(items[i] is T tItem)
                        otherItemArray.Add(tItem);
                }
                return otherItemArray.Count;
            }
        }

        public bool RemoveItem(IItem item) {
            var result = items.Remove(item);
            OnContainerUpdate?.Invoke();
            OnItemRemoved?.Invoke(item);
            return result;
        }

        public bool CanRemoveItem(IItem item) {
            return items.Contains(item);
        }

        public void Sort() {
            Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        public void Sort(Comparison<IItem> sortFunction) {
            Toolkit.Sort.Merge(items, sortFunction);
            OnContainerUpdate?.Invoke();
        }

        public void UpdateContainer(float dt) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(items[i] is IUpdateable update)
                    update.UpdateItem(dt);
            }
            OnContainerUpdate?.Invoke();
        }

        void OnDestroy() {
            for(int i = items.Count - 1; i >= 0; i--) {
                items[i].DestroyItem();
            }
        }
    }
}
