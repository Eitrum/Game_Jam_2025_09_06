using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Toolkit.UI.PanelSystem {
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class PanelCloseOnButtonClick : MonoBehaviour {
        #region Variables

        private UnityEngine.UI.Button button;

        #endregion

        #region Init

        private void Awake() {
            button = GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(OnClick);
        }

        [ContextMenu("Click")]
        public void OnClick() {
            if(!Application.isPlaying)
                return;
            var panel = GetComponentInParent<Panel>();
            if(panel)
                panel.Close();
        }

        #endregion
    }
}
