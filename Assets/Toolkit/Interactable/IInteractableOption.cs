using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Interactables {
    public interface IInteractableOption : IInteractable {
        string Name { get; }
        string Description { get; }
        int Order { get; }

        OptionState GetState(Source source);
    }
}
