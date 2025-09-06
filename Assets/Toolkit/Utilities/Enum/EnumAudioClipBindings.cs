using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumAudioClipBindings : EnumBaseBindings<UnityEngine.AudioClip> {
        protected override bool Validate(UnityEngine.AudioClip value) => value != null;
    }
}
