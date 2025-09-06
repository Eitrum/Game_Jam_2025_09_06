using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables.UI {
    public class InteractableUI_Interact : MonoBehaviour {
        #region Variables

        [SerializeField] private TextField keyLabel;
        [SerializeField] private TextField nameLabel;

        [Header("Hold to Select")]
        [SerializeField] private bool allowHoldKey = true;
        [SerializeField] private float holdDuration = 0.3f;
        [SerializeField] private UnityEngine.UI.Image holdKeyImage;

        private Entity entity;
        private IInteractableObject interactable;
        private bool cancelled = false;
        private float holdTime = 0f;

        private Toolkit.UI.UIElementFollowWorldTransform followTarget;

        #endregion

        #region Properties

        private bool IsHolding => holdTime > Mathf.Epsilon;
        public KeyCode Key => KeyCode.E;

        #endregion

        #region Init

        private void Awake() {
            followTarget = GetComponent<Toolkit.UI.UIElementFollowWorldTransform>();
            gameObject.SetActive(false);
        }

        public void Hide() {
            entity = null;
            interactable = null;
            gameObject.SetActive(false);
        }

        public void Assign(Entity entity, IInteractableObject interactable) {
            this.entity = entity;
            cancelled = true;
            holdTime = 0f;
            this.interactable = interactable;
            UpdateGraphics();
        }

        private void UpdateGraphics() {
            if(interactable == null) {
                gameObject.SetActive(false);
                return;
            }
            keyLabel.Text = $"{Key}";
            gameObject.SetActive(true);
            if(followTarget)
                followTarget.SetTarget((interactable as Component).transform);
            var options = interactable.Options;
            if(options == null || options.Count == 0) {
                nameLabel.Text = "Interact";
                return;
            }
            nameLabel.Text = interactable.Options[0].Name;
            if(holdKeyImage)
                holdKeyImage.fillAmount = 0f;
        }

        #endregion

        #region Variables

        private void Update() {
            if(Input.GetKeyDown(Key)) {
                holdTime = 0;
                cancelled = false;
            }
            if(cancelled) {
                return;
            }
            if(Input.GetKey(Key)) {
                holdTime += Time.deltaTime;
                if(holdTime > holdDuration) {
                    GetComponentInParent<InteractableUIPanel>().OpenList(entity, interactable);
                }
            }
            if(Input.GetKeyUp(Key)) {
                if(holdTime < holdDuration) {
                    using(var s = Source.Create(entity))
                        interactable.Interact(s, 0);
                }
                holdTime = 0f;
            }
            if(holdKeyImage)
                holdKeyImage.fillAmount = Mathf.Clamp01(holdTime / holdDuration).Remap(0.3f, 1f, 0, 1f);
        }

        #endregion
    }
}
