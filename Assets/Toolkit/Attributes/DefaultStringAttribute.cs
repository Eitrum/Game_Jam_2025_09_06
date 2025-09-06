using System;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DefaultStringAttribute : PropertyAttribute {

        public enum Mode {
            Default = 0,
            AutoAssign = 1,
            AlwaysShow = 2,
            Both = AutoAssign | AlwaysShow,
        }

        #region Variables

        public readonly string DefaultString;
        public readonly bool AssignToVariable;
        public readonly bool AlwaysShow;

        #endregion

        #region Constructor

        public DefaultStringAttribute() : this(string.Empty) { }

        public DefaultStringAttribute(string value) {
            DefaultString = value;
            AssignToVariable = false;
            AlwaysShow = false;
        }

        public DefaultStringAttribute(string value, bool autoAssign) {
            DefaultString = value;
            AssignToVariable = autoAssign;
            AlwaysShow = false;
        }

        public DefaultStringAttribute(string value, bool autoAssign = false, bool alwaysShow = false) {
            DefaultString = value;
            AssignToVariable = autoAssign;
            AlwaysShow = false;
        }

        public DefaultStringAttribute(string value, Mode mode) {
            DefaultString = value;
            AssignToVariable = mode.HasFlag(Mode.AutoAssign);
            AlwaysShow = mode.HasFlag(Mode.AlwaysShow);
        }

        #endregion
    }
}
