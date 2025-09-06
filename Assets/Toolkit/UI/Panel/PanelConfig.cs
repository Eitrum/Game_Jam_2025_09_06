using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI.PanelSystem {
    public class PanelConfig : MonoBehaviour {
        #region Variables

        [SerializeField] private PanelMode mode = PanelMode.Additative;
        [SerializeField] private PanelBackgroundMode background = PanelBackgroundMode.DisplayAndClose;
        [SerializeField] private bool keepCached = false;

        #endregion

        #region Properties

        public PanelMode Mode => mode;
        public PanelBackgroundMode Background => background;
        public bool KeepCached => keepCached;

        #endregion
    }
}
