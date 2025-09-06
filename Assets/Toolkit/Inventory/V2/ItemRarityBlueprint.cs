using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [Toolkit.NSOFile("Metadata/Rarity", typeof(IItemDataBlueprint))]
    public class ItemRarityBlueprint : ScriptableObject, IItemDataBlueprint {

        [SerializeField] private Rarity value;

        public System.Type GetItemType() => typeof(ItemRarity);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            data = new ItemRarity(value);
            return true;
        }
    }

    public class ItemRarity : BaseItemData {

        public Rarity Value { get; private set; }

        public ItemRarity() { }

        public ItemRarity(Rarity rarity) {
            this.Value = rarity;
        }

        public override void Serialize(TMLNode node) {
            node.AddProperty("rarity", Value);
        }
        public override void Deserialize(TMLNode node) {
            Value = node.GetEnum("rarity", Rarity.None);
        }
    }
}
