using UnityEngine;

namespace Toolkit.Inventory {
    public interface IItem : INullable {
        // General Item Data
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        int ItemId { get; }
        Texture2D Icon { get; }
        UnityEngine.Object ItemReference { get; }
        ItemType Type { get; }
        Rarity Rarity { get; }

        // Unity Specific
        void OnAddedToContainer(IContainer container);
        void OnAddedToEquipment(IEquipment equipment);
        bool DropItem(Pose preferredPose);
        void DestroyItem();

        bool IsEqual(IItem otherItem);
    }

    public interface IItemReference
    {
        IItem Item { get; set; }
    }

    public interface IUpdateable
    {
        void UpdateItem(float dt);
    }

    public interface IStackable
    {
        int CurrentStackSize { get; }
        int MaxStackSize { get; }
        bool Add(int amount);
        bool Remove(int amount);
    }

    public interface IItemWeight
    {
        float Weight { get; }
    }

    public interface ISize
    {
        Vector2Int GridSize { get; }
    }

    public interface IConsumable
    {
        bool Use();
    }
}
