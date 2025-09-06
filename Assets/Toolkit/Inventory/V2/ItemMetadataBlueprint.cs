using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    //[CreateAssetMenu(menuName = "Toolkit/Inventory/Item Data/Metadata")]
    [Toolkit.NSOFile("Metadata/Default", "Metadata", typeof(IItemDataBlueprint))]
    public class ItemMetadataBlueprint : ScriptableObject, IItemDataBlueprint {

        [SerializeField] private string itemName;
        [SerializeField, TextArea(2, 8)] private string flavorText;
        [SerializeField] private ItemType itemType;

        public System.Type GetItemType() => typeof(ItemMetadata);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            var meta = new ItemMetadata(itemName, flavorText, itemType);
            data = meta;
            return true;
        }
    }

    public class ItemMetadata : BaseItemData {

        public string Name { get; private set; }
        public string FlavorText { get; private set; }
        public ItemType ItemType { get; private set; }

        public ItemMetadata() { }

        public ItemMetadata(string name, string flavorText, ItemType itemType) {
            Name = name;
            FlavorText = flavorText;
            ItemType = itemType;
        }

        public override void Serialize(TMLNode node) {
            node.AddProperty("name", Name);
            node.AddProperty("flavor", FlavorText);
            node.AddProperty("type", ItemType);
        }
        public override void Deserialize(TMLNode node) {
            Name = node.GetString("name", string.Empty);
            FlavorText = node.GetString("flavor", FlavorText);
            ItemType = node.GetEnum<ItemType>("type", ItemType.None);
        }
    }
}
