using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class HUDModule : MonoBehaviour {
        #region Variables

        private const string TAG = "[Toolkit.UI.PanelSystem.HUDModule] - ";

        [SerializeField, Readonly] private int moduleId;
        [SerializeField] private bool cache = false;
        [SerializeField] private Assignable.Search assignableSearch = Assignable.Search.Disable;
        private int uniqueId;

        // Used to keep track of panels created
        private static int uniqueIdGenerator;

        private PanelState cachedState = PanelState.None;
        private IPanelAnimation panelAnimation;
        private bool closeRequested = false;

        private static Dictionary<int, HUDModule> uniqueIdLookup = new Dictionary<int, HUDModule>();
        private HUD manager;
        private IUnit assignedUnit;

        public event System.Action OnShown;
        public event System.Action OnHidden;
        public event System.Action<bool> OnCloseRequested;
        public event System.Action<PanelState> OnStateChange;
        private System.Action<IUnit> onUnitChanged;

        #endregion

        #region Properties

        public int ModuleId => moduleId;
        public int UniqueId => uniqueId;
        public IUnit Unit => assignedUnit;
        public bool Cache => cache;
        public bool Visible {
            get => cachedState == PanelState.Showing;
            set {
                if(value)
                    Show();
                else
                    Hide();
            }
        }

        public bool IsClosing => closeRequested;
        public IPanelAnimation Animation => panelAnimation;
        public PanelState State {
            get => cachedState;
            private set {
                if(cachedState == value)
                    return;
                cachedState = value;
                OnStateChange?.Invoke(cachedState);
            }
        }

        public HUD ParentManager => manager;

        public event System.Action<IUnit> OnUnitChanged {
            add {
                onUnitChanged += value;
                if(this.assignedUnit != null)
                    value?.Invoke(this.assignedUnit);
            }
            remove {
                onUnitChanged -= value;
            }
        }

        #endregion

        #region Init

        private void Awake() {
            panelAnimation = GetComponent<IPanelAnimation>();
            uniqueId = ++uniqueIdGenerator;
            uniqueIdLookup.Add(uniqueId, this);
        }

        private void OnEnable() {
            if(!manager)
                manager = GetComponentInParent<HUD>();
            closeRequested = false;
        }

        private void OnDestroy() {
            State = PanelState.Destroyed;
            uniqueIdLookup.Remove(uniqueId);
        }

        public void ReAssignPanelManager() {
            manager = GetComponentInParent<HUD>();
        }

        #endregion

        #region Assign

        public void Assign(IUnit unit) {
            if(this.assignedUnit == unit)
                return;
            this.assignedUnit = unit;
            try {
                onUnitChanged?.Invoke(unit);
                Assignable.Assign(gameObject, unit, assignableSearch);
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Update

        private void LateUpdate() {
            switch(State) {
                case PanelState.Hiding: {
                        var complete = panelAnimation?.IsComplete ?? true;
                        if(complete) {
                            State = PanelState.None;
                            gameObject.SetActive(false);
                            OnHidden?.Invoke();
                            if(closeRequested) {
                                DestroyOrCache();
                                return;
                            }
                        }
                    }
                    break;
                case PanelState.Showing: {
                        var complete = panelAnimation?.IsComplete ?? true;
                        if(complete) {
                            State = PanelState.Ready;
                            OnShown?.Invoke();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Methods

        public void Show() {
            if(closeRequested)
                return;
            switch(State) {
                case PanelState.Ready:
                    return;
                case PanelState.Hiding:
                    panelAnimation?.Cancel();
                    break;
                case PanelState.None:
                    break;
                default:
                    Debug.Log(TAG + $"Attempting to show panel ('{name}') with a state of '{State}'.");
                    return;
            }

            gameObject.SetActive(true);
            State = PanelState.Showing;
            if(panelAnimation != null) {
                panelAnimation.Show();
                return;
            }
            else {
                State = PanelState.Ready;
                OnShown?.Invoke();
            }
        }

        public void Hide() {
            if(closeRequested)
                return;
            switch(State) {
                case PanelState.None:
                    gameObject.SetActive(false);
                    return;
                case PanelState.Showing:
                    panelAnimation?.Cancel();
                    break;
                case PanelState.Ready:
                    break;
                default:
                    Debug.Log(TAG + $"Attempting to hide panel ('{name}') with a state of '{State}'.");
                    return;
            }

            State = PanelState.Hiding;
            if(panelAnimation != null) {
                panelAnimation.Hide();
            }
            else {
                State = PanelState.None;
                gameObject.SetActive(false);
                OnHidden?.Invoke();
            }
        }

        public void Close(bool instant = false) {
            if(State == PanelState.None) {
                DestroyOrCache(instant);
                return;
            }
            if(instant) {
                gameObject.SetActive(false);
                State = PanelState.None;
                OnHidden?.Invoke();
                OnCloseRequested?.Invoke(instant);
                DestroyOrCache();
            }
            else {
                Hide();
                closeRequested = true;
                OnCloseRequested?.Invoke(instant);
                if(State == PanelState.None && gameObject && !gameObject.activeSelf) {
                    DestroyOrCache();
                }
            }
        }

        public void DestroyOrCache(bool instant = false) {
            if(!instant && State != PanelState.None) {
                Close();
                return;
            }
            if(this == null)
                return;
            closeRequested = false;
            var manager = GetComponentInParent<HUD>();
            if(manager == null)
                Destroy(gameObject);
            manager.DestroyOrCache(this);
        }

        #endregion

        #region Editor

        private void OnValidate() {
            if(Application.isPlaying)
                return;
            moduleId = name.GetHash32();
        }

        #endregion
    }
}
