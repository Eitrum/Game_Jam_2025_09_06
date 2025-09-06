using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Inventory
{
    public interface IContainer
    {
        IReadOnlyList<IItem> Items { get; }

        int Count { get; }
        IItem GetItem(int index);
        T GetItem<T>(int index) where T : IItem;
        int GetItems<T>(IList<T> items) where T : IItem;

        bool AddItem(IItem item);
        bool RemoveItem(IItem item);
        bool CanRemoveItem(IItem item);
        bool CanAddItem(IItem item);

        void Sort();
        void Sort(Comparison<IItem> sortFunction);

        void UpdateContainer(float dt);
    }


    public interface IGridContainer : IContainer
    {
        int Width { get; }
        int Height { get; }
        IReadOnlyList<GridRect> Areas { get; }
        bool AddItemAtLocation(IItem item, Vector2Int location);
        bool RemoveItemAtLocation(Vector2Int location, out IItem item);
        bool HasItemInArea(GridRect area, out IItem item);
        bool GetArea(IItem item, out GridRect area);
    }

    public interface IWeightContainer : IContainer
    {
        float MaxWeight { get; }
        float CurrentWeight { get; }
    }

    public interface IContainerEvents
    {
        event OnContainerUpdateCallback OnContainerUpdate;
        event OnItemAddedCallback OnItemAdded;
        event OnItemRemovedCallback OnItemRemoved;
    }

    public delegate void OnContainerUpdateCallback();
    public delegate void OnItemAddedCallback(IItem item);
    public delegate void OnItemRemovedCallback(IItem item);
}
