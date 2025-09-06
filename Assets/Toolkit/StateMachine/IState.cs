using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public interface IState {
        Phase Phase { get; }

        void OnEnter();
        void OnUpdate(float dt);
        void OnExit();
    }
}
