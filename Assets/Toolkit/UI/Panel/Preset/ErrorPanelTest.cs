using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit.UI.PanelSystem.Preset;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class ErrorPanelTest : MonoBehaviour {

        [SerializeField] private string header;
        [SerializeField] private string content;

        private void Awake() {
            var add = GetComponent<IPanelAdd>();
            add.OnPanelOpen += OnPanelOpen;
        }

        private void OnPanelOpen(Panel panel) {
            var errorPanel = panel.GetComponent<Preset.ErrorPanel>();
            errorPanel.Show(header, content)
                .OnComplete += OnPressed;
        }

        private void OnPressed(ErrorPanel.UserAction value) {
            Debug.Log($"Used action presssed on error panel: " + value);
        }
    }
}
