using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Game.Settings.UI {
    [AddComponentMenu("Toolkit/Game/Settings/UI/Apply OnClick (Button)")]
    [RequireComponent(typeof(Button))]
    public class SettingsApplyOnClickButton : MonoBehaviour {
        #region Variables

        [SerializeField] private SettingsOperationType operationType = SettingsOperationType.Apply;
        [SerializeField] private string groupName;
        [SerializeField] private bool triggerSave = true;
        private Button button;
        private SettingsBase settingsBase;

        #endregion

        #region Init

        private void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        #endregion

        #region Callback

        private void OnClick() {
            if(settingsBase) {
                settingsBase.Do(operationType);
                return;
            }
            switch(operationType) {
                case SettingsOperationType.Apply:
                    InGameSettings.ApplyChanges(groupName, triggerSave);
                    break;
                case SettingsOperationType.Discard:
                    InGameSettings.DiscardChanges(groupName);
                    break;
                case SettingsOperationType.Reset:
                    InGameSettings.Reset(groupName);
                    break;
            }
        }

        #endregion
    }
}
