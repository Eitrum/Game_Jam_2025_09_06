using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [Toolkit.NSOFile("Metadata/Quality", typeof(IItemDataBlueprint))]
    public class ItemQualityBlueprint : ScriptableObject, IItemDataBlueprint {

        [SerializeField] private Quality value;

        public System.Type GetItemType() => typeof(ItemQuality);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            data = new ItemQuality(value);
            return true;
        }
    }

    public class ItemQuality : BaseItemData {

        public Quality Value { get; private set; }

        public ItemQuality() { }

        public ItemQuality(Quality quality) {
            this.Value = quality;
        }

        public override void Serialize(TMLNode node) {
            node.AddProperty("quality", Value);
        }
        public override void Deserialize(TMLNode node) {
            Value = node.GetEnum("quality", Quality.None);
        }
    }
}
