using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumGradientBindings : EnumBaseBindings<UnityEngine.Gradient> {
        protected override bool Validate(UnityEngine.Gradient value) => value != null;
    }
}
