using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public delegate void OnStateTransitionBeginDelegate();
    public delegate void OnStateTransitionSwapDelegate(IState previous, IState newState);
    public delegate void OnStateTransitionCompleteDelegate();
}
