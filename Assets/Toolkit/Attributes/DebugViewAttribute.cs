using System;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class DebugViewAttribute : Attribute {
        #region Variables

        public bool Foldout { get; set; } = false;
        public string Header { get; private set; }
        public bool Truncate { get; private set; } = true;

        #endregion

        #region Constructor

        public DebugViewAttribute() { }
        public DebugViewAttribute(string header) {
            this.Header = header;
        }

        #endregion
    }
}
