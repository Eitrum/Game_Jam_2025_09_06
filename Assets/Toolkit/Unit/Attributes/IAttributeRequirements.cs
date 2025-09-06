using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IAttributeRequirement {
        AttributeType Type { get; }
        float Amount { get; }
    }

    public interface IAttributeRequirements {
        IReadOnlyList<AttributeRequirement> Requirements { get; }
    }
}
