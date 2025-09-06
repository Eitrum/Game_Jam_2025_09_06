using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumTextureBindings : EnumBaseBindings<UnityEngine.Texture> {
        protected override bool Validate(UnityEngine.Texture value) => value != null;
    }
}
