using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public interface IStateMetadata {
        string Name { get; }
        string Description { get; }
    }
}
