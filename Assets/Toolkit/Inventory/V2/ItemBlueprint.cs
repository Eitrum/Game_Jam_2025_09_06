using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    [CreateAssetMenu(menuName = "Toolkit/Inventory/Item Blueprint")]
    public class ItemBlueprint : ScriptableObject {

        #region Variables

        [SerializeField, EditLock] private string uid = "category/item";
        [SerializeField] private NSOReferenceArray<IItemDataBlueprint> dataBlueprints = new NSOReferenceArray<IItemDataBlueprint>();
        private int isStackable = 0;

        #endregion

        #region Properties

        public string UID => uid;
        public int UID_Hashed => ItemUIDDatabase.NameToId(UID);

        #endregion

        #region Lookups

        public bool TryGetItemDataBlueprint<TItemData, TItemDataBlueprint>(TItemData itemData, int hint, out TItemDataBlueprint itemDataBlueprint)
            where TItemData : IItemData
            where TItemDataBlueprint : IItemDataBlueprint {
            if(hint >= 0 && hint < dataBlueprints.Count && dataBlueprints[hint] is TItemDataBlueprint hintresult && hintresult.GetItemType() == typeof(TItemData)) {
                itemDataBlueprint = hintresult;
                return true;
            }

            for(int i = dataBlueprints.Count - 1; i >= 0; i--) {
                if(dataBlueprints[i] is TItemDataBlueprint ittresult && ittresult.GetItemType() == typeof(TItemData)) {
                    itemDataBlueprint = ittresult;
                    return true;
                }
            }

            itemDataBlueprint = default;
            return false;
        }

        #endregion

        #region IsStackable

        public bool IsStackable()
            => IsStackable(out int maxStack);

        public bool IsStackable(out int maxStack) {
            if(isStackable > 0) {
                maxStack = isStackable;
                return maxStack > 1;
            }
            foreach(IItemDataBlueprint idb in dataBlueprints) {
                if(idb is IItemDataStackableBlueprint stackableBlueprint) {
                    isStackable = stackableBlueprint.MaxStack;
                    maxStack = isStackable;
                    return isStackable > 1;
                }
            }

            maxStack = 1;
            isStackable = 1;
            return false;
        }

        #endregion

        #region Create

        public Item Create() {
            return Create(Guid.NewGuid());
        }

        public Item Create(Guid guid) {
            return Create(guid, new System.Random().Next(), 0);
        }

        public Item Create(Guid guid, int seed, int level) {
            var rng = new System.Random(seed);
            return Create(guid, rng, level);
        }

        private Item Create(Guid guid, System.Random random, int level) {
            var item = new Item(uid, guid);

            foreach(var d in dataBlueprints) {
                if(d?.TryCreate(random, level, out var data) ?? false) {
                    item.Add(data);
                }
            }

            return item;
        }

        #endregion

        #region Editor

        [ContextMenu("Test Generate")]
        private void TestGenerate() {
            var item = GenerateTMLItem();
            Debug.Log("Item Generated:\n" + IO.TML.TMLParser.ToString(item, true));
        }

        internal IO.TMLNode GenerateTMLItem() {
            return Create().GetTMLNode();
        }

        #endregion
    }

    public interface IItemDataBlueprint {
        Type GetItemType();
        bool TryCreate(System.Random random, int level, out IItemData data);
    }

    public interface IItemDataStackableBlueprint : IItemDataBlueprint {
        int MaxStack { get; }
    }
}
