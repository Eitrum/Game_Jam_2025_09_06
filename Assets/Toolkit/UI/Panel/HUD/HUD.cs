using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class HUD : MonoBehaviour, IAssignable<IUnit> {
        #region Variables

        private const string TAG = "[Toolkit.UI.PanelSystem.HUD] - ";
        [SerializeField] private HUDPreset preset;
        private CanvasGroup group;
        private RectTransform cacheContainer;
        private List<HUDModule> incache = new List<HUDModule>();
        private List<HUDModule> modules = new List<HUDModule>();
        private IUnit unit;
        private HUDPreset currentPreset;

        #endregion

        #region Properties

        public static HUD Main => PanelManager.Main.GetHUD();
        public IUnit Unit => unit;

        #endregion

        private void Awake() {
            group = this.GetOrAddComponent<CanvasGroup>();
            if(modules == null || modules.Count == 0)
                GetComponentsInChildren(modules);
            if(cacheContainer == null) {
                cacheContainer = transform.ToRectTransform().CreateChildRectTransform("Cache");
                cacheContainer.gameObject.SetActive(false);
                cacheContainer.SetSiblingIndex(0);
            }
        }

        private void Start() {
            if(preset != null && currentPreset == null) {
                SetPreset(preset);
            }
        }

        public void Assign(IUnit unit) {
            this.unit = unit;
            foreach(var m in modules)
                m.Assign(unit);
        }

        public void Clear() {
            var tmod = modules.ToArray();
            for(int i = tmod.Length - 1; i >= 0; i--) {
                if(tmod[i] == null)
                    continue;
                DestroyOrCache(tmod[i]);
            }
            currentPreset = null;
        }

        public void DestroyOrCache(HUDModule module) {
            if(module == null)
                return;
            if(!modules.Remove(module))
                return;
            if(module.Cache) {
                module.transform.SetParent(cacheContainer);
                incache.Add(module);
            }
            else
                Destroy(module.gameObject);
        }

        #region Preset

        public void SetPreset(HUDPreset preset) {
            if(preset == null)
                return;
            if(preset == currentPreset)
                return;
            Clear();
            foreach(var p in preset.Prefabs)
                GetOrAdd(p);
        }

        public void AddPreset(HUDPreset preset) {
            if(preset == null) return;
            foreach(var p in preset.Prefabs)
                GetOrAdd(p);
        }

        public void UseDefaultPreset() {
            SetPreset(currentPreset);
        }

        #endregion

        #region Add / Remove

        public HUDModule GetOrAdd(HUDModule module)
            => GetOrAdd(module.gameObject);

        public HUDModule GetOrAdd(GameObject panelPrefab) {
            if(panelPrefab == null) {
                Debug.LogError(TAG + "Panel prefab is null");
                return null;
            }
            var hudModuleOfPrefab = panelPrefab.GetComponent<HUDModule>();
            if(hudModuleOfPrefab == null) {
                Debug.LogError(TAG + $"Panel prefab ('{panelPrefab.name}') don't have a HUDModule!");
                return null;
            }
            var moduleid = hudModuleOfPrefab.ModuleId;
            for(int i = modules.Count - 1; i >= 0; i--) {
                if(modules[i].ModuleId == moduleid)
                    return modules[i];
            }
            return AddModule(panelPrefab);
        }

        public HUDModule AddModule(HUDModule module)
            => AddModule(module.gameObject);

        public HUDModule AddModule(GameObject panelPrefab) {
            if(panelPrefab == null) {
                Debug.LogError(TAG + "Panel prefab is null");
                return null;
            }
            var hudModuleOfPrefab = panelPrefab.GetComponent<HUDModule>();
            if(hudModuleOfPrefab == null) {
                Debug.LogError(TAG + $"Panel prefab ('{panelPrefab.name}') don't have a HUDModule!");
                return null;
            }

            var moduleid = hudModuleOfPrefab.ModuleId;
            if(!incache.Extract(x => x.ModuleId == moduleid, out HUDModule hudmodule) || hudmodule == null) {
                var go = Instantiate(panelPrefab, transform);
                hudmodule = go.GetComponent<HUDModule>();
            }
            else
                hudmodule.transform.SetParent(transform, false);

            modules.Add(hudmodule);
            if(unit != null)
                hudmodule.Assign(unit);
            return hudmodule;
        }

        public void RemoveModule(HUDModule module)
            => RemoveModule(module.ModuleId);

        public void RemoveModule(int moduleId) {
            for(int i = modules.Count - 1; i >= 0; i--) {
                if(modules[i].ModuleId == moduleId)
                    DestroyOrCache(modules[i]);
            }
        }

        #endregion

        #region Show / Hide

        public void ShowAllVisible() {
            if(!group)
                return;
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public void Hide(HUDModule module) {

        }

        public void HideAll() {
            if(!group)
                return;
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        #endregion
    }
}
