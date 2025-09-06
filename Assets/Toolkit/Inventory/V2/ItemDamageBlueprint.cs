using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [Toolkit.NSOFile("Combat/Base Damage", typeof(IItemDataBlueprint))]
    public class ItemDamageBlueprint : ScriptableObject, IItemDataBlueprint {

        #region Variables

        [SerializeField] private Health.DamageRange[] values;

        #endregion

        #region Properties

        public IReadOnlyList<Health.DamageRange> Values => values;

        #endregion

        #region IItemDataBlueprint Impl

        public System.Type GetItemType() => typeof(ItemDamage);

        public bool TryCreate(System.Random random, int level, out IItemData data) {
            data = new ItemDamage(values);
            return true;
        }

        #endregion
    }

    public class ItemDamage : BaseItemData {

        private Health.DamageRange[] values;
        public IReadOnlyList<Health.DamageRange> Values => values;

        public ItemDamage() { }

        public ItemDamage(Health.DamageRange[] damageValues) {
            this.values = damageValues;
        }

        public override void Serialize(TMLNode node) {
            node.AddProperty("damageRanges", values.Select(x => x.Range.ToVector2()));
            node.AddProperty("damageTypes", values.Select(x => x.DamageTypeMask));
        }
        public override void Deserialize(TMLNode node) {
            var ranges = node.GetVector2s("damageRanges");
            var damageTypes = node.GetInts("damageTypes");
            values = new Health.DamageRange[damageTypes.Count];

            for(int i = 0, length = damageTypes.Count; i < length; i++)
                values[i] = new Health.DamageRange(new MinMax(ranges[i].x, ranges[i].y), damageTypes[i]);
        }
    }
}
