using System;
using Toolkit.IO;
using Toolkit.Unit;

namespace Toolkit.Inventory.V2 {
    public interface IItemAbility : IItemData {
        // IAbility Ability { get; }
    }

    public interface IItemUse : IItemData {
        string DisplayOption { get; }
        bool CanUse(IUnit unit);
        void Use(IUnit unit);
    }

    public class ItemAbility : BaseItemData, IItemUse {

        public string Display = "Cast";
        public float DurabilityCost;
        public UnityEngine.Object Ability;

        public string DisplayOption => Display;

        public bool CanUse(IUnit unit) {
            return true;
        }

        public void Use(IUnit unit) {
            
        }

        public override void Serialize(TMLNode node) {

        }

        public override void Deserialize(TMLNode node) {

        }
    }
}
