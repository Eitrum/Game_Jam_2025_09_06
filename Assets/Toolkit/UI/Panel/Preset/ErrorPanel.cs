using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.UI.PanelSystem.Preset {
    public class ErrorPanel : MonoBehaviour {

        public enum UserAction {
            None,
            Ok,
            Close,
        }

        #region Variables

        private const string DEFAULT_ERROR_HEADER = "Error";
        private const string DEFAULT_OK_BUTTON_LABEL = "Ok";

        [SerializeField] private TextField header;
        [SerializeField] private TextField content;

        [SerializeField] private UnityEngine.UI.Button closeButton;
        [SerializeField] private UnityEngine.UI.Button okButton;

        private Promise<UserAction> promise;
        private Panel panel;

        #endregion

        #region Init

        private void Awake() {
            if(closeButton)
                closeButton.onClick.AddListener(OnCloseClicked);
            if(okButton)
                okButton.onClick.AddListener(OnOkClicked);

            panel = GetComponent<Panel>();
            panel.OnCloseRequested += (i) => promise?.Complete(UserAction.Close);
        }

        private void OnDestroy() {
            promise?.Complete(UserAction.None);
        }

        public Promise<UserAction> Show(string message) {
            return this.Show(DEFAULT_ERROR_HEADER, message);
        }

        public Promise<UserAction> Show(string header, string message) {
            return this.Show(header, message, DEFAULT_OK_BUTTON_LABEL);
        }

        public Promise<UserAction> Show(string header, string message, string confirmButtonText) {
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
                okButton.gameObject.SetActive(true);
                okButton.interactable = true;
                var tf = TextField.FindInChildren(okButton);
                if(tf.IsValid)
                    tf.Text = confirmButtonText;
            }

            return promise;
        }

        #endregion

        #region Callbacks

        private void OnCloseClicked() {
            promise?.Complete(UserAction.Close);
            panel.Close();
        }

        private void OnOkClicked() {
            promise?.Complete(UserAction.Ok);
            panel.Close();
        }

        #endregion
    }
}
