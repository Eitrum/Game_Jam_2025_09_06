using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Game.Settings.UI {
    public class SettingsTextField : SettingsBase {
        #region Variables

        [SerializeField] private string group;
        [SerializeField] private string id;
        [SerializeField] private bool liveUpdate = false;

        [Header("Components")]
        [SerializeField] private TextField valueField;
        [SerializeField] private UnityEngine.UI.InputField input;
        [SerializeField] private TMPro.TMP_InputField tmpInput;

        private InGameSettings.Entry<string> textEntry;

        #endregion

        #region Init

        private void Awake() {
            if(!valueField.IsValid)
                valueField = TextField.FindInChildren(this);

            if(!InGameSettings.TryFindEntry(group, id, out var entryOutput)) {
                Debug.LogError(this.FormatLog($"Failed to find '{group}/{id}'"));
                return;
            }

            Entry = entryOutput;

            switch(Entry) {
                case InGameSettings.Entry<string> textEntry:
                    this.textEntry = textEntry;
                    break;
                default:
                    Debug.LogError(this.FormatLog($"Entry '{group}/{id}' is not of a range type."));
                    break;
            }

            if(input)
                input.onValueChanged.AddListener(OnValueChanged);
            if(tmpInput)
                tmpInput.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable() {
            if(textEntry == null)
                return;
            if(input) {
                textEntry.OnModifiedValueChanged += UpdateInput;
                input.SetTextWithoutNotify(textEntry.ModifiedValue);
            }
            if(tmpInput) {
                textEntry.OnModifiedValueChanged += UpdateTMPInput;
                tmpInput.SetTextWithoutNotify(textEntry.ModifiedValue);
            }
        }

        private void OnDisable() {
            if(textEntry == null)
                return;
            if(input) {
                textEntry.OnModifiedValueChanged -= UpdateInput;
            }
            if(tmpInput) {
                textEntry.OnModifiedValueChanged -= UpdateTMPInput;
            }
        }

        #endregion

        #region Callbacks

        private void UpdateInput(string value) {
            if(input)
                input.SetTextWithoutNotify(value);
            InvokeUpdate();
        }

        private void UpdateTMPInput(string value) {
            if(tmpInput)
                tmpInput.SetTextWithoutNotify(value);
            InvokeUpdate();
        }

        private void OnValueChanged(string value) {
            if(textEntry != null)
                textEntry.ModifiedValue = value;
            if(liveUpdate)
                Entry.ForceUpdate();
            InvokeUpdate();
        }

        #endregion
    }
}
