using Toolkit.IO;

namespace Toolkit.Inventory.V2 {
    public abstract class BaseItemData : IItemData {
        public virtual Item Parent { get; set; }
        public abstract void Deserialize(TMLNode node);
        public abstract void Serialize(TMLNode node);
    }
}
