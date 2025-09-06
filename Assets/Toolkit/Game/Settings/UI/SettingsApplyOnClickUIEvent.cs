using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Toolkit.Game.Settings.UI {
    [AddComponentMenu("Toolkit/Game/Settings/UI/Apply OnClick (UI Event)")]
    public class SettingsApplyOnClickUIEvent : Selectable, IPointerDownHandler, IPointerClickHandler, IInitializePotentialDragHandler, IDragHandler {
        #region Variables

        [SerializeField] private SettingsOperationType operationType = SettingsOperationType.Apply;
        [SerializeField] private Toolkit.UI.ButtonMode mode = Toolkit.UI.ButtonMode.PointerDown;
        [SerializeField] private string groupName;
        [SerializeField] private bool triggerSave = true;
        private SettingsBase settingsBase;

        #endregion

        protected override void OnEnable() {
            base.OnEnable();
            if(settingsBase == null)
                settingsBase = GetComponentInParent<SettingsBase>();
        }

        #region Callback

        public void OnPointerClick(PointerEventData eventData) {
            if(eventData.used)
                return;
            if(mode == Toolkit.UI.ButtonMode.Default)
                OnClick();
            eventData.Use();
        }

        public new void OnPointerDown(PointerEventData eventData) {
            if(eventData.used)
                return;
            if(mode == Toolkit.UI.ButtonMode.PointerDown)
                OnClick();
            eventData.Use();
        }

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

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData) {
            if(eventData.used)
                return;
            eventData.Use();
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if(eventData.used)
                return;
            eventData.Use();
        }

        #endregion
    }
}
