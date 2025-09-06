using System;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ButtonAttribute : Attribute {

        #region Variables

        public string Name { get; set; }
        public EditorGUIMode Setting { get; private set; } = EditorGUIMode.Default;

        #endregion

        #region Constructor

        public ButtonAttribute() { }
        public ButtonAttribute(string name) {
            this.Name = name;
        }

        public ButtonAttribute(EditorGUIMode setting) {
            this.Setting = setting;
        }
        public ButtonAttribute(string name, EditorGUIMode setting) {
            this.Name = name;
            this.Setting = setting;
        }

        #endregion
    }
}
