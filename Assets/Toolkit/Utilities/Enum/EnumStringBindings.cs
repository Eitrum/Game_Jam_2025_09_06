using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public class EnumStringBindings : EnumBaseBindings<string> {
        protected override bool Validate(string value) => !string.IsNullOrEmpty(value);
    }
}
