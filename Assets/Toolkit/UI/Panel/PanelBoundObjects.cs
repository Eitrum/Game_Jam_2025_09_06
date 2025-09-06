using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolkit;

namespace Toolkit.UI.PanelSystem {
    public class PanelBoundObjects : MonoBehaviour {

        public interface IBound {
            void Show();
            void Hide();
            void Destroy();
        }

        #region Variables

        [SerializeField] private List<UnityEngine.Object> objects = new List<Object>();
        private List<IBound> boundObjects = new List<IBound>();
        private Panel panel;

        #endregion

        #region Init

        private void Awake() {
            panel = GetComponent<Panel>();
            panel.OnCloseRequested += OnCloseRequested;
            panel.OnStateChange += OnStateChange;
        }

        private void OnStateChange(PanelState state) {
            switch(state) {
                case PanelState.Ready:
                    ShowAll();
                    break;
                case PanelState.None:
                    HideAll();
                    break;
            }
        }

        #endregion

        #region Add / Remove

        public void Bind(IBound bound) {
            this.boundObjects.Add(bound);
        }

        public bool Unbind(IBound bound) {
            return this.boundObjects.Remove(bound);
        }

        public void Bind(UnityEngine.Object obj) {
            if(obj is IBound ibound)
                Bind(ibound);
            else
                objects.Add(obj);
        }

        public bool Unbind(UnityEngine.Object obj) {
            if(obj is IBound ibound)
                return Unbind(ibound);
            else
                return objects.Remove(obj);
        }

        #endregion

        #region Util

        public void ShowAll() {
            foreach(var o in objects) {
                if(o is Component comp)
                    comp.gameObject.SetActive(true);
                else if(o is GameObject go)
                    go.SetActive(true);
            }

            foreach(var bo in boundObjects)
                bo?.Show();
        }

        public void HideAll() {
            foreach(var o in objects) {
                if(o is Component comp)
                    comp.gameObject.SetActive(false);
                else if(o is GameObject go)
                    go.SetActive(false);
            }

            foreach(var bo in boundObjects)
                bo?.Hide();
        }

        public void DestroyAll() {
            foreach(var o in objects) {
                if(o is Component comp)
                    Destroy(comp.gameObject);
                else if(o is GameObject go)
                    Destroy(go);
            }

            foreach(var bo in boundObjects)
                bo?.Destroy();
        }

        #endregion

        #region Callbacks

        private void OnCloseRequested(bool obj) {
            DestroyAll();
        }

        #endregion


    }

    public static partial class PanelExtensions {

        #region Bind

        public static void Bind(this Panel panel, PanelBoundObjects.IBound bound) {
            var pbo = panel.GetOrAddComponent<PanelBoundObjects>();
            pbo.Bind(bound);
        }

        public static void Bind(this Panel panel, UnityEngine.Object bound) {
            var pbo = panel.GetOrAddComponent<PanelBoundObjects>();
            pbo.Bind(bound);
        }

        #endregion

        #region Unbind

        public static bool Unbind(this Panel panel, PanelBoundObjects.IBound bound) {
            var pbo = panel.GetOrAddComponent<PanelBoundObjects>();
            return pbo.Unbind(bound);
        }

        public static bool Unbind(this Panel panel, UnityEngine.Object bound) {
            var pbo = panel.GetOrAddComponent<PanelBoundObjects>();
            return pbo.Unbind(bound);
        }

        #endregion
    }
}
