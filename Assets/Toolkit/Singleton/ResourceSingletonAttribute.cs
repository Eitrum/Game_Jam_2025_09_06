using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ResourceSingletonAttribute : Attribute {
        private string resourcePath = "";
        public string ResourcePath => resourcePath;
        public bool HasResourcePath => !string.IsNullOrEmpty(resourcePath);

        public ResourceSingletonAttribute() { }
        public ResourceSingletonAttribute(string path) => resourcePath = path;
    }
}
