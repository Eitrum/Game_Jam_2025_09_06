
using System;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute {

        #region Variables

        public readonly string PropertyName;
        public readonly object Value;
        public readonly Mathematics.ComparitorOperatorType OperatorType = Mathematics.ComparitorOperatorType.Equal;
        public readonly bool IgnoreCase;

        #endregion

        #region Constructor

        public ShowIfAttribute(string propertyName) {
            this.PropertyName = propertyName;
        }

        public ShowIfAttribute(string propertyName, bool value, bool inverted = false) : this(propertyName) {
            this.Value = value;
            this.IgnoreCase = !inverted; // Property is checking against this
        }

        public ShowIfAttribute(string propertyName, int value, Mathematics.ComparitorOperatorType operatorType = Mathematics.ComparitorOperatorType.Equal) : this(propertyName) {
            this.Value = value;
            this.OperatorType = operatorType;
        }

        public ShowIfAttribute(string propertyName, float value, Mathematics.ComparitorOperatorType operatorType = Mathematics.ComparitorOperatorType.Equal) : this(propertyName) {
            this.Value = value;
            this.OperatorType = operatorType;
        }

        public ShowIfAttribute(string propertyName, string value, bool ignoreCase = true) : this(propertyName) {
            this.Value = value;
            this.IgnoreCase = ignoreCase;
        }

        public ShowIfAttribute(string propertyName, object value) : this(propertyName) {
            this.Value = value;
        }

        public ShowIfAttribute(string propertyName, object value, Mathematics.ComparitorOperatorType operatorType = Mathematics.ComparitorOperatorType.Equal) : this(propertyName) {
            this.Value = value;
            this.OperatorType = operatorType;
        }

        #endregion
    }
}
