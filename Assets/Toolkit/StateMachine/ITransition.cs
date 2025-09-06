using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public interface ITransition {
        IState Current { get; }
        IState Target { get; }

        bool CanTransition();
    }
}
