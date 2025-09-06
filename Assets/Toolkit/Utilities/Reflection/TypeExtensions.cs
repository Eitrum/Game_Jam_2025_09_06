using System;
using UnityEngine;

namespace Toolkit {
    public static class TypeExtensions {

        #region Inheritance check

        public static bool Inherits<T>(this Type instance) {
            return Inherits(instance, typeof(T));
        }

        public static bool Inherits<T>(this Type instance, T inheritedObject) {
            return Inherits(instance, typeof(T));
        }

        public static bool Inherits(this Type type, Type inheritedType) {
            return inheritedType.IsAssignableFrom(type);
        }

        #endregion
    }
}
