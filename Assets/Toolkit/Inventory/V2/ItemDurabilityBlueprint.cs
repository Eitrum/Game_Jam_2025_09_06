using System.Collections;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;


namespace Toolkit.Inventory.V2 {
    //[CreateAssetMenu(menuName = "Toolkit/Inventory/Item Data/Durability")]
    [Toolkit.NSOFile("Metadata/Durability", typeof(IItemDataBlueprint))]
    public class ItemDurabilityBlueprint : ScriptableObject, IItemDataBlueprint {

        [SerializeField] private float value;

        public System.Type GetItemType() => typeof(ItemDurability);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            data = new ItemDurability(value);
            return true;
        }
    }

    public class ItemDurability : BaseItemData {

        public float Value { get; private set; }

        public ItemDurability() { }

        public ItemDurability(float durability) {
            this.Value = durability;
        }

        public bool Use(float amount) {
            this.Value = Mathf.Max(this.Value - amount, 0);
            return this.Value > Mathf.Epsilon;
        }

        public override void Serialize(TMLNode node) {
            node.AddProperty("durability", Value);
        }
        public override void Deserialize(TMLNode node) {
            Value = node.GetFloat("durability", 1f);
        }
    }
}
