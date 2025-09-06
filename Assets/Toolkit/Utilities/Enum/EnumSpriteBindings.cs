using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumSpriteBindings : EnumBaseBindings<UnityEngine.Sprite> {
        protected override bool Validate(UnityEngine.Sprite value) => value != null;
    }
}
