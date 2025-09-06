using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Toolkit.IO.TML.Properties;
using Debug = UnityEngine.Debug;

namespace Toolkit.IO.TReflection {
    public unsafe delegate void BinarySetterVariable(void* objPtr, IBuffer buffer);
    public unsafe delegate void BinaryGetterVariable(void* objPtr, IBuffer buffer);

    public sealed unsafe class BinaryReflectionVariable {

        #region Variables

        public readonly TReflectionVariable BaseVariable;

        public readonly BinarySetterVariable Deserialize;
        public readonly BinaryGetterVariable Serialize;

        #endregion

        #region Properties

        public string Name => BaseVariable.Name;
        public int Id => BaseVariable.Id;

        #endregion

        #region Constructor

        public BinaryReflectionVariable(TReflectionVariable baseVariable, BinarySetterVariable deserialize, BinaryGetterVariable serialize) {
            this.BaseVariable = baseVariable;
            this.Deserialize = deserialize;
            this.Serialize = serialize;
        }

        #endregion
    }

    public class BinaryClassSerializer {

        #region Variables

        private static Dictionary<Type, BinaryClassSerializer> cachedInstances = new Dictionary<Type, BinaryClassSerializer>();

        public readonly ClassSerializer ClassSerializer;
        public readonly bool IsBufferSerializable;
        private List<BinaryReflectionVariable> variables = new List<BinaryReflectionVariable>();
        private Dictionary<int, BinaryReflectionVariable> idToVariable = new Dictionary<int, BinaryReflectionVariable>();
        private static HashSet<IBuffer> includeMetaDataLookup = new HashSet<IBuffer>();

        #endregion

        #region Properties

        public Type Type => ClassSerializer.Type;
        public bool IsClass => ClassSerializer.IsClass;
        public bool IsArray => ClassSerializer.IsArray;

        #endregion

        #region Cache

        public static BinaryClassSerializer Get<T>() {
            return Get(typeof(T));
        }

