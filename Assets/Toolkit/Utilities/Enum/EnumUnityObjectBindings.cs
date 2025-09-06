using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumUnityObjectBindings : EnumBaseBindings<UnityEngine.Object> {
        protected override bool Validate(UnityEngine.Object value) => value != null;
    }
}
