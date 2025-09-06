using System;
using UnityEngine;

namespace Toolkit {
    public class ReadonlyAttribute : PropertyAttribute {
        public bool onlyInGame = false;

        public ReadonlyAttribute() { }
        public ReadonlyAttribute(bool onlyInGame) => this.onlyInGame = onlyInGame;
    }
}
