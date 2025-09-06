using System;

namespace Toolkit {
    /// <summary>
    ///  Base interface for anything that need to use a null check to verify UnityEngine.Object is actual null by Unity standard.
    ///  
    /// An UnityEngine.Object is not null if viewed through an interface and will always keep it's reference unless checked through ==(UnityEngine.Object, null) override.
    /// </summary>
    public interface INullable {
        bool IsNull { get; }

        /// <summary>
        /// Returns a nullable object that never becomes null
        /// </summary>
        public static INullable Never { get; private set; } = new Nullable(false);

        /// <summary>
        /// Returns a nullable object that is always null
        /// </summary>
        public static INullable Always { get; private set; } = new Nullable(true);

        internal class Nullable : INullable {
            public bool IsNull { get; private set; }
            public Nullable(bool isNull) { this.IsNull = isNull; }
        }
    }

    /// <summary>
    /// A wrapper for Unity Object references to use INullable
    /// </summary>
    public class UnityObjectNullable : INullable {
        public readonly UnityEngine.Object obj;
        public bool IsNull => obj == null;
        public UnityObjectNullable(UnityEngine.Object obj) { this.obj = obj; }
    }
}
