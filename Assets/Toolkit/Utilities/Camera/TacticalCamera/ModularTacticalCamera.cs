using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.TacticalCamera
{
    [AddComponentMenu("Toolkit/Camera/Modular Tactical Camera")]
    public class ModularTacticalCamera : NullableBehaviour, ITacticalCamera,
        IEarlyUpdate, IUpdate, IPostUpdate
    {
        #region Variables

        private const string TAG = "[Modular Tactical Camera] - ";

        // Update
        [SerializeField] private bool manualUpdate; // Editor in run-time should be
        private TLinkedListNode<IEarlyUpdate> earlyUpdateNode = null;
        private TLinkedListNode<IUpdate> mainUpdateNode = null;
        private TLinkedListNode<IPostUpdate> postUpdateNode = null;
        private TacticalCameraUpdateState updateState = TacticalCameraUpdateState.None;

        // Modules
        [SerializeField, TypeFilter(typeof(ITacticalCameraInput))] private UnityEngine.Object inputModule = null;
        [SerializeField] private List<Module> modules = new List<Module>();
        private ITacticalCameraInput input;

        // Multiplayer Support
        private int id = 0;

        // References
        private Transform root;
        private Transform pivot;
        private Transform cameraTransform;
        private Camera cameraReference;

        // Global
        private static List<ModularTacticalCamera> instances = new List<ModularTacticalCamera>();

        #endregion

        #region Properties

        // Update

        public bool Enabled {
            get => enabled;
            set => enabled = value;
        }

        public bool ManualUpdate {
            get => manualUpdate;
            set {
                if(manualUpdate != value) {
                    manualUpdate = value;
                    if(Application.isPlaying) {
                        if(manualUpdate) {
                            UpdateSystem.Unsubscribe(earlyUpdateNode);
                            UpdateSystem.Unsubscribe(mainUpdateNode);
                            UpdateSystem.Unsubscribe(postUpdateNode);
                        }
                        else {
                            earlyUpdateNode = UpdateSystem.Subscribe(this as IEarlyUpdate);
                            mainUpdateNode = UpdateSystem.Subscribe(this as IUpdate);
                            postUpdateNode = UpdateSystem.Subscribe(this as IPostUpdate);
                        }
                    }
                }
            }
        }

        public TacticalCameraUpdateState UpdateState => updateState;

        // Modules
        public ITacticalCameraInput Input => input;
        public IReadOnlyList<ITacticalCameraModule> Modules {
            get {
                ITacticalCameraModule[] mod = new ITacticalCameraModule[modules.Count];
                for(int i = 0, length = mod.Length; i < length; i++)
                    mod[i] = modules[i].Instance;
                return mod;
            }
        }

        // Multiplayer Support
        public int PlayerId { get => id; set => id = value; }

        // References
        public Transform Root => root;
        public Transform Pivot => pivot;
        public Transform CameraTransform => cameraTransform;
        public Camera Camera => cameraReference;

        // Global
        public static ModularTacticalCamera Main => instances.Count > 0 ? instances[0] : null;
        public static IReadOnlyList<ModularTacticalCamera> Instances => instances;

        #endregion

        #region Init

        private void Awake() {
            // Setup References
            root = transform;
            cameraReference = root.GetComponentInChildren<Camera>();
            cameraTransform = cameraReference.transform;

            if(root == cameraTransform) {
                Debug.LogWarning(TAG + "Camera is attached to root");
                pivot = root;
            }
            else {
                pivot = cameraTransform.parent;

                if(pivot == root)
                    pivot = cameraTransform;
            }
            // Input
            input = inputModule as ITacticalCameraInput;
            // Add Global Instance
            instances.Add(this);
        }

        void OnDestroy() {
            // Do Cleanup
            for(int i = 0, length = modules.Count; i < length; i++) {
                modules[i].Destroy();
            }
            // Remove Global Instance
            instances.Remove(this);
        }

        private void OnEnable() {
            if(!manualUpdate) {
                earlyUpdateNode = UpdateSystem.Subscribe(this as IEarlyUpdate);
                mainUpdateNode = UpdateSystem.Subscribe(this as IUpdate);
                postUpdateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            }
        }

        private void OnDisable() {
            if(!manualUpdate) {
                UpdateSystem.Unsubscribe(earlyUpdateNode);
                UpdateSystem.Unsubscribe(mainUpdateNode);
                UpdateSystem.Unsubscribe(postUpdateNode);
            }
        }

        #endregion

        #region Update

        public void DoUpdate(float dt) {
            updateState = TacticalCameraUpdateState.Early;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.Main;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.Post;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.None;
        }

        void IEarlyUpdate.EarlyUpdate(float dt) {
            updateState = TacticalCameraUpdateState.Early;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.None;
        }

        void IUpdate.Update(float dt) {
            updateState = TacticalCameraUpdateState.Main;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.None;
        }

        void IPostUpdate.PostUpdate(float dt) {
            updateState = TacticalCameraUpdateState.Post;
            InternalUpdate(dt);
            updateState = TacticalCameraUpdateState.None;
        }

        private void InternalUpdate(float dt) {
            for(int i = 0, length = modules.Count; i < length; i++) {
                var m = modules[i];
                if(m.Enabled && m.Update == updateState)
                    m.Instance?.UpdateModule(this, dt);
            }
        }

        #endregion

        #region Module

        public void AddModule(ITacticalCameraModule module, TacticalCameraUpdateState update) {
            this.modules.Add(new Module(module, update));
        }

        public void RemoveModule(ITacticalCameraModule module) {
            for(int i = modules.Count - 1; i >= 0; i--) {
                if(modules[i].Instance == module) {
                    modules.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Editor

        internal void SortModules() {
            Sort.Merge(modules, (a, b) => {
                var res = a.Enabled.CompareTo(b.Enabled);
                if(res != 0)
                    return res;
                return a.Update.CompareTo(b.Update);
            });
        }

        #endregion

        [System.Serializable]
        private class Module
        {
            #region Variables

            [SerializeField] private bool enabled = true;
            [SerializeField] private TacticalCameraUpdateState update = TacticalCameraUpdateState.Main;
            [SerializeField, TypeFilter(typeof(ITacticalCameraModule))] private UnityEngine.Object reference = null;
            [SerializeField] private bool createNewInstance = false;

            private UnityEngine.Object instantiated;
            private ITacticalCameraModule instance;

            #endregion

            #region Properties

            public bool Enabled {
                get => enabled;
                set => enabled = value;
            }

            public TacticalCameraUpdateState Update {
                get => update;
                set => update = value;
            }

            public ITacticalCameraModule Instance {
                get {
                    if(instance == null) {
                        if(createNewInstance && reference is ScriptableObject) {
                            instantiated = Instantiate(reference);
                            instance = instantiated as ITacticalCameraModule;
                        }
                        else
                            instance = reference as ITacticalCameraModule;
                    }
                    return instance;
                }
            }

            #endregion

            #region Constructor

            public Module(ITacticalCameraModule module) {
                this.instance = module;
            }

            public Module(ITacticalCameraModule module, TacticalCameraUpdateState update) {
                this.instance = module;
                this.update = update;
            }

            public Module(UnityEngine.Object reference) : this(reference, false, TacticalCameraUpdateState.Main) { }

            public Module(UnityEngine.Object reference, bool createNewInstance) : this(reference, createNewInstance, TacticalCameraUpdateState.Main) { }

            public Module(UnityEngine.Object reference, bool createNewInstance, TacticalCameraUpdateState update) {
                this.reference = reference;
                this.createNewInstance = createNewInstance && reference is ScriptableObject;
                this.update = update;
            }

            #endregion

            #region Methods

            public void Destroy() {
                if(createNewInstance && instantiated) {
                    GameObject.Destroy(instantiated);
                }
            }

            #endregion
        }
    }

    public enum TacticalCameraUpdateState
    {
        /// <summary>
        /// Disables modules and state between updates.
        /// </summary>
        None,
        /// <summary>
        /// Handles early update, used for input.
        /// </summary>
        Early,
        /// <summary>
        /// The main update loop.
        /// </summary>
        Main,
        /// <summary>
        /// Post late update calls, great for resolving outside bounds.
        /// </summary>
        Post,
    }

    public interface ITacticalCamera
    {
        // Update
        bool Enabled { get; set; }
        bool ManualUpdate { get; set; }
        TacticalCameraUpdateState UpdateState { get; }

        // Modules
        ITacticalCameraInput Input { get; }
        IReadOnlyList<ITacticalCameraModule> Modules { get; }

        // Multiplayer Support
        int PlayerId { get; set; }

        // References
        Transform Root { get; }
        Transform Pivot { get; }
        Transform CameraTransform { get; }
        Camera Camera { get; }

        // Methods
        void DoUpdate(float dt);
        void AddModule(ITacticalCameraModule module, TacticalCameraUpdateState update);
        void RemoveModule(ITacticalCameraModule module);
    }

    public interface ITacticalCameraModule
    {
        void UpdateModule(ITacticalCamera tc, float dt);
    }

    public interface ITacticalCameraInput
    {

    }
}
