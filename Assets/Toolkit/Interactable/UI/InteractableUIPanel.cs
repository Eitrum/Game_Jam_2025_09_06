using System.Collections;
using System.Collections.Generic;
using Toolkit.UI.PanelSystem;
using UnityEngine;

namespace Toolkit.Interactables.UI {
    public class InteractableUIPanel : MonoBehaviour {

        [SerializeField] private InteractableUI_Interact interact;
        [SerializeField] private InteractableUI_List list;

        private Entity entity;
        private IInteractableObject interactableObject;
        private HUDModule panel; // Swap out panels for HUD

        private void Awake() {
            panel = GetComponent<HUDModule>();
        }

        private void OnDisable() {
            this.entity = null;
            this.interactableObject = null;
        }

        public void Assign(Entity entity, IInteractableObject interactable) {
            if(interactableObject == interactable) {
                if((list == null || !list.gameObject.activeSelf) && !interact.gameObject.activeSelf)
                    interact.Assign(entity, interactable);
                return;
            }

            this.entity = entity;
            this.interactableObject = interactable;

            if(interactable == null) {
                list.Hide();
                interact.Hide();
                panel.Hide();
                return;
            }

            if(list)
                list.Hide();
            interact.Assign(entity, interactable);
        }

        public void OpenList(Entity entity, IInteractableObject interactable) {
            interact.Hide();
            if(list)
                list.Assign(entity, interactable);
        }
    }
}
