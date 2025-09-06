using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

namespace Toolkit {
    public static class ScriptAttributeUtility {

        #region Variables

        private static Type scriptableAttributeUtility;
        private static MethodInfo getHandler;
        private static MethodInfo getFieldInfoFromProperty;
        private static MethodInfo getScriptTypeFromProperty;

        #endregion

        #region Init

        static ScriptAttributeUtility() {
            UnityInternalUtility.TryGetClass<Editor>("UnityEditor.ScriptAttributeUtility", out scriptableAttributeUtility);
        }

        #endregion

        #region GetHandler

        public static bool TryGetHandler(SerializedProperty property, out object handler) {
            if(!UnityInternalUtility.TryCacheMethod(scriptableAttributeUtility, "GetHandler", ref getHandler)) {
                handler = null;
                return false;
            }
            handler = getHandler?.Invoke(null, new object[] { property });
            return handler != null;
        }

        public static object GetHandler(SerializedProperty property) {
            UnityInternalUtility.TryCacheMethod(scriptableAttributeUtility, "GetHandler", ref getHandler);
            return getHandler?.Invoke(null, new object[] { property });
        }

        #endregion

        #region FieldInfo

        public static bool TryGetFieldInfoFromProperty(SerializedProperty property, out FieldInfo fieldInfo, out Type type) {
            UnityInternalUtility.TryCacheMethod(scriptableAttributeUtility, "getFieldInfoFromProperty", ref getFieldInfoFromProperty);
            System.Type typeOut = null;
            fieldInfo = getFieldInfoFromProperty?.Invoke(null, new object[] { property, typeOut }) as FieldInfo;
            type = typeOut;
            return fieldInfo != null && type != null;
        }

        #endregion

        #region Script Type

        public static bool TryGetScriptTypeFromProperty(SerializedProperty property, out Type type) {
            UnityInternalUtility.TryCacheMethod(scriptableAttributeUtility, "GetScriptTypeFromProperty", ref getScriptTypeFromProperty);
            type = getScriptTypeFromProperty?.Invoke(null, new object[] { property }) as Type;
            return type != null;
        }

        public static Type GetScriptTypeFromProperty(SerializedProperty property) {
            UnityInternalUtility.TryCacheMethod(scriptableAttributeUtility, "GetScriptTypeFromProperty", ref getScriptTypeFromProperty);
            return getScriptTypeFromProperty?.Invoke(null, new object[] { property }) as Type;
        }

        #endregion
    }
}
