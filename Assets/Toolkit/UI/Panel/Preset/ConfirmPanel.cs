using System;
using System.Collections;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.UI.PanelSystem.Preset {
    public class ConfirmPanel : MonoBehaviour {

        public enum UserAction {
            None,
            Ok,
            Cancel,
            Close,
        }

        #region Variables

        private const string DEFAULT_OK_TEXT = "Ok";
        private const string DEFAULT_CANCEL_TEXT = "Cancel";

        private const string YES_TEXT = "Yes";
        private const string NO_TEXT = "No";

        [SerializeField] private TextField header;
        [SerializeField] private TextField content;

        [SerializeField] private UnityEngine.UI.Button closeButton;
        [SerializeField] private UnityEngine.UI.Button cancelButton;
        [SerializeField] private UnityEngine.UI.Button okButton;

        private Promise<UserAction> promise;
        private Panel panel;

        #endregion

        #region Init

        private void Awake() {
            panel = GetComponent<Panel>();

            if(closeButton)
                closeButton.onClick.AddListener(OnCloseClicked);
            if(okButton)
                okButton.onClick.AddListener(OnOkClicked);
            if(cancelButton)
                cancelButton.onClick.AddListener(OnCancelClicked);

            panel = GetComponent<Panel>();
            panel.OnCloseRequested += (i) => promise?.Complete(UserAction.Close);
        }

        private void OnDestroy() {
            promise?.Complete(UserAction.None);
        }

        #endregion

        #region Show

        public Promise<UserAction> ShowYesNo(string header, string message)
            => Show(header, message, YES_TEXT, NO_TEXT);

        public Promise<UserAction> Show(string header, string message)
            => Show(header, message, DEFAULT_OK_TEXT, DEFAULT_CANCEL_TEXT);

        public Promise<UserAction> Show(string header, string message, string okText)
            => Show(header, message, okText, DEFAULT_CANCEL_TEXT);

        public Promise<UserAction> Show(string header, string message, string okText, string cancelText) {
            promise = new Promise<UserAction>();
            promise.DisableTimeout();
            if(this.header.IsValid)
                this.header.Text = header;
            if(this.content.IsValid)
                this.content.Text = message;

            if(closeButton) {
                closeButton.gameObject.SetActive(true);
                closeButton.interactable = true;
            }

            if(okButton) {
                okButton.gameObject.SetActive(!string.IsNullOrEmpty(okText));
                okButton.interactable = true;
                var tf = TextField.FindInChildren(okButton);
                if(tf.IsValid)
                    tf.Text = okText;
            }

            if(cancelButton) {
                cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(cancelText));
                cancelButton.interactable = true;
                var tf = TextField.FindInChildren(cancelButton);
                if(tf.IsValid)
                    tf.Text = cancelText;
            }

            return promise;
        }

        #endregion

        #region Callbacks

        private void OnOkClicked() {
            promise?.Complete(UserAction.Ok);
            panel.Close();
        }

        private void OnCancelClicked() {
            promise?.Complete(UserAction.Cancel);
            panel.Close();
        }

        private void OnCloseClicked() {
            promise?.Complete(UserAction.Close);
            panel.Close();
        }

        #endregion
    }
}
