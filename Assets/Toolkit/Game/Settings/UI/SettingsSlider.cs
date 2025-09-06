using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Game.Settings.UI {
    [DefaultExecutionOrder(500)]
    public class SettingsSlider : SettingsBase {

        #region Variables

        [SerializeField] private string group;
        [SerializeField] private string id;
        [SerializeField] private bool liveUpdate = false;

        [Header("Components")]
        [SerializeField] private Slider slider;
        [SerializeField] private TextField valueField;

        private InGameSettings.RangeEntry rangeEntry;
        private InGameSettings.IntRangeEntry intRangeEntry;

        #endregion

        #region Init

        private void Awake() {
            if(slider == null)
                slider = GetComponentInChildren<Slider>();
            if(!InGameSettings.TryFindEntry(group, id, out var entryOutput)) {
                Debug.LogError(this.FormatLog($"Failed to find '{group}/{id}'"));
                return;
            }

            Entry = entryOutput;

            switch(Entry) {
                case InGameSettings.RangeEntry rangeEntry:
                    slider.minValue = rangeEntry.Min;
                    slider.maxValue = rangeEntry.Max;
                    slider.value = rangeEntry.ModifiedValue;
                    slider.wholeNumbers = false;
                    this.rangeEntry = rangeEntry;
                    break;
                case InGameSettings.IntRangeEntry intRangeEntry:
                    slider.minValue = intRangeEntry.Min;
                    slider.maxValue = intRangeEntry.Max;
                    slider.value = intRangeEntry.ModifiedValue;
                    slider.wholeNumbers = true;
                    this.intRangeEntry = intRangeEntry;
                    break;
                default:
                    Debug.LogError(this.FormatLog($"Entry '{group}/{id}' is not of a range type."));
                    break;
            }

            slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable() {
            if(rangeEntry != null) {
                rangeEntry.OnModifiedValueChanged += UpdateSlider;
                slider.SetValueWithoutNotify(rangeEntry.ModifiedValue);
            }
            if(intRangeEntry != null) {
                intRangeEntry.OnModifiedValueChanged += UpdateIntSlider;
                slider.SetValueWithoutNotify(intRangeEntry.ModifiedValue);
            }
        }

        private void OnDisable() {
            if(rangeEntry != null) {
                rangeEntry.OnModifiedValueChanged -= UpdateSlider;
            }
            if(intRangeEntry != null) {
                intRangeEntry.OnModifiedValueChanged -= UpdateIntSlider;
            }
        }

        #endregion

        #region Callbacks

        private void UpdateIntSlider(int value) {
            slider.SetValueWithoutNotify(value);
            InvokeUpdate();
        }

        private void UpdateSlider(float value) {
            slider.SetValueWithoutNotify(value);
            InvokeUpdate();
        }

        private void OnValueChanged(float value) {
            if(rangeEntry != null)
                rangeEntry.ModifiedValue = value;
            if(intRangeEntry != null)
                intRangeEntry.ModifiedValue = Mathf.RoundToInt(value);
            if(liveUpdate)
                Entry.ForceUpdate();
            InvokeUpdate();
        }

        #endregion
    }
}