        public static BinaryClassSerializer Get(Type type) {
            if(!cachedInstances.TryGetValue(type, out var serializer)) {
                serializer = new BinaryClassSerializer(type);
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

        private BinaryClassSerializer(System.Type type) {
            cachedInstances.TryAdd(type, this);
            ClassSerializer = ClassSerializer.Get(type);
            IsBufferSerializable = type.Inherits(typeof(IBufferSerializable));

            var serializable = SerializationUtility.GetSerializableFields(Type);
            foreach(var fi in serializable) {
                var fieldType = fi.FieldType;
                var id = fi.Name.GetHash32();
                if(!ClassSerializer.TryGetById(id, out var variable)) {
                    Debug.LogError("Failed to find cached variable");
                    continue;
                }

                var t = CallbackBuilder.CreateBinaryWrapper(variable, fi);
                if(t != null) {
                    variables.Add(t);
                    idToVariable.Add(id, t);
                }
            }
        }

        #endregion

        #region Serialize

        public unsafe DynamicBuffer Serialize<T>(in T obj, bool includeMetaData = false) where T : class {
            DynamicBuffer buffer = new DynamicBuffer();
            Serialize(PointerUtility.GetPointer(obj), buffer, includeMetaData);
            return buffer;
        }

        public unsafe void Serialize<T>(in T obj, IBuffer buffer, bool includeMetaData = false) where T : class {
            Serialize(PointerUtility.GetPointer(obj), buffer, includeMetaData);
        }

        public unsafe DynamicBuffer Serialize<T>(in T obj, bool includeMetaData = false, bool _ = default) where T : struct {
            DynamicBuffer buffer = new DynamicBuffer();
            Serialize(PointerUtility.GetPointer(obj), buffer, includeMetaData);
            return buffer;
        }

        public unsafe void Serialize<T>(in T obj, IBuffer buffer, bool includeMetaData = false, bool _ = default) where T : struct {
            Serialize(PointerUtility.GetPointer(obj), buffer, includeMetaData);
        }

        public unsafe void Serialize(void* ptr, IBuffer buffer, bool includeMetaData = false) {
            if(includeMetaData)
                includeMetaDataLookup.Contains(buffer);
            SerializeInternal(ptr, buffer);
            includeMetaDataLookup.Remove(buffer);
        }

        internal unsafe void SerializeInternal(void* ptr, IBuffer buffer) {
            if(IsBufferSerializable) {
                var obj = ClassSerializer.GetObjectFromPointer(ptr);
                if(obj is IBufferSerializable bufferSerializable) {
                    bufferSerializable.Serialize(buffer);
                    ClassSerializer.SetObjectAtPointer(ptr, obj);
                }

                return;
            }
            var includeMetaData = includeMetaDataLookup.Contains(buffer);
            if(includeMetaData) {
                buffer.Write((byte)variables.Count);
                foreach(var v in variables) {
                    buffer.Write(v.Id);
                    using(buffer.CreateSizeScope()) // Records the written size
                        v.Serialize(ptr, buffer);
                }
            }
            else {
                foreach(var v in variables)
                    v.Serialize(ptr, buffer);
            }
        }

        #endregion

        #region Deserialize

        public unsafe object Deserialize(IBuffer buffer) {
            var t = ClassSerializer.Create();
            if(IsClass) {
                Deserialize(Toolkit.Internal.UnsafeUtility.GetPointerFromObject(t), buffer);
            }
            else {
                GCHandle handle = GCHandle.Alloc(t, GCHandleType.Pinned);
                try {
                    IntPtr objPtr = handle.AddrOfPinnedObject();
                    byte* structPtr = (byte*)objPtr + 8;
                    Deserialize(structPtr, buffer);
                }
                finally {
                    handle.Free();
                }
            }
            return t;
        }

        public unsafe T Deserialize<T>(IBuffer buffer, bool includeMetaData = false, bool _ = default) where T : struct {
            var t = ClassSerializer.Create<T>();
            Deserialize(PointerUtility.GetPointer(t), buffer, includeMetaData);
            return t;
        }

        //public unsafe void Deserialize<T>(in T obj, IBuffer buffer, bool includeMetaData = false, bool _ = default) where T : struct {
        //    Deserialize(PointerUtility.GetPointer(obj), buffer, includeMetaData);
        //}

        public unsafe T Deserialize<T>(IBuffer buffer, bool includeMetaData = false) where T : class {
            var t = ClassSerializer.Create<T>();
            Deserialize(Toolkit.PointerUtility.GetPointer(t), buffer, includeMetaData);
            return t;
        }

        //public unsafe void Deserialize<T>(in T obj, IBuffer buffer, bool includeMetaData = false) where T : class {
        //    Deserialize(Toolkit.PointerUtility.GetPointer(obj), buffer, includeMetaData);
        //}

        public unsafe void Deserialize(void* ptr, IBuffer buffer, bool includeMetaData = false) {
            if(includeMetaData)
                includeMetaDataLookup.Add(buffer);
            DeserializeInternal(ptr, buffer);
            includeMetaDataLookup.Remove(buffer);
        }

        internal unsafe void DeserializeInternal(void* ptr, IBuffer buffer) {
            if(IsBufferSerializable) {
                var obj = ClassSerializer.GetObjectFromPointer(ptr);
                if(obj is IBufferSerializable bufferSerializable) {
                    bufferSerializable.Deserialize(buffer);
                    ClassSerializer.SetObjectAtPointer(ptr, obj);
                }

                return;
            }
            var includeMetaData = includeMetaDataLookup.Contains(buffer);
            if(includeMetaData) {
                var count = buffer.ReadByte();
                for(var i = 0; i < count; i++) {
                    var id = buffer.ReadInt();
                    var len = buffer.ReadInt();
                    var cindex = buffer.Index;
                    try {
                        if(idToVariable.TryGetValue(id, out var variable))
                            variable.Deserialize(ptr, buffer);
                        else
                            buffer.Index += len;
                    }
                    catch {
                        buffer.Index = cindex + len;
                    }
                }
            }
            else {
                foreach(var v in variables)
                    v.Deserialize(ptr, buffer);
            }
        }

        #endregion
    }
}
