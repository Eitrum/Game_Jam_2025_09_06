using Toolkit.UI.PanelSystem;
using UnityEngine;

namespace Toolkit.Interactables.UI {
    public class InteractableUI_ListItem : MonoBehaviour {

        [SerializeField] private TextField indexLabel;
        [SerializeField] private TextField nameLabel;

        [SerializeField] private UnityEngine.UI.Button button;

        private Entity entity;
        private IInteractableOption option;

        private void Awake() {
            button = GetComponent<UnityEngine.UI.Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Activate);
        }

        public void Assign(int index, Entity entity, IInteractableOption option, OptionState state) {
            if(this.indexLabel.IsValid)
                this.indexLabel.Text = $"{index}";
            if(this.nameLabel.IsValid)
                this.nameLabel.Text = option.Name;
            button.interactable = state == OptionState.None;

            this.entity = entity;
            this.option = option;
        }

        public void Activate() {
            GetComponentInParent<InteractableUI_List>().Hide();
            try {
                using(var s = Source.Create(entity))
                    option.Interact(s);
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }
    }
}
