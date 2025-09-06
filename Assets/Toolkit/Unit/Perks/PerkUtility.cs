using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static partial class PerkUtility {
        public static int GetPerkId(string fileName) => fileName.GetHash32();
    }
}
