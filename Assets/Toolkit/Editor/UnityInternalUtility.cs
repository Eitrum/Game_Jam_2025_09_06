using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace Toolkit {
    public static class UnityInternalUtility {

        #region Variables

        private const string TAG = "[Toolkit.UnityInternalUtility] - ";

        #endregion

        #region Try Get Class

        public static bool TryGetClass<T>(string namespace_class, out System.Type result) {
            return TryGetClass(typeof(T), namespace_class, out result);
        }

        public static bool TryGetClass(System.Type sameAssemblyObject, string namespace_class, out System.Type result) {
            result = null;
            try {
                if(sameAssemblyObject == null) {
                    Debug.LogError(TAG + "Base assembly object is null");
                    return false;
                }
                result = sameAssemblyObject.Assembly.GetType(namespace_class);
                return result != null;
            }
            catch {
                Debug.LogError(TAG + $"Class '{namespace_class}' not found inside assembly '{sameAssemblyObject.Assembly.FullName}'");
                return false;
            }
        }

        public static bool TryCacheClass<T>(string namespace_class, ref System.Type result) {
            return TryCacheClass(typeof(T), namespace_class, ref result);
        }

        public static bool TryCacheClass(System.Type sameAssemblyObject, string namespace_class, ref System.Type result) {
            if(result != null)
                return true;
            try {
                if(sameAssemblyObject == null) {
                    Debug.LogError(TAG + "Base assembly object is null");
                    return false;
                }
                result = sameAssemblyObject.Assembly.GetType(namespace_class);
                return result != null;
            }
            catch {
                Debug.LogError(TAG + $"Class '{namespace_class}' not found inside assembly '{sameAssemblyObject.Assembly.FullName}'");
                return false;
            }
        }

        #endregion

        #region Method

        public static bool TryGetMethod(System.Type type, string methodName, out MethodInfo method) {
            method = null;
            if(type == null) {
                Debug.LogError(TAG + $"Type/class is null, can't search for method '{methodName}'");
                return false;
            }
            try {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                method = methods.FirstOrDefault(x => x.Name == methodName);
                return method != null;
            }
            catch(System.Exception e) {
                Debug.LogError(TAG + $"Method '{methodName}' not found inside class '{type.FullName}'\n{e.Message}");
                return false;
            }
        }

        public static bool TryCacheMethod(System.Type type, string methodName, ref MethodInfo method) {
            if(method != null)
                return true;
            if(type == null) {
                Debug.LogError(TAG + $"Type/class is null, can't search for method '{methodName}'");
                return false;
            }
            try {
                method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                return method != null;
            }
            catch {
                Debug.LogError(TAG + $"Method '{methodName}' not found inside class '{type.FullName}'");
                return false;
            }
        }

        #endregion

        #region Fields

        public static bool TryGetField(System.Type type, string fieldName, out FieldInfo fieldInfo) {
            fieldInfo = null;
            try {
                if(type == null) {
                    Debug.LogError(TAG + $"Type/class is null, can't search for field '{fieldName}'");
                    return false;
                }
                fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                return fieldInfo != null;
            }
            catch {
                Debug.LogError(TAG + $"Field '{fieldName}' not found inside class '{type.FullName}'");
                return false;
            }
        }

        public static bool TryCacheField(System.Type type, string fieldName, ref FieldInfo fieldInfo) {
            if(fieldInfo != null)
                return true;
            try {
                if(type == null) {
                    Debug.LogError(TAG + $"Type/class is null, can't search for field '{fieldName}'");
                    return false;
                }
                fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                return fieldInfo != null;
            }
            catch {
                Debug.LogError(TAG + $"Field '{fieldName}' not found inside class '{type.FullName}'");
                return false;
            }
        }

        #endregion

        #region Properties


        public static bool TryGetProperty(System.Type type, string propertyName, out PropertyInfo propertyInfo) {
            propertyInfo = null;
            try {
                if(type == null) {
                    Debug.LogError(TAG + $"Type/class is null, can't search for Property '{propertyName}'");
                    return false;
                }
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty);
                return propertyInfo != null;
            }
            catch {
                Debug.LogError(TAG + $"Property '{propertyName}' not found inside class '{type.FullName}'");
                return false;
            }
        }

        public static bool TryCacheProperty(System.Type type, string propertyName, ref PropertyInfo propertyInfo) {
            if(propertyInfo != null)
                return true;
            try {
                if(type == null) {
                    Debug.LogError(TAG + $"Type/class is null, can't search for Property '{propertyName}'");
                    return false;
                }
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty);
                return propertyInfo != null;
            }
            catch {
                Debug.LogError(TAG + $"Property '{propertyName}' not found inside class '{type.FullName}'");
                return false;
            }
        }

        #endregion

        #region Get/Set Value

        public static bool TryGetValue(object obj, string fieldName, out object result) {
            if(!TryGetField(obj.GetType(), fieldName, out var fieldInfo)) {
                result = null;
                return false;
            }

            try {
                result = fieldInfo.GetValue(obj);
                return true;
            }
            catch {
                result = null;
                return false;
            }
        }

        public static bool TrySetValue(object obj, string fieldName, object value) {
            if(!TryGetField(obj.GetType(), fieldName, out var fieldInfo)) {
                return false;
            }

            try {
                fieldInfo.SetValue(obj, value);
                return true;
            }
            catch {
                return false;
            }
        }

        #endregion

        #region Get/Set Property

        public static bool TryGetPropertyValue(object obj, string propertyName, out object result) {
            if(!TryGetProperty(obj.GetType(), propertyName, out var propertyInfo)) {
                result = null;
                return false;
            }

            try {
                result = propertyInfo.GetValue(obj);
                return true;
            }
            catch {
                result = null;
                return false;
            }
        }

        public static bool TrySetPropertyValue(object obj, string propertyName, object value) {
            if(!TryGetProperty(obj.GetType(), propertyName, out var propertyInfo)) {
                return false;
            }

            try {
                propertyInfo.SetValue(obj, value);
                return true;
            }
            catch {
                return false;
            }
        }

        #endregion

        #region Editor Window

        public static class EditorWindow {
        
            public static UnityEditor.EditorWindow GetInspector() {
                TryGetClass<Editor>("UnityEditor.InspectorWindow", out var type);
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        
            public static UnityEditor.EditorWindow GetHierarchy() {
                TryGetClass<Editor>("UnityEditor.SceneHierarchyWindow", out var type);
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        
            public static UnityEditor.EditorWindow GetConsoleWindow() {
                TryGetClass<Editor>("UnityEditor.ConsoleWindow", out var type);
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        
            public static UnityEditor.EditorWindow GetSceneView() {
                TryGetClass<Editor>("UnityEditor.SceneView", out var type);
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        
            public static UnityEditor.EditorWindow GetProject() {
                TryGetClass<Editor>("UnityEditor.ProjectBrowser", out var type);
                return UnityEditor.EditorWindow.GetWindow(type);
            }
        }

        #endregion
    }
}
