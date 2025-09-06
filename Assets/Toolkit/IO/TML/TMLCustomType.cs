using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.IO.TML {
    public static class TMLCustomType {

        // Custom types will be serialized as byte array
        // Maximum size is 65536 bytes

        private static HashSet<string> names = new HashSet<string>();

        private static Dictionary<short, List<string>> lookup = new Dictionary<short, List<string>>();
    }
}
