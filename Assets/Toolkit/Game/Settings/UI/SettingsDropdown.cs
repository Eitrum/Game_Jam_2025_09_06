using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Game.Settings.UI {
    [DefaultExecutionOrder(500)]
    public class SettingsDropdown : SettingsBase {

        #region Variables

        [SerializeField] private string group;
        [SerializeField] private string id;
        [SerializeField] private bool liveUpdate = false;
        [SerializeField] private string[] nameOverrides = { };

        [Header("Components")]
        [SerializeField] private TMPro.TMP_Dropdown tmpDropdown;
        [SerializeField] private Dropdown dropdown;

        private InGameSettings.Entry<int> intWrapper;
        private InGameSettings.IEnumEntry enumEntry;
        private InGameSettings.IArrayEntry arrayEntry;

        #endregion

        #region Init

        private void Awake() {
            if(dropdown == null)
                dropdown = GetComponentInChildren<Dropdown>();
            if(tmpDropdown == null)
                tmpDropdown = GetComponentInChildren<TMPro.TMP_Dropdown>();

            if(!InGameSettings.TryFindEntry(group, id, out var entryOuput)) {
                Debug.LogError(this.FormatLog($"Failed to find '{group}/{id}'"));
                return;
            }

            Entry = entryOuput;

            switch(Entry) {
                case InGameSettings.IEnumEntry enumEntry:
                    this.enumEntry = enumEntry;
                    var names = FastEnum.GetNames(enumEntry.EnumType);
                    if(dropdown)
                        dropdown.options = names.Select(x => new Dropdown.OptionData(x.SplitPascalCase())).ToList();
                    if(tmpDropdown)
                        tmpDropdown.options = names.Select(x => new TMPro.TMP_Dropdown.OptionData(x.SplitPascalCase())).ToList();
                    break;
                case InGameSettings.IArrayEntry arrayEntry:
                    this.arrayEntry = arrayEntry;
                    if(dropdown)
                        dropdown.options = arrayEntry.Names.Select(x => new Dropdown.OptionData(x)).ToList();
                    if(tmpDropdown)
                        tmpDropdown.options = arrayEntry.Names.Select(x => new TMPro.TMP_Dropdown.OptionData(x)).ToList();
                    break;
                default:
                    Debug.LogError(this.FormatLog($"Entry '{group}/{id}' is not of a dropdown type."));
                    break;
            }

            if(nameOverrides != null && nameOverrides.Length > 0) {
                if(dropdown) {
                    var len = Mathf.Min(nameOverrides.Length, dropdown.options.Count);
                    for(int i = 0; i < len; i++) {
                        if(string.IsNullOrEmpty(nameOverrides[i]))
                            continue;
                        dropdown.options[i].text = nameOverrides[i];
                    }
                }
                if(tmpDropdown) {
                    var len = Mathf.Min(nameOverrides.Length, tmpDropdown.options.Count);
                    for(int i = 0; i < len; i++) {
                        if(string.IsNullOrEmpty(nameOverrides[i]))
                            continue;
                        tmpDropdown.options[i].text = nameOverrides[i];
                    }
                }
            }

            intWrapper = Entry as InGameSettings.Entry<int>;
            if(dropdown) {
                dropdown.SetValueWithoutNotify(intWrapper.ModifiedValue);
                dropdown.onValueChanged.AddListener(OnValueChanged);
            }
            if(tmpDropdown) {
                tmpDropdown.SetValueWithoutNotify(intWrapper.ModifiedValue);
                tmpDropdown.onValueChanged.AddListener(OnValueChanged);
            }
        }

        private void OnEnable() {
            intWrapper.OnModifiedValueChanged += UpdateDropdown;
            if(dropdown) {
                dropdown.SetValueWithoutNotify(intWrapper.ModifiedValue);
            }
            if(tmpDropdown) {
                tmpDropdown.SetValueWithoutNotify(intWrapper.ModifiedValue);
            }
        }

        private void OnDisable() {
            intWrapper.OnModifiedValueChanged -= UpdateDropdown;
        }

        #endregion

        #region Callbacks

        private void UpdateDropdown(int value) {
            if(dropdown)
                dropdown.SetValueWithoutNotify(value);
            if(tmpDropdown)
                tmpDropdown.SetValueWithoutNotify(value);
            InvokeUpdate();
        }

        private void OnValueChanged(int value) {
            intWrapper.ModifiedValue = value;
            if(liveUpdate)
                Entry.ForceUpdate();
            InvokeUpdate();
        }

        #endregion
    }
}
