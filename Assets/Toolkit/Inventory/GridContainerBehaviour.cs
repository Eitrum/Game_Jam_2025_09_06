using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Container (Grid)")]
    public class GridContainerBehaviour : MonoBehaviour, IGridContainer, IContainerEvents
    {
        [SerializeField, Min(1)] private int width = 10;
        [SerializeField, Min(1)] private int height = 6;

        private List<IItem> items = new List<IItem>();
        private List<GridRect> itemArea = new List<GridRect>();

        public event OnContainerUpdateCallback OnContainerUpdate;
        public event OnItemAddedCallback OnItemAdded;
        public event OnItemRemovedCallback OnItemRemoved;

        public IReadOnlyList<GridRect> Areas => itemArea;
        public IReadOnlyList<IItem> Items => items;
        public int Count => items.Count;
        public int Width => width;
        public int Height => height;

        public bool IsInside(GridRect rect) {
            return
                rect.position.x >= 0 &&
                rect.position.y >= 0 &&
                rect.position.x + rect.size.x <= width &&
                rect.position.y + rect.size.y <= height;
        }

        public bool AddItemAtLocation(IItem item, Vector2Int location) {
            GridRect itemBound = new GridRect(location, Vector2Int.one);
            if(item is ISize size) {
                itemBound.size = size.GridSize;
            }

            if(!IsInside(itemBound))
                return false;

            if(HasItemInArea(itemBound, out IItem otherItem)) {
                if(item is IStackable stackable && otherItem.IsEqual(item) && otherItem is IStackable otherStackable) {
                    var slots = otherStackable.MaxStackSize - otherStackable.CurrentStackSize;
                    var toAdd = Mathf.Min(stackable.CurrentStackSize, slots);
                    if(otherStackable.Add(toAdd)) {
                        stackable.Remove(toAdd);
                        if(stackable.CurrentStackSize <= 0) {
                            item.DestroyItem();
                            return true;
                        }
                        OnContainerUpdate?.Invoke();
                    }
                }
                return false;
            }

            items.Add(item);
            itemArea.Add(itemBound);
            OnItemAdded?.Invoke(item);
            OnContainerUpdate?.Invoke();
            return true;
        }

        public bool RemoveItemAtLocation(Vector2Int location, out IItem item) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(itemArea[i].Contains(location)) {
                    item = items[i];
                    items.RemoveAt(i);
                    itemArea.RemoveAt(i);
                    OnContainerUpdate?.Invoke();
                    OnItemRemoved?.Invoke(item);
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool HasItemInArea(GridRect area, out IItem item) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(itemArea[i].Intersects(area)) {
                    item = items[i];
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool GetArea(IItem item, out GridRect rect) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(items[i] == item) {
                    rect = itemArea[i];
                    return true;
                }
            }

            rect = new GridRect();
            return false;
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

        public bool AddItem(IItem item) {
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

            GridHelper helper = new GridHelper(width, height);
            helper.SetOccupied(itemArea);
            if(item is ISize size) {
                if(helper.HasAvailableLocation(new GridRect(Vector2Int.zero, size.GridSize), out GridRect __sizeArea)) {
                    items.Add(item);
                    itemArea.Add(__sizeArea);
                    OnItemAdded?.Invoke(item);
                    OnContainerUpdate?.Invoke();
                    return true;
                }
            }
            else {
                if(helper.HasAvailableLocation(new GridRect(Vector2Int.zero, Vector2Int.one), out GridRect _area)) {
                    items.Add(item);
                    itemArea.Add(_area);
                    OnItemAdded?.Invoke(item);
                    OnContainerUpdate?.Invoke();
                    return true;
                }
            }
            Debug.Log("Failed to add");
            return false;
        }

        public bool RemoveItem(IItem item) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(items[i] == item) {
                    items.RemoveAt(i);
                    itemArea.RemoveAt(i);
                    OnItemRemoved?.Invoke(item);
                    OnContainerUpdate?.Invoke();
                    return true;
                }
            }
            return false;
        }

        public bool CanRemoveItem(IItem item) {
            return items.Contains(item);
        }

        public bool CanAddItem(IItem item) {
            GridHelper helper = new GridHelper(width, height);
            helper.SetOccupied(itemArea);
            if(item is ISize size)
                return helper.HasAvailableLocation(new GridRect(Vector2Int.zero, size.GridSize), out GridRect __);
            return helper.HasAvailableLocation(new GridRect(Vector2Int.zero, Vector2Int.one), out GridRect _);
        }

        public void Sort() {
            Debug.LogWarning("Sorting a grid container is not supported");
        }

        public void Sort(Comparison<IItem> sortFunction) {
            Debug.LogWarning("Sorting a grid container is not supported");
        }

        public void UpdateContainer(float dt) {
            for(int i = 0, length = items.Count; i < length; i++) {
                if(items[i] is IUpdateable update)
                    update.UpdateItem(dt);
            }
            OnContainerUpdate?.Invoke();
        }
    }
}
