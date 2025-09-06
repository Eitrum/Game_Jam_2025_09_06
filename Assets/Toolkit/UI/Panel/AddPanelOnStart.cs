using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class AddPanelOnStart : MonoBehaviour {
        [SerializeField] private GameObject panelPrefab;

        [ContextMenu("Run Start")]
        [Button]
        private void Start() {
            var manager = GetComponentInParent<PanelManager>();
            if(manager || PanelManager.TryGet(0, out manager))
                manager.Add(panelPrefab);
        }
    }
}
