
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.EntityExtended {
    public interface IEntityColliders {
        Collider Main { get; }
        IReadOnlyList<Collider> Colliders { get; }
    }
}
