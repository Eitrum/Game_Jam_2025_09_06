using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    public interface INavMeshSource
    {
        NavMeshBuildSource Source { get; }
        int Area { get; set; }
    }
}
