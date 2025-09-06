using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [System.Serializable]
    public class Container : IContainer {

        [System.Serializable]
        public struct Priority {
            public ItemType Type;
            public int Value;
        }

        #region Variables

        [SerializeField] private string name;
        [SerializeField] private int basePriority = 0;
        [SerializeField, Min(0)] private int capacity = 0;
        [SerializeField] private List<Priority> priorities = new List<Priority>();
        [SerializeField] private List<ItemType> filter = new List<ItemType>();
        private List<Item> items = new List<Item>();

        #endregion

        #region Properties

        public string Name {
            get => name;
            set => name = value;
        }

        public int Capacity {
            get => capacity;
            set => capacity = value;
        }

        public Item this[int index] {
            get => items[index];
            set => items[index] = value;
        }

        public int Count => items.Count;
        public IReadOnlyList<Item> Items => items;

        #endregion

        #region Constructor

        public Container() { capacity = 9999; }

        public Container(int capacity) {
            this.name = string.Empty;
            this.capacity = capacity;
        }

        public Container(string name, int capacity) {
            this.name = name;
            this.capacity = capacity;
        }

        #endregion

        #region IContainer Impl

        public bool CanAdd(Item item) {
            if(filter == null)
                return true;
            if(filter.Count == 0)
                return true;

            // Check for filter
            var type = item.TryGetData(out IItemType itype) ? itype.Type : ItemType.None;
            for(int i = filter.Count - 1; i >= 0; i--)
                if(filter[i] == type)
                    return true;

            return false;
        }

        public bool Add(Item item, int index) {
            if(!CanAdd(item))
                return false;
            if(index < 0 && item.IsStackable(out IStackable stackable)) // Fill stacks first
                FillStackables(item, stackable);

            if(items.Count >= capacity)
                return false;
            if(index < 0 || index > items.Count)
                items.Add(item);
            else
                items.Insert(index, item);
            return true;
        }

        private void FillStackables(Item item, IStackable stackable) {
            foreach(var i in items) {
                if(i.UID != item.UID)
                    continue;
                if(i.TryGetData<IStackable>(out var data))
                    data.Fill(stackable);
                if(stackable.Count == 0)
                    return;
            }
        }

        public Item GetItem(int index) {
            return items[index];
        }

        public int InputPriority(Item item) {
            if(priorities == null || priorities.Count == 0)
                return basePriority;
            if(!item.TryGetData<IItemType>(out var type))
                return basePriority;
            foreach(var prio in priorities)
                if(prio.Type == type.Type)
                    return prio.Value;
            return basePriority;
        }

        public bool Move(Item item, int index) {
            if(!this.TryGetIndexOf(item, out int currentIndex))
                return false;
            return items.MoveTo(currentIndex, index);
        }

        public bool Remove(int index) {
            if(index < 0)
                return false;
            if(index >= items.Count)
                return false;
            items.RemoveAt(index);
            return true;
        }

        public void Sort(Comparison<Item> sortFunction) {
            items.Sort(sortFunction);
        }

        public void UpdateContainer(float dt) {

        }

        #endregion
    }
}
