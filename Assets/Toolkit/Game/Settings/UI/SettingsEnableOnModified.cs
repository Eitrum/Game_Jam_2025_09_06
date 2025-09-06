using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Game.Settings.UI {
    public class SettingsEnableOnModified : MonoBehaviour {
        #region Variables

        private SettingsBase settingsBase;
        [SerializeField] private bool invert = false;
        [SerializeField] private Transform targetContainer;

        #endregion

        #region Init

        private void Awake() {
            settingsBase = GetComponentInParent<SettingsBase>();
            settingsBase.OnIsDirtyUpdate += OnUpdate;
            if(targetContainer == null)
                targetContainer = transform;
        }

        #endregion

        #region Callbacks

        private void OnUpdate(InGameSettings.Entry entry) {
            var shouldBeActive = invert ? !entry.IsDirty : entry.IsDirty;
            targetContainer.SetActive(shouldBeActive);
        }

        #endregion
    }
}
