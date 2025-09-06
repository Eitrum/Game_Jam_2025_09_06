using System;
using System.Runtime.Serialization;

namespace Toolkit {
    public static class Type<T> {
        #region Variables

        public static readonly Type CachedType;
        public static readonly string FullName;
        public static readonly string Name;
        public static readonly T Value;

        #endregion

        #region Constructor

        static Type() {
            CachedType = typeof(T);
            FullName = CachedType.FullName;
            Name = CachedType.Name;
            Value = default(T);
            if(Value == null) {
                try {
                    Value = (T)FormatterServices.GetUninitializedObject(typeof(T));
                }
                catch { }
            }
            //UnityEngine.Debug.LogWarning("Creating a type cache: " + FullName);
        }

        #endregion

        #region Util

        public static bool Inherits<TOther>() {
            return CachedType.Inherits<TOther>();
        }

        #endregion
    }
}
