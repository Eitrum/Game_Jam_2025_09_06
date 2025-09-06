using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class AddHUDOnStart : MonoBehaviour {
        [SerializeField] private HUDModule hudPrefab;

        private void Start() {
            HUD.Main.AddModule(hudPrefab);
        }
    }
}
