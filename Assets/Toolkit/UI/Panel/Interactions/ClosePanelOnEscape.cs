using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class ClosePanelOnEscape : MonoBehaviour {

        private Panel panel;
        private int frameAdded;
        private static int frameRemoved;

        private void Awake() {
            panel = GetComponentInParent<Panel>();
            frameAdded = Time.frameCount;
        }

        void LateUpdate() {
            if(panel == null)
                return;
            if(!panel.IsOnTop)
                return;

            if(frameAdded == Time.frameCount)
                return;

            if(frameRemoved == Time.frameCount)
                return;

            if(Input.GetKeyDown(KeyCode.Escape)) {
                panel.Close();
                frameRemoved = Time.frameCount;
            }
        }
    }
}
