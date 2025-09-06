using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Toolkit.IO.TML.Properties;
using Debug = UnityEngine.Debug;

namespace Toolkit.IO.TReflection {
    public unsafe delegate void TMLSetterVariable(void* objPtr, ITMLProperty property);
    public unsafe delegate void TMLGetterVariable(void* objPtr, TMLNode node);

    public unsafe delegate void TMLNodeSetterVariable(void* objPtr, TMLNode node);
    public unsafe delegate void TMLNodeGetterVariable(void* objPtr, TMLNode node);

    public sealed unsafe class TMLReflectionVariable {

        #region Variables

        public readonly TReflectionVariable BaseVariable;

        public readonly TMLSetterVariable Deserialize;
        public readonly TMLGetterVariable Serialize;

        #endregion

        #region Properties

        public string Name => BaseVariable.Name;
        public int Id => BaseVariable.Id;

        #endregion

        #region Constructor

        public TMLReflectionVariable(TReflectionVariable baseVariable, TMLSetterVariable deserialize, TMLGetterVariable serialize) {
            this.BaseVariable = baseVariable;
            this.Deserialize = deserialize;
            this.Serialize = serialize;
        }

        #endregion
    }

    public sealed unsafe class TMLReflectionVariableObject {

        #region Variables

        public readonly TReflectionVariable BaseVariable;

        public readonly TMLNodeSetterVariable Deserialize;
        public readonly TMLNodeGetterVariable Serialize;

        #endregion

        #region Properties

        public string Name => BaseVariable.Name;
        public int Id => BaseVariable.Id;

        #endregion

        #region Constructor

        public TMLReflectionVariableObject(TReflectionVariable baseVariable, TMLNodeSetterVariable deserialize, TMLNodeGetterVariable serialize) {
            this.BaseVariable = baseVariable;
            this.Deserialize = deserialize;
            this.Serialize = serialize;
        }

        #endregion
    }

    public class TMLClassSerializer {

        #region Variables

        private static Dictionary<Type, TMLClassSerializer> cachedInstances = new Dictionary<Type, TMLClassSerializer>();

        public readonly ClassSerializer ClassSerializer;
        public readonly bool IsTMLSerializable;
        private List<TMLReflectionVariable> variables = new List<TMLReflectionVariable>();
        private Dictionary<int, TMLReflectionVariable> idToVariable = new Dictionary<int, TMLReflectionVariable>();
        private List<TMLReflectionVariableObject> objects = new List<TMLReflectionVariableObject>();
        private Dictionary<int, TMLReflectionVariableObject> idToObject = new Dictionary<int, TMLReflectionVariableObject>();

        #endregion

        #region Properties

        public Type Type => ClassSerializer.Type;
        public bool IsClass => ClassSerializer.IsClass;
        public bool IsArray => ClassSerializer.IsArray;

        #endregion

        #region Cache

        public static TMLClassSerializer Get<T>()
            => Get(typeof(T));

        public static TMLClassSerializer Get(Type type) {
            if(!cachedInstances.TryGetValue(type, out var serializer)) {
                serializer = new TMLClassSerializer(type);
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

        protected TMLClassSerializer(System.Type type) {
            cachedInstances.TryAdd(type, this);
            ClassSerializer = ClassSerializer.Get(type);
            IsTMLSerializable = type.Inherits(typeof(ITMLSerializable));

            var serializable = SerializationUtility.GetSerializableFields(Type);
            foreach(var fi in serializable) {
                var fieldType = fi.FieldType;
                var id = fi.Name.GetHash32();
                if(!ClassSerializer.TryGetById(id, out var variable)) {
                    Debug.LogError("Failed to find cached variable");
                    continue;
                }

                var t = CallbackBuilder.CreateTMLWrapper(variable, fi);
                if(t != null) {
                    variables.Add(t);
                    idToVariable.Add(id, t);
                }

                var t2 = CallbackBuilder.CreateTMLWrapperObject(variable, fi);
                if(t2 != null) {
                    objects.Add(t2);
                    idToObject.Add(id, t2);
                }
            }
        }

        #endregion

        #region Serialize

        public unsafe TMLNode Serialize<T>(in T obj) where T : class {
            TMLNode node = new TMLNode("root");
            Serialize(PointerUtility.GetPointer(obj), node);
            return node;
        }

        public unsafe void Serialize<T>(in T obj, TMLNode node) where T : class {
            Serialize(PointerUtility.GetPointer(obj), node);
        }

        public unsafe TMLNode Serialize<T>(in T obj, bool _ = default) where T : struct {
            TMLNode node = new TMLNode("root");
            Serialize(PointerUtility.GetPointer(obj), node);
            return node;
        }

        public unsafe void Serialize<T>(in T obj, TMLNode node, bool _ = default) where T : struct {
            Serialize(PointerUtility.GetPointer(obj), node);
        }

        public unsafe void Serialize(void* ptr, TMLNode node) {
            if(IsTMLSerializable) {
                var obj = ClassSerializer.GetObjectFromPointer(ptr);
                if(obj is ITMLSerializable tmlSerializable) {
                    tmlSerializable.Serialize(node);
                    ClassSerializer.SetObjectAtPointer(ptr, obj);
                }

                return;
            }
            foreach(var v in variables) {
                v.Serialize(ptr, node);
            }
            foreach(var o in objects) {
                o.Serialize(ptr, node);
            }
        }

        #endregion

        #region Deserialize

        public unsafe object Deserialize(TMLNode node) {
            var t = ClassSerializer.Create();
            if(IsClass) {
                Deserialize(Toolkit.Internal.UnsafeUtility.GetPointerFromObject(t), node);
            }
            else {
                GCHandle handle = GCHandle.Alloc(t, GCHandleType.Pinned);
                try {
                    IntPtr objPtr = handle.AddrOfPinnedObject();
                    byte* structPtr = (byte*)objPtr + 8;
                    Deserialize(structPtr, node);
                }
                finally {
                    handle.Free();
                }
            }
            return t;
        }

        public unsafe T Deserialize<T>(TMLNode node, bool _ = default) where T : struct {
            var t = ClassSerializer.Create<T>();
            Deserialize(PointerUtility.GetPointer(t), node);
            return t;
        }

        //private unsafe void Deserialize<T>(in T obj, TMLNode node, bool _ = default) where T : struct {
        //    Deserialize(PointerUtility.GetPointer(obj), node);
        //}

        public unsafe T Deserialize<T>(TMLNode node) where T : class {
            var t = ClassSerializer.Create<T>();
            Deserialize(Toolkit.PointerUtility.GetPointer(t), node);
            return t;
        }

        //private unsafe void Deserialize<T>(in T obj, TMLNode node) where T : class {
        //    Deserialize(Toolkit.PointerUtility.GetPointer(obj), node);
        //}

        public unsafe void Deserialize(void* ptr, TMLNode node) {
            if(IsTMLSerializable) {
                var obj = ClassSerializer.GetObjectFromPointer(ptr);
                if(obj is ITMLSerializable tmlSerializable) {
                    tmlSerializable.Deserialize(node);
                    ClassSerializer.SetObjectAtPointer(ptr, obj);
                }

                return;
            }
            foreach(var p in node.Properties) {
                var id = p.Name.GetHash32();
                if(idToVariable.TryGetValue(id, out var t)) {
                    t.Deserialize(ptr, p);
                }
            }
            foreach(var n in node.Children) {
                var id = n.Name.GetHash32();
                if(idToObject.TryGetValue(id, out var t)) {
                    t.Deserialize(ptr, n);
                }
            }
        }

        #endregion
    }
}
