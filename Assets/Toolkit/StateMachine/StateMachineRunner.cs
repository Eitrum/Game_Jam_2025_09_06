using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Toolkit.State {
    public class StateMachineRunner : MonoBehaviour {
        #region Main Instance

        private static StateMachineRunner instance;
        private static StateMachineRunner Instance {
            get {
                if(!instance) {
                    var go = new GameObject("--Global StateMachine Runner--");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<StateMachineRunner>();
                }
                return instance;
            }
        }

        #endregion

        #region Variables

        [SerializeField] private bool useProfiler = true;
        private TLinkedList<StateMachine> machines = new TLinkedList<StateMachine>();

        #endregion

        #region Properties

        public bool UseProfiler { get => useProfiler; set => useProfiler = value; }

        #endregion

        #region Init

        private void OnDestroy() {
            TLinkedList<StateMachine>.IterateWithNullcheck(machines, DestroyMachine);
        }

        private void DestroyMachine(StateMachine machine) => machine.Destroy();

        #endregion

        #region Update

        private void Update() {
            if(useProfiler)
                UpdateStateMachinesWithProfiler();
            else
                UpdateStateMachinesWithoutProfiler();
        }

        private void UpdateStateMachinesWithoutProfiler() {
            float dt = Time.deltaTime;
            var iterator = machines.GetEnumerator();
            TLinkedListNode<StateMachine> node;
            while(iterator.MoveNext(out node)) {
                if(node.Value.IsNull)
                    iterator.DestroyCurrent();
                else {
                    node.Value.Update(dt);
                }
            }
        }

        private void UpdateStateMachinesWithProfiler() {
            float dt = Time.deltaTime;
            Profiler.BeginSample($"StateMachineRunner::{name}");
            var iterator = machines.GetEnumerator();
            TLinkedListNode<StateMachine> node;
            while(iterator.MoveNext(out node)) {
                if(node.Value.IsNull)
                    iterator.DestroyCurrent();
                else {
                    Profiler.BeginSample($"{node.Value.Name}");
                    node.Value.Update(dt);
                    Profiler.EndSample();
                }
            }
            Profiler.EndSample();
        }

        #endregion

        #region Add / Remove

        public static void AddGlobal(StateMachine machine) {
            try {
                Instance.Add(machine);
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        public void Add(StateMachine machine) {
            machines.Add(machine);
        }

        public static bool RemoveGlobal(StateMachine machine) {
            try {
                return Instance.Remove(machine);
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Remove(StateMachine machine) {
            return machines.Remove(machine);
        }

        #endregion

        #region Create

        public static StateMachine CreateGlobal(string name) {
            var machine = new StateMachine(name);
            AddGlobal(machine);
            return machine;
        }

        public StateMachine Create(string name) {
            var machine = new StateMachine(name);
            Add(machine);
            return machine;
        }

        #endregion

        #region Find

        public static bool FindGlobal(string name, out StateMachine machine) {

            machine = null;
            return false;
        }

        #endregion
    }
}
