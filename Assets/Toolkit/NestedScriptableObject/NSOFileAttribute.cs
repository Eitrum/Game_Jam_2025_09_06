using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NSOFileAttribute : Attribute {

        #region Variables

        public string Name { get; private set; }
        public Type Type { get; internal set; }
        public int Priority { get; private set; }
        public string FileName { get; private set; }

        #endregion

        #region Constructor

        public NSOFileAttribute(Type type) {
            this.Type = type;
        }

        public NSOFileAttribute(string name, Type type) {
            this.Name = name;
            this.Type = type;
        }

        public NSOFileAttribute(string name, Type type, int priority) {
            this.Name = name;
            this.Type = type;
            this.Priority = priority;
        }

        public NSOFileAttribute(Type type, string fileName) {
            this.Type = type;
            this.FileName = fileName;
        }

        public NSOFileAttribute(string name, string fileName, Type type) {
            this.Name = name;
            this.FileName = fileName;
            this.Type = type;
        }

        public NSOFileAttribute(string name, string fileName, Type type, int priority) {
            this.Name = name;
            this.FileName = fileName;
            this.Type = type;
            this.Priority = priority;
        }

        #endregion
    }
}
