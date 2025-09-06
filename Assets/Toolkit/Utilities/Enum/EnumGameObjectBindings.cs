using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumGameObjectBindings : EnumBaseBindings<UnityEngine.GameObject> {
        protected override bool Validate(UnityEngine.GameObject value) => value != null;
    }
}
