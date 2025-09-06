using System;
using System.Collections.Generic;

namespace Toolkit.Inventory {
    public interface IInventory {
        IContainer[] Containers { get; }

        int ContainerCount { get; }
        IContainer GetContainer(int index);
        T GetContainer<T>(int index) where T : IContainer;

        T FindItem<T>(Func<T, bool> search) where T : IItem;
        T[] FindItems<T>(Func<T, bool> search) where T : IItem;
    }
}
