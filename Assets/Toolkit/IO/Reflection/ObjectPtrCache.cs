using System;
using System.Collections.Generic;
using System.Reflection;

namespace Toolkit.IO.TReflection {


    public unsafe delegate ref object GetObjectFromPtr(void* ptr);
    public unsafe delegate void* GetPtrFromObject(in object obj);
    public unsafe delegate ref T GetReferenceFromPtr<T>(void* ptr);
    public unsafe delegate void* GetPtrFromReference<T>(in T obj);

    public unsafe abstract class ObjectPtrCache {
        #region Variables

        private static Dictionary<Type, ObjectPtrCache> cachedInstances = new Dictionary<Type, ObjectPtrCache>();

        public readonly Type Type;
        public readonly bool IsClass;
        public readonly bool IsStruct;

        protected GetObjectFromPtr getObjFromPtr;
        protected GetPtrFromObject getPtrFromObj;

        #endregion

        #region Cache

        public static ObjectPtrCache<T> Get<T>() {
            if(!cachedInstances.TryGetValue(typeof(T), out var serializer)) {
                serializer = new ObjectPtrCache<T>();
            }
            return serializer as ObjectPtrCache<T>;
        }

        public static ObjectPtrCache Get(System.Type type) {
            if(!cachedInstances.TryGetValue(type, out var serializer)) {
                serializer = Activator.CreateInstance(typeof(ObjectPtrCache<>).MakeGenericType(type)) as ObjectPtrCache;
            }
            return serializer;
        }

        public static void RemoveCache(Type type) {
            cachedInstances.Remove(type);
        }

        public static void ClearCache() {
            cachedInstances.Clear();
        }

        #endregion

        #region Constructor

        public ObjectPtrCache(System.Type type) {
            this.Type = type;
            IsClass = SerializationUtility.IsClass(type);
            IsStruct = SerializationUtility.IsStruct(type);
        }

        #endregion

        #region Conversion

        public object GetObjectFromPtr(void* ptr) {
            return getObjFromPtr(ptr);
        }

        public void* GetPtrFromObject(object obj) {
            return getPtrFromObj(obj);
        }

        #endregion

        #region TryGet

        public bool TryGetReferenceFromPtr<T>(void* ptr, out T reference) {
            if(this is ObjectPtrCache<T> optrc) {
                reference = optrc.GetReferenceFromPtr(ptr);
                return true;
            }
            reference = default(T);
            return false;
        }

        public bool TryGetPtrFromReference<T>(in T reference, out void* ptr) {
            if(this is ObjectPtrCache<T> optrc) {
                ptr = optrc.GetPtrFromReference(reference);
                return true;
            }
            ptr = null;
            return false;
        }

        #endregion
    }

    public unsafe sealed class ObjectPtrCache<T> : ObjectPtrCache {

        #region Variables

        private static MethodInfo getClassFromPointerBase;
        private static MethodInfo getStructFromPointerBase;
        private static MethodInfo getPointerFromClassBase;
        private static MethodInfo getPointerFromStructBase;

        private static GetPtrFromObject getPtrFromClassObject;
        private static GetObjectFromPtr getClassFromPtrObject;
        private static GetPtrFromObject getPtrFromStructObject;
        private static GetObjectFromPtr getStructFromPtrObject;

        private GetReferenceFromPtr<T> getRefFromPtr;
        private GetPtrFromReference<T> getPtrFromRef;

        #endregion

        #region Constructor

        static ObjectPtrCache() {
            getClassFromPointerBase = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("GetClassFromPointer");
            getStructFromPointerBase = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("GetStructFromPointer");
            getPointerFromClassBase = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("GetPointerFromClass");
            getPointerFromStructBase = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("GetPointerFromStruct");

            //getPtrFromClassObject = getPointerFromClassBase.MakeGenericMethod(typeof(object)).CreateDelegate(typeof(GetPtrFromObject)) as GetPtrFromObject;
            //getClassFromPtrObject = getClassFromPointerBase.MakeGenericMethod(typeof(object)).CreateDelegate(typeof(GetObjectFromPtr)) as GetObjectFromPtr;
            //getPtrFromStructObject = getPointerFromStructBase.MakeGenericMethod(typeof(object)).CreateDelegate(typeof(GetPtrFromObject)) as GetPtrFromObject;
            //getStructFromPtrObject = getStructFromPointerBase.MakeGenericMethod(typeof(object)).CreateDelegate(typeof(GetObjectFromPtr)) as GetObjectFromPtr;
        }

        public ObjectPtrCache() : base(typeof(T)) {
            var t = typeof(T);
            if(IsClass) {
                getRefFromPtr = getClassFromPointerBase.MakeGenericMethod(t).CreateDelegate(typeof(GetReferenceFromPtr<T>)) as GetReferenceFromPtr<T>;
                getPtrFromRef = getPointerFromClassBase.MakeGenericMethod(t).CreateDelegate(typeof(GetPtrFromReference<T>)) as GetPtrFromReference<T>;
                getObjFromPtr = getClassFromPtrObject;
                getPtrFromObj = getPtrFromClassObject;
            }
            else {
                getRefFromPtr = getStructFromPointerBase.MakeGenericMethod(t).CreateDelegate(typeof(GetReferenceFromPtr<T>)) as GetReferenceFromPtr<T>;
                getPtrFromRef = getPointerFromStructBase.MakeGenericMethod(t).CreateDelegate(typeof(GetPtrFromReference<T>)) as GetPtrFromReference<T>;
                getObjFromPtr = getStructFromPtrObject;
                getPtrFromObj = getPtrFromStructObject;
            }
        }

        #endregion

        #region Conversions

        public ref T GetReferenceFromPtr(void* ptr) {
            return ref getRefFromPtr(ptr);
        }

        public void* GetPtrFromReference(in T reference) {
            return getPtrFromRef(reference);
        }

        #endregion
    }
}
