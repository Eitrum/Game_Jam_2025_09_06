using System;
using System.Collections.Generic;
using System.Reflection;
using Toolkit.IO.TReflection;

namespace Toolkit {
    public static class FieldInfoExtensions {
        #region ToTReflection

        public static TReflectionVariable ToTReflection(this FieldInfo fieldInfo) {
            return CallbackBuilder.GetCachedVariable(fieldInfo);
        }

        public static TReflectionVariable<T> ToTReflection<T>(this FieldInfo fieldInfo) {
            return CallbackBuilder.GetCachedVariable<T>(fieldInfo);
        }

        #endregion
    }
}
