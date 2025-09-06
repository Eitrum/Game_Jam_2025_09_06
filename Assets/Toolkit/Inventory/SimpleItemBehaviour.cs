using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Simple Item")]
    public class SimpleItemBehaviour : MonoBehaviour, IItem, IItemWeight
    {
        [SerializeField] private string nameOverride = "";
        [SerializeField, TextArea(2, 8)] private string description = "";
        [SerializeField] private Texture2D icon = null;
        [SerializeField] private ItemType itemType = ItemType.None;
        [SerializeField] private Rarity rarity = Rarity.None;

        private IWeight weight;

        public string Name => string.IsNullOrEmpty(nameOverride) ? name : nameOverride;
        public string DisplayName => Name;
        public string Description => description;
        public int ItemId => Name.GetHash32();
        public Texture2D Icon => icon;
        public Object ItemReference => this.gameObject;
        public ItemType Type => itemType;
        public Rarity Rarity => rarity;
        public bool IsNull => this == null;

        public float Weight => weight?.Weight ?? 0f;

        void Awake() {
            weight = GetComponent<IWeight>();
        }

        public void DestroyItem() {
            Destroy(this.gameObject);
        }

        public bool DropItem(Pose preferredPose) {
            transform.gameObject.SetActive(true);
            transform.SetPositionAndRotation(preferredPose.position, preferredPose.rotation);
            return true;
        }

        public bool IsEqual(IItem otherItem) {
            return
                itemType == otherItem.Type &&
                rarity == otherItem.Rarity &&
                ItemId == otherItem.ItemId &&
                otherItem.GetType() == GetType();
        }

        public void OnAddedToContainer(IContainer container) {
            transform.gameObject.SetActive(false);
        }

        public void OnAddedToEquipment(IEquipment equipment) {
            transform.gameObject.SetActive(false);
        }

        public void Save() {
            throw new System.NotImplementedException();
        }

        public void Load() {
            throw new System.NotImplementedException();
        }
    }
}
