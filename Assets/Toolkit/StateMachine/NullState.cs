using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public class NullState : IState, IStateMetadata {

        private Phase phase = Phase.None;

        string IStateMetadata.Name => "null state";
        string IStateMetadata.Description => "empty does nothing";
        Phase IState.Phase => phase;

        void IState.OnEnter() => phase = Phase.Update;
        void IState.OnExit() => phase = Phase.None;
        void IState.OnUpdate(float dt) { }
    }
}
