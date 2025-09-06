using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Game.Settings.UI {
    [DefaultExecutionOrder(500)]
    public class SettingsToggle : SettingsBase {
        #region Variables

        [SerializeField] private string group;
        [SerializeField] private string id;
        [SerializeField] private bool liveUpdate = false;

        [Header("Components")]
        [SerializeField] private Toggle toggle;

        private InGameSettings.Toggleable toggleable;

        #endregion

        #region Init

        private void Awake() {
            if(toggle == null)
                toggle = GetComponentInChildren<Toggle>();
            if(!InGameSettings.TryFindEntry(group, id, out var entryOutput)) {
                Debug.LogError(this.FormatLog($"Failed to find '{group}/{id}'"));
                return;
            }
            Entry = entryOutput;

            switch(Entry) {
                case InGameSettings.Toggleable togglableEntry:
                    this.toggleable = togglableEntry;
                    break;
                default:
                    Debug.LogError(this.FormatLog($"Entry '{group}/{id}' is not of a toggle type."));
                    break;
            }

            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable() {
            if(toggleable != null) {
                toggleable.OnModifiedValueChanged += UpdateToggle;
                toggle.SetIsOnWithoutNotify(toggleable.ModifiedValue);
            }
        }

        private void OnDisable() {
            if(toggleable != null)
                toggleable.OnModifiedValueChanged -= UpdateToggle;
        }

        #endregion

        #region Callbacks

        private void UpdateToggle(bool value) {
            toggle.SetIsOnWithoutNotify(value);
            InvokeUpdate();
        }

        private void OnValueChanged(bool value) {
            if(toggleable != null)
                toggleable.ModifiedValue = value;
            if(liveUpdate)
                Entry.ForceUpdate();
            InvokeUpdate();
        }

        #endregion
    }
}
