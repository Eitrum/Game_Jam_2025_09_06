using UnityEngine.AI;

namespace Toolkit.AI.Navigation
{
    public interface INavMeshQueryFilter
    {
        NavMeshQueryFilter QueryFilter { get; }
    }
}
