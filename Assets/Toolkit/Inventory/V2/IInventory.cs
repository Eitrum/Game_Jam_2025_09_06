using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public interface IInventory {
        int ContainerCount { get; }
        IContainer this[int index] { get; }
        IReadOnlyList<IContainer> Containers { get; }

        bool TryAdd(IContainer container);
        bool TryRemove(IContainer container);
    }
}
