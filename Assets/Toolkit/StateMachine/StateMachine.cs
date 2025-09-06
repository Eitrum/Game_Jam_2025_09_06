using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.State {
    public class StateMachine : INullable {
        #region Variables

        private const string TAG = "[StateMachine] - ";

        private bool loggingEnabled = false;
        private IState current;
        private IState next;

        private Phase cachedPhase = Phase.None;
        private List<ITransition> transitions = new List<ITransition>();
        private ITransitionEvents transitionEvents;

        public event OnStateTransitionBeginDelegate OnStateTransitionBegin;
        public event OnStateTransitionSwapDelegate OnStateTransitionSwap;
        public event OnStateTransitionCompleteDelegate OnStateTransitionComplete;

        #endregion

        #region Properties

        public bool LoggingEnabled {
            get => loggingEnabled;
            set => loggingEnabled = value;
        }
        public string Name { get; private set; }

        public IState Current => current;

        public bool HasState => current != null || next != null;
        public bool IsTransitioning => StateUtility.IsInTransition(current);

        public bool IsNull { get; private set; } = false;

        #endregion

        #region Constructor

        public StateMachine() { Name = "StateMachine"; }
        public StateMachine(string name) { this.Name = name; }

        public StateMachine(string name, IReadOnlyList<ITransition> transitions) : this(name) {
            this.transitions.AddRange(transitions);
        }

        public StateMachine(IReadOnlyList<ITransition> transitions) : this() {
            this.transitions.AddRange(transitions);
        }

        public static StateMachine CreateWithoutCopy(List<ITransition> transitions) {
            StateMachine stateMachine = new StateMachine();
            stateMachine.transitions = transitions;
            return stateMachine;
        }

        public static StateMachine CreateWithoutCopy(string name, List<ITransition> transitions) {
            StateMachine stateMachine = new StateMachine(name);
            stateMachine.transitions = transitions;
            return stateMachine;
        }

        #endregion

        #region Destroy

        public void Destroy() {
            IsNull = true;
            transitions.Clear();
            next = null;
            if(current != null && current.Phase != Phase.Exiting)
                current?.OnExit();
            current = null;
        }

        #endregion

        #region Update

        public void Update(float dt) {
            UpdateTransitions();
            UpdateState(dt);
        }

        private void UpdateTransitions() {
            if(next != null)
                return;
            foreach(var t in transitions) {
                if((t.Current == null || t.Current == current) && t.CanTransition()) {
                    SetTransition(t);
                    return;
                }
            }
        }

        private void UpdateState(float dt) {
            if(current == null) {
                if(next != null)
                    InternalSwap();
                return;
            }
            var phase = current.Phase;

            if(cachedPhase != phase) {
                cachedPhase = phase;
                if(cachedPhase == Phase.Update) {
                    OnTransitionComplete();
                }
            }

            switch(phase) {
                case Phase.Update:
                    current.OnUpdate(dt);
                    break;
                case Phase.Entering:

                    break;
                case Phase.Exiting:

                    break;
                case Phase.None:
                    InternalSwap();
                    break;
            }
        }

        #endregion

        #region Set Transition

        public bool SetTransition(ITransition transition) {
            if(transition == null) {
                return false;
            }
            InternalSetNext(transition);
            return true;
        }

        #endregion

        #region TransitionTo

        public bool TransitionTo() {
            InternalSetNext(null, null);
            return true;
        }

        public bool TransitionTo<T>(T state) where T : IState {
            InternalSetNext(state);
            return true;
        }

        public bool TransitionTo<T>(T state, bool replaceCurrentTransition) where T : IState {
            try {
                if(!replaceCurrentTransition && next != null)
                    return false;
                InternalSetNext(state);
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        #endregion

        #region Internal Stuff

        private void InternalSetNext(IState state) {
            InternalSetNext(state, null);
        }

        private void InternalSetNext(ITransition transition) {
            if(transition == null) {
                return;
            }
            InternalSetNext(transition.Target, transition);
        }

        private void InternalSetNext(IState state, ITransition transition) {
            try {
                transitionEvents = transition as ITransitionEvents;
                next = state;
                OnTransitionBegin();
                var phase = current?.Phase ?? Phase.None;
                switch(phase) {
                    case Phase.None:
                        InternalSwap();
                        break;
                    case Phase.Entering:
                    case Phase.Update:
                        current.OnExit();
                        if(current.Phase == Phase.None)
                            InternalSwap();
                        break;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        private void InternalSwap() {
            try {
                var c = current;
                current = next;
                next = null;
                OnTransitionSwap(c, current);
                current?.OnEnter();
                cachedPhase = current?.Phase ?? Phase.None;
                switch(cachedPhase) {
                    case Phase.None:
                    case Phase.Update:
                        OnTransitionComplete();
                        break;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Events

        private void OnTransitionBegin() {
            if(loggingEnabled)
                Debug.Log(TAG + $"OnTransitionBegin ({StateUtility.GetName(current)} -> {StateUtility.GetName(next)})");
            transitionEvents?.OnTransitionBegin();
            OnStateTransitionBegin?.Invoke();
        }

        private void OnTransitionSwap(IState previous, IState target) {
            if(loggingEnabled)
                Debug.Log(TAG + $"OnTransitionSwap ({StateUtility.GetName(previous)} -> {StateUtility.GetName(target)})");
            transitionEvents?.OnTransitionSwap();
            OnStateTransitionSwap?.Invoke(previous, target);
        }

        private void OnTransitionComplete() {
            if(loggingEnabled)
                Debug.Log(TAG + $"OnTransitionComplete ({StateUtility.GetName(current)})");
            transitionEvents?.OnTransitionComplete();
            OnStateTransitionComplete?.Invoke();
            transitionEvents = null;
        }

        #endregion
    }
}
