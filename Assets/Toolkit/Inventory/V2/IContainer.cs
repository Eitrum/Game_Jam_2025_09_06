

using System;
using System.Collections.Generic;

namespace Toolkit.Inventory.V2 {
    public interface IContainer {
        int Count { get; }
        Item this[int index] { get; set; }
        IReadOnlyList<Item> Items { get; }

        Item GetItem(int index);

        bool Add(Item item, int index);
        bool Remove(int index);
        bool Move(Item item, int index);

        int InputPriority(Item item);

        void UpdateContainer(float dt);
        void Sort(Comparison<Item> sortFunction);
    }

    public interface IGridContainer : IContainer {
        Item GetItem(int x, int y);
    }
}
