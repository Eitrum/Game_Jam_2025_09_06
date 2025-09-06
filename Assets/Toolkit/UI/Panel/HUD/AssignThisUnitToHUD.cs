using System.Collections;
using System.Collections.Generic;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit.UI.PanelSystem.Components {
    public class AssignThisUnitToHUD : MonoBehaviour {

        #region Variables

        [SerializeField] private bool onStart = true;
        [SerializeField] private bool onEnable = false;

        #endregion

        #region Init

        private void Start() {
            if(!onStart)
                return;
            var unit = GetComponent<IUnit>();
            if(HUD.Main)
                HUD.Main.Assign(unit);
        }

        private void OnEnable() {
            if(!onEnable)
                return;
            var unit = GetComponent<IUnit>();
            if(HUD.Main)
                HUD.Main.Assign(unit);
        }

        #endregion
    }
}
