using System;

namespace Toolkit.AI.Navigation
{
    public interface INavMeshGenerator
    {
        void Add(INavMeshSource source);
        void Remove(INavMeshSource source);
    }
}
