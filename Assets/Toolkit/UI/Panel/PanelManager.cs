using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem {
    public class PanelManager : MonoBehaviour {

        #region Instances

        private static List<PanelManager> managers = new List<PanelManager>();

        public static bool TryGet(int index, out PanelManager panelManager) {
            if(index < 0 || index >= managers.Count) {
                panelManager = null;
                return false;
            }
            panelManager = managers[index];
            return true;
        }
        public static int InstanceCount => managers.Count;
        public static bool Exists => managers.Count > 0;

        public static PanelManager Main => TryGet(0, out var main) ? main : null;

        #endregion

        #region Variables

        private const string TAG = "[Panel] - ";

        [Header("Config")]
        [SerializeField, Readonly(true)] private bool isGlobalInstance = false;

        [Header("Components")]
        [SerializeField] private HUD hud;
        [SerializeField] private RectTransform cacheContainer;
        [SerializeField] private RectTransform container;
        [SerializeField] private PanelBackground background;

        [Header("Loading")]
        [SerializeField] private LoadingOverlay loadingOverlay;


        private List<Panel> incache = new List<Panel>();
        private List<Panel> panels = new List<Panel>();
        private int containerChildCount = 0;
        private float isFocusedDelayCache = 0;

        #endregion

        #region Properties

        public IReadOnlyList<Panel> Panels => panels;
        public IReadOnlyList<Panel> InCache => incache;
        public HUD HUD => hud;

        #endregion

        #region Init

        private void Awake() {
            if(isGlobalInstance)
                managers.Add(this);
            if(background)
                background.OnClicked += OnBackgroundClicked;

            foreach(Transform c in container) {
                var p = c.GetComponent<Panel>();
                if(p == null)
                    continue;
                panels.Add(p);
            }
            if(cacheContainer == null) {
                cacheContainer = transform.ToRectTransform().CreateChildRectTransform("Cache");
                cacheContainer.gameObject.SetActive(false);
                cacheContainer.SetSiblingIndex(0);
            }

            if(loadingOverlay)
                loadingOverlay.Hide();

            if(panels.Count > 1) {
                int destroyIndex = -1;
                for(int i = panels.Count - 1; i >= 0; i--) {
                    if(i < destroyIndex)
                        Destroy(panels[i].gameObject);
                    var cfg = panels[i].Config;
                    if(cfg.Mode == PanelMode.Replace)
                        destroyIndex = i;
                }
                if(destroyIndex > 0)
                    panels.RemoveRange(0, destroyIndex - 1);
            }

            foreach(var p in panels)
                p.gameObject.SetActive(false);

            RecalculatePanels();
        }

        private void OnDestroy() {
            managers.Remove(this);
        }

        private void OnBackgroundClicked() {
            if(!Application.isFocused || isFocusedDelayCache > 0)
                return;
            var last = panels.Last();
            if(last != null && last.State != PanelState.Hiding && last.Config.Background == PanelBackgroundMode.DisplayAndClose)
                last.Close();
        }

        #endregion

        #region Update

        private void LateUpdate() {
            isFocusedDelayCache = Mathf.Clamp(isFocusedDelayCache + (Application.isFocused ? -Time.deltaTime : Time.deltaTime), 0, 0.2f);

            if(containerChildCount != container.childCount)
                RecalculatePanels();

            var last = panels.Last();
            if(last == null)
                return;
            switch(last.State) {
                case PanelState.Hiding:

                    break;
            }
        }

        #endregion

        #region Global Wrappers

        public static Panel AddToMain(GameObject panelPrefab) {
            if(TryGet(0, out var manager))
                return manager.Add(panelPrefab);
            else
                Debug.LogWarning(TAG + "No panel exist");
            return null;
        }

        public static Panel GetExistingOrAddToMain(GameObject panelPrefab) {
            if(TryGet(0, out var manager))
                return manager.GetExistingOrAdd(panelPrefab);
            else
                Debug.LogWarning(TAG + "No panel exist");
            return null;
        }

        #endregion

        #region Add

        public Promise<Panel> GetExistingOrAdd(string panelName, bool loading = true) {
            if(string.IsNullOrEmpty(panelName)) {
                Debug.LogError(TAG + "Panel name is null");
                return null;
            }

            for(int i = panels.Count - 1; i >= 0; i--) {
                if(panels[i].name == panelName)
                    return new Promise<Panel>().Complete(panels[i]);
            }

            return Add(panelName, loading);
        }

        public Panel GetExistingOrAdd(GameObject panelPrefab) {
            if(panelPrefab == null) {
                Debug.LogError(TAG + "Panel prefab is null");
                return null;
            }
            var panelOfPrefab = panelPrefab.GetComponent<Panel>();
            if(panelOfPrefab == null) {
                Debug.LogError(TAG + $"Panel prefab ('{panelPrefab.name}') don't have a panel script attached!");
                return null;
            }
            var panelid = panelOfPrefab.PanelId;
            for(int i = panels.Count - 1; i >= 0; i--) {
                if(panels[i].PanelId == panelid)
                    return panels[i];
            }
            return Add(panelPrefab);
        }

        public Promise<Panel> Add(string panelName, bool loading = true) {
            Promise<Panel> panelPromise = new Promise<Panel>();
            if(loading)
                SetLoading(true);

            if(!PanelStorage.TryGetPanelPrefab(panelName, out var promise, true)) {
                panelPromise.Error("panel not found");
                if(loading)
                    SetLoading(false);
                return panelPromise;
            }

            promise
                .SetOnComplete((go) => {
                    var panel = Add(go);
                    if(panel == null)
                        panelPromise.Error();
                    else
                        panelPromise.Complete(panel);
                    if(loading)
                        SetLoading(false);
                })
                .SetOnError((err) => {
                    panelPromise.Error(err);
                    if(loading)
                        SetLoading(false);
                });

            return panelPromise;
        }

        public Panel Add(GameObject panelPrefab) {
            if(panelPrefab == null) {
                Debug.LogError(TAG + "Panel prefab is null");
                return null;
            }
            var panelOfPrefab = panelPrefab.GetComponent<Panel>();
            if(panelOfPrefab == null) {
                Debug.LogError(TAG + $"Panel prefab ('{panelPrefab.name}') don't have a panel script attached!");
                return null;
            }
            var panelid = panelOfPrefab.PanelId;
            if(!incache.Extract(x => x.PanelId == panelid, out Panel panel) || panel == null) {
                var go = Instantiate(panelPrefab, container);
                go.name = panelPrefab.name;
                panel = go.GetComponent<Panel>();
            }
            else
                panel.transform.SetParent(container, false);

            var cfg = panel.Config;
            switch(cfg.Mode) {
                case PanelMode.Replace:
                    for(int i = panels.Count - 1; i >= 0; i--) {
                        DestroyOrCache(panels[i]);
                    }
                    panels.Clear();
                    break;
                case PanelMode.AdditativeHideBehind:
                    panel.gameObject.SetActive(false);
                    break;
            }
            panels.Add(panel);
            panel.ReAssignPanelManager();
            RecalculatePanels();
            return panel;
        }

        #endregion

        #region Remove

        public void CloseAll(bool instant = false) {
            for(int i = panels.Count - 1; i >= 0; i--) {
                Close(panels[i], instant);
            }
        }

        public void CloseById(int panelId, bool instant = false) {
            for(int i = panels.Count - 1; i >= 0; i--) {
                if(panels[i].PanelId == panelId)
                    Close(panels[i], instant);
            }
        }

        public void Close(Panel panel, bool instant) {
            if(instant) {
                if(!panels.Remove(panel))
                    return;
                panel.Close(instant);
                DestroyOrCacheWithoutListRemoval(panel);
            }
            else
                panel.Close();
        }

        #endregion

        #region DestroyOrCache

        public void DestroyOrCache(Panel panel) {
            if(panel == null)
                return;
            if(!panels.Remove(panel))
                return;
            DestroyOrCacheWithoutListRemoval(panel);
        }

        private void DestroyOrCacheWithoutListRemoval(Panel panel) {
            if(panel == null)
                return;

            if(panel.Config.KeepCached) {
                incache.Add(panel);
                panel.transform.SetParent(cacheContainer, false);
            }
            else
                Destroy(panel.gameObject);
        }

        #endregion

        #region HUD

        private void InitializeHUD() {
            if(this.hud != null)
                return;
            var hudContainer = transform.ToRectTransform().CreateChildRectTransform("HUD");
            hudContainer.gameObject.SetActive(false);
            hudContainer.SetSiblingIndex(0);
            this.hud = hudContainer.GetOrAddComponent<HUD>();
        }

        public HUD GetHUD() {
            InitializeHUD();
            return this.hud;
        }

        public HUDModule AddHUDModule(GameObject panelPrefab) {
            InitializeHUD();
            return this.hud.AddModule(panelPrefab);
        }

        #endregion

        #region Util

        public int GetIndex(Panel panel)
            => panels.IndexOf(panel);

        public bool IsLoading()
            => loadingOverlay != null && loadingOverlay.IsLoading;

        public void SetLoading(bool active) {
            if(loadingOverlay)
                loadingOverlay.SetOverlayActive(active);
        }

        [Button]
        public void ClearLoading() {
            if(loadingOverlay)
                loadingOverlay.SetOverlayActive(false, 99999);
        }

        public Panel GetTop()
            => panels.Last();

        public Panel GetBottom()
            => panels.Count > 0 ? panels[0] : null;

        public bool HasPanel(System.Func<Panel, bool> search)
            => HasPanel(search, out Panel panel);

        public bool HasPanel(System.Func<Panel, bool> search, out Panel panel) {
            for(int i = panels.Count - 1; i >= 0; i--) {
                if(search(panels[i])) {
                    panel = panels[i];
                    return true;
                }
            }
            panel = null;
            return false;
        }

        #endregion

        #region Recalculate

        public void RecalculatePanels() {
            RecalculatePanels_Nullcheck();
            RecalculatePanels_ShowHide();
            RecalculatePanels_Background();
            containerChildCount = container.childCount;
        }

        private void RecalculatePanels_Nullcheck() {
            for(int i = panels.Count - 1; i >= 0; i--)
                if(panels[i] == null)
                    panels.RemoveAt(i);

            for(int i = incache.Count - 1; i >= 0; i--) {
                if(incache[i] == null)
                    incache.ReplaceWithLastAndRemove(i);
            }
        }

        private void RecalculatePanels_ShowHide() {
            if(panels.Count == 0) {
                if(hud)
                    hud.ShowAllVisible();
                return;
            }

            int hideIndex = -1;
            for(int i = panels.Count - 1; i >= 0; i--) {
                if(panels[i].IsClosing)
                    continue;
                if(i > hideIndex) {
                    if(panels[i].State != PanelState.Ready)
                        panels[i].Show();
                }
                else {
                    if(panels[i].State != PanelState.None)
                        panels[i].Hide();
                }
                if(panels[i].Config.Mode == PanelMode.AdditativeHideBehind)
                    hideIndex = i;
            }
            if(hud) {
                if(hideIndex >= 0)
                    hud?.HideAll();
                else
                    hud?.ShowAllVisible();
            }
        }

        private void RecalculatePanels_Background() {
            if(background == null)
                return;
            if(panels.Count == 0)
                background.Hide();

            for(int i = panels.Count - 1; i >= 0; i--) {
                var p = panels[i];
                if(!p.gameObject.activeSelf)
                    continue;

                if(p.Config.Background == PanelBackgroundMode.None)
                    continue;

                background.transform.SetSiblingIndex(i);
                background.Show();
                return;
            }

            background.Hide();
        }

        #endregion
    }
}
