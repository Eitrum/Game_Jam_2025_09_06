using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Toolkit.UI.PanelSystem {
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class PanelAddOnButtonClick : MonoBehaviour, IPanelAdd {
        #region Variables

        [SerializeField] private GameObject panelPrefab;
        private UnityEngine.UI.Button button;
        public event System.Action<Panel> OnPanelOpen;

        #endregion

        #region Init

        void Awake() {
            button = GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(OnClick);
        }

        [ContextMenu("Click")]
        private void OnClick() {
            Open();
        }

        #endregion

        #region IPanelAdd

        public Promise<Panel> Open() {
            var p = new Promise<Panel>();
            var manager = GetComponentInParent<PanelManager>();
            var panel = manager.Add(panelPrefab);
            if(panel == null) {
                return p.ErrorAndReturn("failed to add panel");
            }
            p.Complete(panel);
            OnPanelOpen?.Invoke(panel);
            return p;
        }

        #endregion
    }
}
