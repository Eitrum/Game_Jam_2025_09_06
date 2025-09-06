using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CreateAssetMenu(fileName = "Simple Item", menuName = "Toolkit/Inventory/Simple Item")]
    public class SimpleItemObject : ScriptableObject, IItem, IItemWeight
    {
        [SerializeField] private string nameOverride = "";
        [SerializeField, TextArea(2, 8)] private string description = "";
        [SerializeField] private Texture2D icon = null;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private ItemType itemType = ItemType.None;
        [SerializeField] private Rarity rarity = Rarity.None;
        [SerializeField] private float weight = 0f;

        public string Name => string.IsNullOrEmpty(nameOverride) ? name : nameOverride;
        public string DisplayName => Name;
        public string Description => description;
        public int ItemId => Name.GetHash32();
        public Texture2D Icon => icon;
        public Object ItemReference => prefab;
        public ItemType Type => itemType;
        public Rarity Rarity => rarity;
        public bool IsNull => this == null;

        public float Weight => weight;

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
    }
}
