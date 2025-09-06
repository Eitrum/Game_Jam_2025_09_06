using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Toolkit.PhysicEx {
    public static class PhysicsUpdateSystem {
        #region Variables

        public static event Action OnBeforeSyncTransform;
        public static event Action OnBeforeSimulate;
        public static event Action OnAfterSimulate;

        /// <summary>
        /// Called whenever physics simulation wasn't called during a frame.
        /// Useful for performance balancing
        /// </summary>
        public static event Action OnDidNotUpdatePhysics;

        private static bool enabled = true;
        private static float time = 0f;
        private static float timeStep = 0f;
        private static float defaultFixedDeltaTime;
        private static bool didUpdatePhysics = true;
        private static int simulatedAmount = 0;
        private static bool useSmoothing = false;
        private static CircularBuffer<float> smoothing;
        private static PlayerLoopUtilty.Node fixedUpdateSystem;

        private static Dictionary<int, DeterministicDelegates<ModifyContactDelegate>> modifiableContacts = new Dictionary<int, DeterministicDelegates<ModifyContactDelegate>>();
        private static Dictionary<int, Rigidbody> registeredRigidbodies = new Dictionary<int, Rigidbody>();

        #endregion

        #region Properties

        public static bool IsPhysicsFrame => didUpdatePhysics;
        public static float DefaultFixedDeltaTime => defaultFixedDeltaTime;
        public static int SimulatedAmount => simulatedAmount;

        public static int UpdatesPerSecond {
            get => timeStep < Mathf.Epsilon ? 0 : (int)(1f / timeStep);
            set {
                var t = Mathf.Clamp(value, 0, 4196);
                timeStep = t == 0 ? 0f : (1f / t);
            }
        }

        public static bool Enabled {
            get => enabled;
            set {
                if(enabled != value) {
                    enabled = value;

                    // Change updates
                    PhysicsUpdateSystemInstance.Instance.OnUpdate -= OnUpdate;
                    if(enabled)
                        PhysicsUpdateSystemInstance.Instance.OnUpdate += OnUpdate;
                    else {
                        PhysicsUpdateSystemInstance.Instance.OnUpdate -= OnUpdate;
                        didUpdatePhysics = false;
                    }
                }
            }
        }

        #endregion

        #region Initialize

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            defaultFixedDeltaTime = Time.fixedDeltaTime;

            if(PhysicsUpdateSettings.IsToolkitPhysicsUpdate) {
                Physics.simulationMode = SimulationMode.Script;
                Physics.autoSyncTransforms = false;
                timeStep = PhysicsUpdateSettings.UpdatePerFrame ? 0f : (1f / PhysicsUpdateSettings.PhysicsSettings.UpdatesPerSecond);
                if(PhysicsUpdateSettings.TryGetSmoothing(out int smooth)) {
                    useSmoothing = true;
                    smoothing = new CircularBuffer<float>(smooth);
                }
                PhysicsUpdateSystemInstance.Instance.OnUpdate += OnUpdate;

                if(PlayerLoopUtilty.Find<UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate>(out fixedUpdateSystem)) {
                    fixedUpdateSystem.Enabled = false;
                    PlayerLoopUtilty.Save();
                }
            }

            Physics.ContactModifyEventCCD += OnModifyContact;
            Physics.ContactModifyEvent += OnModifyContact;
        }

        #endregion

        #region Simulate

        public static void Simulate(float timeStep) {
            // Ensure dt is calculated correctly
            Time.fixedDeltaTime = timeStep;
            // Handle Transform Sync
            OnBeforeSyncTransform?.Invoke();
            Physics.SyncTransforms();

            // Handle Simulations
            OnBeforeSimulate?.Invoke();
            fixedUpdateSystem?.Run();
            Physics.Simulate(timeStep);
            OnAfterSimulate?.Invoke();

            simulatedAmount++;
        }

        #endregion

        #region Modifiable Contacts

        public static void RegisterContactCallback(int rigidbodyId, ModifyContactDelegate callback) {
            if(!modifiableContacts.TryGetValue(rigidbodyId, out DeterministicDelegates<ModifyContactDelegate> dd))
                modifiableContacts.Add(rigidbodyId, dd = new DeterministicDelegates<ModifyContactDelegate>());
            dd.Add(callback);
        }

        public static void UnregisterContactCallback(int rigidbodyId, ModifyContactDelegate callback) {
            if(modifiableContacts.TryGetValue(rigidbodyId, out DeterministicDelegates<ModifyContactDelegate> dd)) {
                dd.Remove(callback);
                if(dd.IsEmpty)
                    modifiableContacts.Remove(rigidbodyId);
            }
        }

        #endregion

        #region Rigidbody Lookup

        public static void RegisterRigidbody(Rigidbody body) {
            registeredRigidbodies.TryAdd(body.GetInstanceID(), body);
        }

        public static void RegisterRigidbody(Rigidbody body, int rigidbodyId) {
            registeredRigidbodies.TryAdd(rigidbodyId, body);
        }

        public static void UnregisterRigidbody(Rigidbody body) {
            registeredRigidbodies.Remove(body.GetInstanceID());
        }

        public static void UnregisterRigidbody(int rigidbodyId) {
            registeredRigidbodies.Remove(rigidbodyId);
        }

        #endregion

        #region Callbacks

        private static void OnUpdate() {
            var dt = Time.deltaTime;
            if(dt < Mathf.Epsilon)
                return;

            if(timeStep <= 0f) {
                if(useSmoothing) {
                    smoothing.Write(dt);
                    dt = smoothing.Average();
                }

                Simulate(dt);
            }
            else {
                time += dt;
                didUpdatePhysics = time >= timeStep;
                while(time >= timeStep) {
                    time -= timeStep;
                    Debug.Log($"Physics Update at frame ({Time.frameCount}) with step: {timeStep}");
                    Simulate(timeStep);
                }
                if(!didUpdatePhysics)
                    OnDidNotUpdatePhysics?.Invoke();
            }
        }

        private static void OnModifyContact(PhysicsScene scene, NativeArray<ModifiableContactPair> collisions) {
            for(int i = 0, length = collisions.Length; i < length; i++) {
                var mcp = collisions[i];
                if(modifiableContacts.TryGetValue(mcp.bodyInstanceID, out DeterministicDelegates<ModifyContactDelegate> val)) {
                    val.Delegate.Invoke(ref mcp);
                }
                if(modifiableContacts.TryGetValue(mcp.otherBodyInstanceID, out DeterministicDelegates<ModifyContactDelegate> val2)) {
                    val2.Delegate.Invoke(ref mcp);
                }
                collisions[i] = mcp;
            }
        }

        #endregion

        #region Set Modifiable

        private static List<Collider> colliderCache = new List<Collider>();

        public static void SetGameObjectModifiable(GameObject gameObject, bool modifiable)
            => SetGameObjectModifiable(gameObject, modifiable, true);

        public static void SetGameObjectModifiable(GameObject gameObject, bool modifiable, bool includeChildren) {
            if(modifiable)
                RegisterGameObjectModifiable(gameObject, includeChildren);
            else
                UnregisterGameObjectModifiable(gameObject, includeChildren);
        }

        public static void RegisterGameObjectModifiable(GameObject gameObject)
            => RegisterGameObjectModifiable(gameObject, true);

        public static void RegisterGameObjectModifiable(GameObject gameObject, bool includeChildren) {
            if(includeChildren)
                gameObject.GetComponentsInChildren(colliderCache);
            else
                gameObject.GetComponents(colliderCache);

            foreach(var c in colliderCache)
                c.hasModifiableContacts = true;
        }

        public static void UnregisterGameObjectModifiable(GameObject gameObject)
           => UnregisterGameObjectModifiable(gameObject, true);

        public static void UnregisterGameObjectModifiable(GameObject gameObject, bool includeChildren) {
            if(includeChildren)
                gameObject.GetComponentsInChildren(colliderCache);
            else
                gameObject.GetComponents(colliderCache);

            foreach(var c in colliderCache)
                c.hasModifiableContacts = false;
        }

        #endregion

        public delegate void ModifyContactDelegate(ref ModifiableContactPair pair);
    }

    [DefaultExecutionOrder(int.MinValue)]
    internal class PhysicsUpdateSystemInstance : MonoSingleton<PhysicsUpdateSystemInstance> {
        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }
        public event Action OnUpdate;
        private void Update() => OnUpdate?.Invoke();
    }
}
