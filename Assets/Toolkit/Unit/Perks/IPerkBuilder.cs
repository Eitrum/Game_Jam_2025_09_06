using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public interface IPerkBuilder {
        IPerk Create(IUnit owner);
    }

    public interface IPerkBuilderWithData {
        IPerk Create(IUnit owner, IReadOnlyDictionary<PerkBuilderDataType, object> data);
    }

    public enum PerkBuilderDataType {
        Name,
        Source,
        Targets,
    }
}
