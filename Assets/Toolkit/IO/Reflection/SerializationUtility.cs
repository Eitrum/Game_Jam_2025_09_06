using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Toolkit.IO.TReflection {
    public static class SerializationUtility {
        #region Constructor

        static SerializationUtility() {
            unsafe {
                // Set the FirstArrayElementOffset
                int[] a = {1,2,3};
                var h = GCHandle.Alloc(a, GCHandleType.Pinned);
                FirstArrayElementOffset = (int)((byte*)PointerUtility.GetPointer(a[0]) - (byte*)PointerUtility.GetPointer(a));
                h.Free();

                var items = typeof(List<int>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                ListBackingFieldOffset = CallbackBuilder.GetFieldOffset(items);
            }
        }

        #endregion

        #region Array Helper

        public static readonly int FirstArrayElementOffset;

        public static unsafe int CalculateArraySizeOf(Type t) {
            if(t.IsValueType)
                return Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf(t);
            return IntPtr.Size;
        }

        #endregion

        #region Is Struct

        public static bool IsStruct<T>()
            => IsStruct(typeof(T));

        public static bool IsStruct(Type type) {
            var code = Type.GetTypeCode(type);
            if(code != TypeCode.Object)
                return false;
            return type.IsValueType;
        }

        #endregion

        #region IsClass

        public static bool IsClass<T>()
            => IsClass(typeof(T));

        public static bool IsClass(Type type) {
            var code = Type.GetTypeCode(type);
            if(code != TypeCode.Object)
                return false;
            return type.IsClass;
        }

        #endregion

        #region IsBaseType

        public static bool IsBaseType<T>()
            => IsBaseType(typeof(T));

        public static bool IsBaseType(Type type) {
            var code = Type.GetTypeCode(type);
            switch(code) {
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return false;
            }
            return true;
        }

        #endregion

        #region IsList

        public static readonly int ListBackingFieldOffset;

        public static bool IsList<T>(out Type elementType)
            => IsList(typeof(T), out elementType);

        public static bool IsList(Type type, out Type elementType) {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            elementType = null;
            return false;
        }

        #endregion

        #region Serializable Fields

        public static IEnumerable<FieldInfo> GetSerializableFields(Type type) {
            var allFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return allFields.Where(IsSerializable);
        }

        private static bool IsSerializable(FieldInfo fi) {
            if(fi == null)
                return false;
            if(fi.IsNotSerialized)
                return false;
            if(fi.IsPublic)
                return true;
            var serializeFieldAttribute = fi.GetCustomAttribute<SerializeField>();
            if(serializeFieldAttribute != null)
                return true;
            return false;
        }

        #endregion
    }
}
