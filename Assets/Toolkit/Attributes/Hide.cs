
using UnityEngine;

namespace Toolkit {
    public class HideAttribute : PropertyAttribute {
        public readonly bool ShowIfHoldingAlt;

        public HideAttribute() { }
        public HideAttribute(bool showIfHoldingAlt) {
            this.ShowIfHoldingAlt = showIfHoldingAlt;
        }
    }
}
