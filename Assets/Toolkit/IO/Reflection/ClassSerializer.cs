using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Toolkit.IO.TReflection {
    public unsafe class ClassSerializer {
        #region Variables

        private static Dictionary<Type, ClassSerializer> cachedInstances = new Dictionary<Type, ClassSerializer>();

        public readonly Type Type;
        public readonly bool IsClass;
        public readonly bool IsArray;
        public readonly int ArraySizeOf;

        private List<TReflectionVariable> variables;
        private Dictionary<int, TReflectionVariable> idToVariable;

        private ReadStructWrapper readStructWrapper;
        private WriteStructWrapper writeStructWrapper;

        #endregion

        #region Properties

        public IReadOnlyList<TReflectionVariable> Variables => variables;
        public IReadOnlyDictionary<int, TReflectionVariable> IdToVariable => idToVariable;

        #endregion

        #region Cache

        public static ClassSerializer Get<T>()
            => Get(typeof(T));

        public static ClassSerializer Get(System.Type type) {
            if(!cachedInstances.TryGetValue(type, out var serializer)) {
                serializer = new ClassSerializer(type);
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

        private ClassSerializer(System.Type type) {
            //UnityEngine.Debug.LogWarning("Creating a ClassSerializer cache: " + type.FullName);
            cachedInstances.Add(type, this);
            try {
                this.Type = type;
                this.IsClass = SerializationUtility.IsClass(type);
                this.IsArray = type.IsArray;
                this.ArraySizeOf = SerializationUtility.CalculateArraySizeOf(type);
                var serializable = SerializationUtility.GetSerializableFields(type);

                variables = new List<TReflectionVariable>();
                idToVariable = new Dictionary<int, TReflectionVariable>();

                foreach(var s in serializable) {
                    var variable = CallbackBuilder.GetVariable(s, IsClass);
                    variables.Add(variable);
                    idToVariable.Add(variable.Id, variable);
                }

                if(!IsClass) {
                    readStructWrapper = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("Read")
                    .MakeGenericMethod(Type)
                    .CreateDelegate(typeof(ReadStructWrapper)) as ReadStructWrapper;

                    writeStructWrapper = typeof(Toolkit.Internal.UnsafeUtility).GetMethod("Write")
                    .MakeGenericMethod(Type)
                    .CreateDelegate(typeof(WriteStructWrapper)) as WriteStructWrapper;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                if(type != null) {
                    Debug.LogError(this.FormatLog("CRASHED INITIALIZATION", type.FullName));
                }
            }
        }

        #endregion

        #region GetById

        public bool TryGetById(int id, out TReflectionVariable variable) {
            return idToVariable.TryGetValue(id, out variable);
        }

        #endregion

        #region Get

        public TReflectionVariable Get(FieldInfo fi)
            => Get(fi.Name);

        public TReflectionVariable Get(string name) {
            TryGet(name, out var variable);
            return variable;
        }

        public TReflectionVariable<T> Get<T>(FieldInfo fi)
            => Get<T>(fi.Name);

        public TReflectionVariable<T> Get<T>(string name) {
            TryGet<T>(name, out var variable);
            return variable;
        }

        public bool TryGet(string name, out TReflectionVariable variable) {
            for(int i = 0; i < variables.Count; i++)
                if(variables[i].Name == name) {
                    variable = variables[i];
                    return true;
                }
            variable = null;
            return false;
        }

        public bool TryGet<T>(string name, out TReflectionVariable<T> variable) {
            for(int i = 0; i < variables.Count; i++)
                if(variables[i].Name == name) {
                    variable = variables[i].As<T>();
                    return variable != null;
                }
            variable = null;
            return false;
        }

        #endregion

        #region Create

        /// <summary>
        /// GetUninitializedObject
        /// </summary>
        public object Create(bool initializeWithConstructor = false) {
            try {
                if(initializeWithConstructor) {
                    var obj = Activator.CreateInstance(Type);
                    return obj;
                }
            }
            catch { }
            return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(Type);
        }

        /// <summary>
        /// Activator.CreateInstance
        /// </summary>
        public object Create(object item) {
            return Activator.CreateInstance(Type, item);
        }

        /// <summary>
        /// Activator.CreateInstance
        /// </summary>
        public object Create(params object[] item) {
            return Activator.CreateInstance(Type, item);
        }

        public T Create<T>(bool initializeWithConstructor = false) {
            try {
                if(initializeWithConstructor) {
                    var obj = Activator.CreateInstance<T>();
                    return obj;
                }
            }
            catch { }
            return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
        }

        public object CreateArray(int count) {
            //return CallbackBuilder.CreateArrayFunction(Type);
            return Array.CreateInstance(Type, count);
        }

        public T[] CreateArray<T>(int count) {
            return new T[count];
        }

        #endregion

        #region GetObject Wrapper

        public delegate object ReadStructWrapper(void* ptr);
        public delegate void WriteStructWrapper(void* ptr, object obj);

        public unsafe object GetObjectFromPointer(void* ptr) {
            if(IsClass) {
                return Toolkit.Internal.UnsafeUtility.GetObjectFromPointer(ptr);
            }
            else {
                return readStructWrapper(ptr);
            }
        }

        public unsafe void SetObjectAtPointer(void* ptr, in object value) {
            if(IsClass) {
                var objPtr = Toolkit.Internal.UnsafeUtility.GetPointerFromObject(value);
                void** storedPtr = &ptr;
                *storedPtr = objPtr;
            }
            else {
                writeStructWrapper(ptr, value);
            }
        }

        #endregion

        #region Tree

        public TMLNode Tree(int depth = 100) {
            TMLNode node = new TMLNode(Type.Name);
            if(depth <= 0)
                return node;

            foreach(var v in variables) {
                var workingNode = node;
                var childType = v.FieldInfo.FieldType;
                if(childType.IsArray) {
                    childType = childType.GetElementType();
                    workingNode = workingNode.AddChild($"{v.Name} - array<{childType.Name}>");
                }
                if(SerializationUtility.IsList(childType, out var listElementType)) {
                    childType = listElementType;
                    workingNode = workingNode.AddChild($"{v.Name} - list<{childType.Name}>");
                }
                var tcode = Type.GetTypeCode(childType);

                if(tcode == TypeCode.Object) {
                    var recursiveNode = Get(childType).Tree(depth - 1);
                    recursiveNode.SetName($"{v.Name} - {recursiveNode.Name}");
                    workingNode.AddNode(recursiveNode);
                }
                else {
                    workingNode.AddNode($"{v.Name} - {childType.Name}");
                }
            }
            return node;
        }

        #endregion
    }
}
