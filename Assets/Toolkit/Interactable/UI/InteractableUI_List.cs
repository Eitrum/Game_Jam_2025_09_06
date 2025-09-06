using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables.UI {
    public class InteractableUI_List : MonoBehaviour {

        private Entity entity;
        private IInteractableObject interactableObject;
        private Toolkit.UI.UIElementFollowWorldTransform followTarget;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private RectTransform listContainer;
        private List<InteractableUI_ListItem> items = new List<InteractableUI_ListItem>();

        private void Awake() {
            followTarget = GetComponent<Toolkit.UI.UIElementFollowWorldTransform>();
            gameObject.SetActive(false);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void Assign(Entity entity, IInteractableObject interactable) {
            this.entity = entity;
            this.interactableObject = interactable;
            if(interactableObject == null) {
                gameObject.SetActive(false);
                return;
            }

            if(followTarget)
                followTarget.SetTarget((interactableObject as Component).transform);

            if(itemPrefab == null) {
                // Try salvage?
                if(listContainer.childCount == 0)
                    return;
                var ttrans = listContainer.GetChild(0);
                ttrans.SetParent(transform);
                ttrans.gameObject.SetActive(false);
                itemPrefab = ttrans.gameObject;
            }
            listContainer.DestroyAllChildren();
            items.Clear();

            var options = interactableObject.Options;

            using(var s = Source.Create(entity)) {
                int index = 1;
                foreach(var o in options) {
                    var state = o.GetState(s);
                    if(state == OptionState.Hidden)
                        continue;

                    var go = Instantiate(itemPrefab, listContainer);
                    go.SetActive(true);
                    var listitem = go.GetComponent<InteractableUI_ListItem>();
                    listitem.Assign(index++, entity, o, state);
                    items.Add(listitem);
                }
            }

            gameObject.SetActive(true);
        }

        private void Update() {
            for(int i = 0; i < items.Count; i++) {
                if(Input.GetKeyDown((KeyCode.Alpha1 + i))) {
                    items[i].Activate();
                }
            }
        }
    }
}
