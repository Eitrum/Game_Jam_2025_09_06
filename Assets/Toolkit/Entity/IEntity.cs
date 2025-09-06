using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit.EntityExtended;

namespace Toolkit {
    public interface IEntity {
        IEntityColliders Colliders { get; }
    }
}
