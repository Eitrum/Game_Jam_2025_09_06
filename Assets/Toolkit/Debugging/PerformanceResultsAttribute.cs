using System;
using UnityEngine;

namespace Toolkit {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PerformanceResultsAttribute : PropertyAttribute {

        public enum ViewMode {
            Default,
            Milliseconds,
            Ticks,
        }

        public ViewMode Mode { get; private set; } = ViewMode.Default;

        public PerformanceResultsAttribute() { }

        public PerformanceResultsAttribute(ViewMode mode) {
            this.Mode = mode;
        }
    }
}
