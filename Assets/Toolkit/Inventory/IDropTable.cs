using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.Inventory
{
    public interface IDropTable
    {
        string Name { get; }
        string Description { get; }
        IDropData[] GetDrop(IUnit source);
        IDropData[][] GetDropAdvanced(IUnit source);
    }

    public interface IDropData
    {
        IItem Drop { get; }
        bool IsStackable { get; }
        int GetAmount(IUnit source);
    }
}
