using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public class EventState : IState, IStateMetadata {

        #region Variables

        public event System.Action OnEnter;
        public event System.Action<float> OnUpdate;
        public event System.Action OnExit;

        #endregion

        #region Constructor

        public EventState() { }
        public EventState(string name) {
            this.Name = name;
        }

        public EventState(string name, System.Action onEnter, System.Action<float> onUpdate, System.Action onExit) {
            this.Name = name;
            this.OnEnter = onEnter;
            this.OnUpdate = onUpdate;
            this.OnExit = onExit;
        }

        #endregion

        #region IStateMetadata Impl

        public string Name { get; private set; }
        string IStateMetadata.Description => "no description";

        #endregion

        #region IState Impl

        public Phase Phase { get; private set; }

        void IState.OnEnter() {
            OnEnter?.Invoke();
            Phase = Phase.Update;
        }

        void IState.OnUpdate(float dt) {
            OnUpdate?.Invoke(dt);
        }

        void IState.OnExit() {
            OnExit?.Invoke();
            Phase = Phase.None;
        }

        #endregion
    }
}
