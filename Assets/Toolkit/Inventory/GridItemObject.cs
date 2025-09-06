using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [CreateAssetMenu(fileName = "Grid Item", menuName = "Toolkit/Inventory/Grid Item")]
    public class GridItemObject : ScriptableObject, IItem, ISize
    {
        [SerializeField] private string nameOverride = "";
        [SerializeField, TextArea(2, 8)] private string description = "";
        [SerializeField] private Texture2D icon = null;
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private ItemType itemType = ItemType.None;
        [SerializeField] private Rarity rarity = Rarity.None;
        [SerializeField] private Vector2Int size = new Vector2Int(1, 1);

        public string Name => string.IsNullOrEmpty(nameOverride) ? name : nameOverride;
        public string DisplayName => Name;
        public string Description => description;
        public int ItemId => Name.GetHash32();
        public Texture2D Icon => icon;
        public Object ItemReference => prefab;
        public ItemType Type => itemType;
        public Rarity Rarity => rarity;
        public bool IsNull => this == null;

        public Vector2Int GridSize => size;

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

        public void Save() {
            throw new System.NotImplementedException();
        }

        public void Load() {
            throw new System.NotImplementedException();
        }
    }
}
