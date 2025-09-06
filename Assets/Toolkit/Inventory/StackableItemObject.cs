using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CreateAssetMenu(fileName = "Stackable Item", menuName = "Toolkit/Inventory/Stackable Item")]
    public class StackableItemObject : ScriptableObject, IItem, IStackable, IItemWeight
    {
        [SerializeField] private string nameOverride = "";
        [SerializeField, TextArea(2, 8)] private string description = "";
        [SerializeField] private Texture2D icon = null;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private ItemType itemType = ItemType.None;
        [SerializeField] private Rarity rarity = Rarity.None;
        [SerializeField] private int maxStack = 64;
        [SerializeField] private float weightPerStack = 0f;

        private int currentStack = 1;


        public string Name => string.IsNullOrEmpty(nameOverride) ? name : nameOverride;
        public string DisplayName => Name;
        public string Description => description;
        public int ItemId => Name.GetHash32();
        public Texture2D Icon => icon;
        public Object ItemReference => prefab;
        public ItemType Type => itemType;
        public Rarity Rarity => rarity;
        public bool IsNull => this == null;
        public int CurrentStackSize => currentStack;
        public int MaxStackSize => maxStack;

        public float Weight => weightPerStack * currentStack;

        public void DestroyItem() {
            Destroy(this);
        }

        public bool DropItem(Pose preferredPose) {
            if(prefab != null && prefab.GetComponent<IItemReference>() != null) {
                var go = Instantiate(prefab, preferredPose.position, preferredPose.rotation);
                var reference = go.GetComponent<IItemReference>();
                reference.Item = this;
                return true;
            }
            return false;
        }

        public bool IsEqual(IItem otherItem) {
            return
                itemType == otherItem.Type &&
                rarity == otherItem.Rarity &&
                ItemId == otherItem.ItemId &&
                otherItem.GetType() == GetType();
        }

        public void OnAddedToContainer(IContainer container) { }

        public void OnAddedToEquipment(IEquipment equipment) { }

        public bool Add(int amount) {
            if(currentStack + amount <= maxStack) {
                currentStack += amount;
                return true;
            }
            return false;
        }

        public bool Remove(int amount) {
            if(currentStack >= amount) {
                currentStack -= amount;
                return true;
            }
            return false;
        }

        public void Save() {
            throw new System.NotImplementedException();
        }

        public void Load() {
            throw new System.NotImplementedException();
        }
    }
}
