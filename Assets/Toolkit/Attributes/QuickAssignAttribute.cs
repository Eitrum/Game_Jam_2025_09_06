using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public class QuickAssignAttribute : PropertyAttribute
    {
        public string Path { get; private set; }
        public System.Type Type { get; private set; }

        public QuickAssignAttribute(string path) {
            this.Path = path;
        }

        public QuickAssignAttribute(System.Type type) {
            this.Type = type;
        }

        public QuickAssignAttribute(string path, System.Type type) {
            this.Path = path;
            this.Type = type;
        }
    }
}
