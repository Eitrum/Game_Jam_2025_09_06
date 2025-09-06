using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumMaterialBindings : EnumBaseBindings<UnityEngine.Material> {
        protected override bool Validate(UnityEngine.Material value) => value != null;
    }
}
