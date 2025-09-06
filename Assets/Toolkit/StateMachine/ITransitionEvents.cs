using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public interface ITransitionEvents {
        void OnTransitionBegin();
        void OnTransitionSwap();
        void OnTransitionComplete();
    }
}
