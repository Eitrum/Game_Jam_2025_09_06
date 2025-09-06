using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem {
    public class Panel : MonoBehaviour {
        #region Variables

        private const string TAG = "[Panel] - ";

        [SerializeField, Readonly] private int panelId;
        private int uniqueId;

        // Used to keep track of panels created
        private static int uniqueIdGenerator;

        private PanelState cachedState = PanelState.None;
        private PanelConfig config;
        private IPanelAnimation panelAnimation;
        private bool closeRequested = false;

        private static Dictionary<int, Panel> uniqueIdLookup = new Dictionary<int, Panel>();
        private PanelManager manager;

        public event System.Action OnShown;
        public event System.Action OnHidden;
        public event System.Action<bool> OnCloseRequested;
        public event System.Action<PanelState> OnStateChange;

        #endregion

        #region Properties

        public int PanelId => panelId;
        public int UniqueId => uniqueId;

        public bool IsClosing => closeRequested;
        public PanelConfig Config => config;
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

        public PanelManager ParentManager => manager;

        public int Index => manager != null ? manager.GetIndex(this) : -1;
        public bool IsOnTop {
            get {
                if(manager == null) {
                    var childCount = transform.parent.childCount;
                    var index = transform.GetSiblingIndex();
                    return (index - 1 == childCount);
                }
                else {
                    return manager.Panels.Last() == this;
                }
            }
            set {
                if(IsOnTop)
                    return;
                transform.SetAsLastSibling();
                if(manager == null)
                    return;
                manager.RecalculatePanels();
            }
        }

        #endregion

        #region Init

        private void Awake() {
            config = this.GetOrAddComponent<PanelConfig>();
            panelAnimation = GetComponent<IPanelAnimation>();
            uniqueId = ++uniqueIdGenerator;
            uniqueIdLookup.Add(uniqueId, this);
        }

        private void OnEnable() {
            if(!manager)
                manager = GetComponentInParent<PanelManager>();
            closeRequested = false;
        }

        private void OnDestroy() {
            State = PanelState.Destroyed;
            uniqueIdLookup.Remove(uniqueId);
        }

        public void ReAssignPanelManager() {
            manager = GetComponentInParent<PanelManager>();
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
            var manager = GetComponentInParent<PanelManager>();
            if(manager == null)
                Destroy(gameObject);
            manager.DestroyOrCache(this);
        }

        #endregion

        #region Assignable
        
        public void Assign<T>(T item, Assignable.Search search = Assignable.Search.Default)
            => Assignable.Assign(gameObject, item, search);

        public void Assign<T0, T1>(T0 item0, T1 item1, Assignable.Search search = Assignable.Search.Default)
            => Assignable.Assign(gameObject, item0, item1, search);

        #endregion

        #region Editor

        private void OnValidate() {
            if(Application.isPlaying)
                return;
            panelId = name.GetHash32();
        }

        #endregion
    }
}
