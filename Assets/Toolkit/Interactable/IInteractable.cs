using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables {
    public interface IInteractable {
        void Interact(Source source);
    }

    public interface IInteractableMany : IInteractable {
        IReadOnlyList<IInteractableOption> Options { get; }
    }
}
