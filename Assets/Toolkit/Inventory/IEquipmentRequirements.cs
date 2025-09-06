using System;
using Toolkit.Unit;

namespace Toolkit.Inventory
{
    public interface IEquipmentRequirements
    {
        IAttributeRequirements AttributeRequirements { get; }
    }
}
