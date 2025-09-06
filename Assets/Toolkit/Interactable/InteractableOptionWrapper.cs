
namespace Toolkit.Interactables {
    public class InteractableOptionWrapper : IInteractableOption {

        #region Variables

        public string Name { get; private set; }
        public string Description { get; private set; } = string.Empty;

        public OptionState State { get; private set; }
        public int Order { get; private set; }

        private IInteractable method;

        #endregion

        #region Constructor

        public InteractableOptionWrapper(IInteractable method) {
            var hasEntry = InteractableNames.TryGetEntry(method.GetType(), out var entry);
            this.Name = string.IsNullOrEmpty(entry.Name) ? "Interact" : entry.Name;
            this.Description = entry.Description;
            this.method = method;
            this.Order = entry.Order;
        }

        public InteractableOptionWrapper(IInteractable method, int order) {
            this.Name = "Interact";
            this.method = method;
            this.Order = order;
        }

        public InteractableOptionWrapper(string name, IInteractable method, int order = 0) {
            this.Name = name;
            this.method = method;
            this.Order = order;
        }

        public InteractableOptionWrapper(string name, OptionState state, IInteractable method, int order = 0) {
            this.Name = name;
            this.State = state;
            this.method = method;
            this.Order = order;
        }

        public InteractableOptionWrapper(string name, string description, IInteractable method, int order = 0) {
            this.Name = name;
            this.Description = description;
            this.method = method;
            this.Order = order;
        }

        public InteractableOptionWrapper(string name, string description, OptionState state, IInteractable method, int order = 0) {
            this.Name = name;
            this.Description = description;
            this.State = state;
            this.method = method;
            this.Order = order;
        }

        #endregion

        #region Option Impl

        public void Interact(Source source) {
            method?.Interact(source);
        }

        public OptionState GetState(Source source) {
            return State;
        }

        #endregion
    }
}
