using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Interactables {
    public class ExamineInteractable : MonoBehaviour, IInteractableMany {
        [SerializeField] private ExaminePreset preset;
        [SerializeField] private GameObject examineUIPanelPrefab;

        public IReadOnlyList<IInteractableOption> Options => new IInteractableOption[]{
            new InteractableOptionWrapper(preset.ExamineCustomName, this)
        };

        public void Interact(Source source) {
            var panel = Toolkit.UI.PanelSystem.PanelManager.GetExistingOrAddToMain(examineUIPanelPrefab);
            panel.IsOnTop = true;
            panel.Assign(preset);
        }
    }
}
