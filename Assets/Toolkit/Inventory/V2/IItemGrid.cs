using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public interface IItemGrid : IItemData {
        Vector2Int Size { get; set; }
    }
}
