using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.TurnEngine {
    public static class TurnSystem {

        #region Variables

        public const string TAG = "<color=purple>[Turn System]</color> - ";
        private const float UPDATE_INTERVAL = 0.2f;
        private static List<Instance> instances = new List<Instance>();

        #endregion

        #region Create Instance

        public static Instance CreateInstance(TurnMode mode) {
            var instance = new Instance(mode);
            instances.Add(instance);
            return instance;
        }

        public static Instance CreateInstance(IReadOnlyList<ITurn> turns)
            => CreateInstance(TurnMode.All, turns);

        public static Instance CreateInstance(IEnumerable<ITurn> turns)
            => CreateInstance(TurnMode.All, turns);

        public static Instance CreateInstance(TurnMode mode, IReadOnlyList<ITurn> turns) {
            var instance = new Instance(mode, turns);
            instances.Add(instance);
            return instance;
        }

        public static Instance CreateInstance(TurnMode mode, IEnumerable<ITurn> turns) {
            var instance = new Instance(mode, turns);
            instances.Add(instance);
            return instance;
        }

        #endregion

        #region Remove Instance

        public static void RemoveInstance(Instance instance) {
            if(instances.Remove(instance)) {
                instance.Destroy();
            }
        }

        public static void RemoveAllInstances() {
            foreach(var instance in instances) {
                instance.Destroy();
            }
            instances.Clear();
        }

        #endregion

        #region Initialization / Update

        static TurnSystem() {
            TurnUpdateManager.Instance.OnUpdate += Update;
        }

        private static void Update() {
            for(int i = instances.Count - 1; i >= 0; i--) {
                var instance = instances[i];
                if(instance.HasUpdate) {
                    instance.Update();
                }
                else if((instance.DestroyWhenEmpty && instance.IsEmpty)) {
                    RemoveInstance(instance);
                }
                else if(instance.IsDestroyed) {
                    instances.Remove(instance);
                }
            }
        }

        #endregion

        public class Instance : INullable {

            #region Variables

            private List<ITurn> turns = new List<ITurn>();
            private TurnMode mode = TurnMode.None;
            private int rounds = 0;
            private bool destroyed = false;
            public event OnRoundPassedCallback OnRoundPassed;
            public event OnNewTurnDelegate OnNewTurn;
            public event OnEndTurnDelegate OnEndTurn;
            private NextTurnCallbackFix callbackFix = NextTurnCallbackFix.None;

            #endregion

            #region Properties

            public TurnMode Mode {
                get => mode;
                set {
                    if(mode.HasFlag(TurnMode.RoundIndicator)) {
                        if(!value.HasFlag(TurnMode.RoundIndicator)) {
                            // Remove round indicator
                        }
                    }
                    else {
                        if(value.HasFlag(TurnMode.RoundIndicator)) {
                            // Add round indicator
                        }
                    }
                    Debug.LogError(TAG + "Mode change currently not supported");
                    //mode = value;
                }
            }

            public bool HasRecycle => mode.HasFlag(TurnMode.Recycle);
            public bool HasUpdate => mode.HasFlag(TurnMode.Update);
            public bool HasRoundIndicator => mode.HasFlag(TurnMode.RoundIndicator);
            public bool DestroyWhenEmpty => mode.HasFlag(TurnMode.DestroyWhenEmpty);

            public int Round => rounds;
            public bool HasTurnsPending => turns.Count > 0;
            public ITurn CurrentTurn => turns.Count > 0 ? turns[0] : null;

            public bool IsEmpty => HasRoundIndicator ? (turns.Count == 1 && turns[0] is RoundIndicator) : turns.Count == 0;

            public IReadOnlyList<ITurn> Turns => turns;

            public bool IsDestroyed => destroyed;
            public bool IsNull => destroyed;

            #endregion

            #region Constructor

            internal Instance(TurnMode mode) {
                this.mode = mode;
                if(HasRoundIndicator)
                    AddRoundIndicator();
            }

            internal Instance(TurnMode mode, IReadOnlyList<ITurn> turns) {
                this.mode = mode;
                if(turns != null) {
                    this.turns.AddRange(turns);
                }
                if(HasRoundIndicator)
                    AddRoundIndicator();
            }

            internal Instance(TurnMode mode, IEnumerable<ITurn> turns) : this(mode) {
                this.mode = mode;
                if(turns != null) {
                    this.turns.AddRange(turns);
                }
                if(HasRoundIndicator)
                    AddRoundIndicator();
            }

            public void Destroy() {
                if(!IsEmpty) {
                    Debug.LogWarning(TAG + "Destroying turn instances while having active turns in list!");
                }
                destroyed = true;
                turns.Clear();
            }

            #endregion

            #region Update / Next Round

            public void Update() {
                if(IsDestroyed || IsEmpty) {
                    return;
                }
                var current = CurrentTurn;
                if(current != null && (current.IsComplete || !current.HasStarted)) {
                    NextRound();
                }
            }

            public bool NextRound() {
                if(callbackFix == NextTurnCallbackFix.Callback)
                    return false;
                if(IsDestroyed || IsEmpty) {
                    return false;
                }
                var current = CurrentTurn;
                if(current == null) {
                    return false;
                }

                if(!current.HasStarted) {
                    var t1 = current;
                    callbackFix = NextTurnCallbackFix.Start;
                    current.OnStart();
                    if(callbackFix == NextTurnCallbackFix.Start && t1 == current)
                        OnNewTurn?.Invoke(t1);
                    callbackFix = NextTurnCallbackFix.None;
                    return true;
                }

                if(!current.IsComplete && !current.OnSkip())
                    return false;

                if(callbackFix == NextTurnCallbackFix.Start) {
                    callbackFix = NextTurnCallbackFix.Callback;
                    OnNewTurn?.Invoke(current);
                    callbackFix = NextTurnCallbackFix.None;
                }

                current.OnEnd();
                OnEndTurn?.Invoke(current);
                turns.RemoveAt(0);
                if(HasRecycle || current is RoundIndicator) {
                    current.OnReset();
                    turns.Add(current);
                }
                current = CurrentTurn;
                if(current == null) {
                    return false;
                }

                var t = current;
                callbackFix = NextTurnCallbackFix.Start;
                current.OnStart();
                if(callbackFix == NextTurnCallbackFix.Start && t == current)
                    OnNewTurn?.Invoke(t);
                callbackFix = NextTurnCallbackFix.None;

                /*if(current is RoundIndicator) {
                    return NextRound();
                }*/
                return true;
            }

            private enum NextTurnCallbackFix {
                None,
                Start,
                Callback,
            }

            #endregion

            #region Turn Add/Insert/Remove Manipulation

            public void Insert(int index, ITurn turn) {
                if(index <= 0)
                    this.turns.Add(turn);
                else
                    this.turns.Insert(index, turn);
            }

            public void Add(ITurn turn, bool beforeRoundIndicator = true) {
                if(beforeRoundIndicator && HasRoundIndicator) {
                    for(int i = 1, length = turns.Count; i < length; i++) {
                        if(turns[i] is RoundIndicator) {
                            turns.Insert(i, turn);
                            return;
                        }
                    }
                    turns.Add(turn);
                }
                else {
                    turns.Add(turn);
                }
            }

            public bool Remove(ITurn turn) {
                if(turns.Count > 0 && turns[0] == turn) {
                    var res = turns.Remove(turn);
                    if(res) {
                        callbackFix = NextTurnCallbackFix.None;
                        NextRound();
                    }
                    return res;
                }
                return turns.Remove(turn);
            }

            public void Clear(bool includingRoundIndicator = false) {
                for(int i = turns.Count - 1; i >= 0; i--) {
                    if(includingRoundIndicator || !(turns[i] is RoundIndicator)) {
                        turns.RemoveAt(i);
                    }
                }
            }

            #endregion

            #region Utility

            public void SetTurn(ITurn turn) {
                int index = -1;
                for(int i = 0, length = turns.Count; i < length; i++) {
                    if(turns[i] == turn) {
                        index = i;
                    }
                }
                if(index > -1)
                    Skip(index - 1);
                else
                    Debug.LogError("Could not find turn in turn instance!");
            }

            public void Skip(int turns) {
                turns++;
                if(mode.HasFlag(TurnMode.Recycle)) {
                    for(int i = 0; i < turns; i++) {
                        var t = this.turns[0];
                        this.turns.RemoveAt(0);
                        this.turns.Add(t);
                    }
                }
                else {
                    for(int i = 0; i < turns; i++) {
                        var t = this.turns[0];
                        this.turns.RemoveAt(0);
                        if(t is RoundIndicator)
                            this.turns.Add(t);
                    }
                }
            }

            #endregion

            #region Round Indicator

            public void AddRoundIndicator() {
                mode |= TurnMode.RoundIndicator;
                for(int i = 0, length = turns.Count; i < length; i++) {
                    if(turns[i] is RoundIndicator) {
                        return;
                    }
                }
                turns.Add(new RoundIndicator(OnRoundIndicatorTrigger));
            }

            public void ClearRoundIndicators() {
                mode &= ~TurnMode.RoundIndicator;
                for(int i = turns.Count - 1; i >= 0; i--) {
                    if(turns[i] is RoundIndicator) {
                        turns.RemoveAt(i);
                    }
                }
            }

            public void ResetRoundCounter() => rounds = 0;

            private void OnRoundIndicatorTrigger() { rounds++; Debug.Log(TAG + "On Round Passed!"); OnRoundPassed?.Invoke(rounds); }

            #endregion
        }

        private class TurnUpdateManager : MonoSingleton<TurnUpdateManager> {

            protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

            private float time = 0f;
            public event System.Action OnUpdate;

            void Start() {
                Debug.Log(TAG + "Turn Update Manager Initialized");
            }

            void Update() {
                time += Time.unscaledDeltaTime;
                if(time >= UPDATE_INTERVAL) {
                    time -= UPDATE_INTERVAL;
                    this.OnUpdate?.Invoke();
                }
            }
        }
    }
}
