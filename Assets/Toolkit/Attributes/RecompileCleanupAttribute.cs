
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RecompileCleanupAttribute : PropertyAttribute {
        public RecompileCleanupAttribute() { }
    }
}
