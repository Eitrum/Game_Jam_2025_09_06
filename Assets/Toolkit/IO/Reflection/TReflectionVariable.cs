
using System.Reflection;

namespace Toolkit.IO.TReflection {
    public unsafe delegate void SetterVariable<T>(void* objPtr, T o);
    public unsafe delegate T GetterVariable<T>(void* objPtr);
    public unsafe delegate void* GetterVariableAsPtr(void* objPtr);

    public abstract unsafe class TReflectionVariable {
        #region Variables

        public readonly string Name;
        public readonly int Id;
        public readonly FieldInfo FieldInfo;

        #endregion

        #region Constructor

        public TReflectionVariable(FieldInfo fieldInfo) : this(fieldInfo.Name) {
            this.FieldInfo = fieldInfo;
        }

        public TReflectionVariable(string name) {
            this.Name = name;
            this.Id = name.GetHash32();
        }

        #endregion

        #region GetSet Object

        public abstract object GetObject(void* objPtr);
        public abstract void SetObject(void* objPtr, object obj);

        public unsafe void* GetPtr<TParent>(in TParent parent) where TParent : class
            => GetPtr(Toolkit.PointerUtility.GetPointer(parent));

        public unsafe void* GetPtr<TParent>(in TParent parent, bool _ = default) where TParent : struct
            => GetPtr(PointerUtility.GetPointer(parent));

        public abstract void* GetPtr(void* objPtr);

        #endregion

        #region Get Generic

        public T Get<T>(void* objPtr)
            => (this is TReflectionVariable<T> tclass) ? tclass.Get(objPtr) : default(T);

        public bool TryGet<T>(void* objPtr, out T value) {
            if(this is TReflectionVariable<T> tclass)
                return tclass.TryGet(objPtr, out value);
            value = default;
            return false;
        }

        #endregion

        #region Set Generic

        public void Set<T>(void* objPtr, T value) {
            if(this is TReflectionVariable<T> tclass)
                tclass.Set(objPtr, value);
        }

        public bool TrySet<T>(void* objPtr, in T value) {
            if(this is TReflectionVariable<T> tclass) {
                return tclass.TrySet(objPtr, value);
            }
            return false;
        }

        #endregion

        #region Convert

        public bool Is<T>(out TReflectionVariable<T> var) {
            if(this is TReflectionVariable<T> tvar) {
                var = tvar;
                return true;
            }
            var = default;
            return false;
        }

        public TReflectionVariable<T> As<T>() => this as TReflectionVariable<T>;

        #endregion
    }

    public sealed unsafe class TReflectionVariable<T> : TReflectionVariable {
        #region Variables

        public readonly SetterVariable<T> Setter;
        public readonly GetterVariable<T> Getter;
        public readonly GetterVariableAsPtr GetterAsPtr;

        #endregion

        #region Constructor

        public TReflectionVariable(FieldInfo fieldInfo, SetterVariable<T> setter, GetterVariable<T> getter, GetterVariableAsPtr getterAsPtr = null) : base(fieldInfo) {
            this.Setter = setter;
            this.Getter = getter;
            this.GetterAsPtr = getterAsPtr;
        }

        public TReflectionVariable(string name, SetterVariable<T> setter, GetterVariable<T> getter, GetterVariableAsPtr getterAsPtr = null) : base(name) {
            this.Setter = setter;
            this.Getter = getter;
            this.GetterAsPtr = getterAsPtr;
        }

        #endregion

        #region Overrides

        public override object GetObject(void* objPtr) {
            return Getter(objPtr);
        }

        public override void SetObject(void* objPtr, object obj) {
            Setter(objPtr, (T)obj);
        }

        public override unsafe void* GetPtr(void* objPtr) {
            return GetterAsPtr(objPtr);
        }

        #endregion

        #region Get

        public T Get<TObj>(in TObj tObj) where TObj : class {
            return Get(Toolkit.PointerUtility.GetPointer(tObj));
        }

        public T Get<TObj>(in TObj tObj, bool _ = default) where TObj : struct {
            return Get(PointerUtility.GetPointer(tObj));
        }

        public T Get(void* objPtr) {
            return Getter(objPtr);
        }

        public bool TryGet<TObj>(TObj tObj, out T value) where TObj : class {
            return TryGet(Toolkit.PointerUtility.GetPointer(tObj), out value);
        }

        public bool TryGet<TObj>(TObj tObj, out T value, bool _ = default) where TObj : struct {
            return TryGet(PointerUtility.GetPointer(tObj), out value);
        }

        public bool TryGet(void* objPtr, out T value) {
            value = Getter(objPtr);
            return true;
        }

        #endregion

        #region Set

        public void Set<TObj>(in TObj tObj, T value) where TObj : class {
            Set(Toolkit.PointerUtility.GetPointer(tObj), value);
        }

        public void Set<TObj>(in TObj tObj, T value, bool _ = default) where TObj : struct {
            Set(PointerUtility.GetPointer(tObj), value);
        }

        public void Set(void* objPtr, T value) {
            Setter(objPtr, value);
        }

        public bool TrySet<TObj>(TObj tObj, T value) where TObj : class {
            return TrySet(Toolkit.PointerUtility.GetPointer(tObj), value);
        }

        public bool TrySet<TObj>(TObj tObj, T value, bool _ = default) where TObj : struct {
            return TrySet(PointerUtility.GetPointer(tObj), value);
        }

        public bool TrySet(void* objPtr, T value) {
            Setter(objPtr, value);
            return true;
        }

        #endregion
    }
}
