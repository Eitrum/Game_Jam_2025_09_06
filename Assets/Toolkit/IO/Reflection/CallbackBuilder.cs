using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Toolkit.IO.TReflection {

    /// <summary>
    /// https://meetemq.com/2023/01/14/using-pointers-in-c-unity/
    /// </summary>

    public static unsafe class CallbackBuilder {

        #region Variables

        private const string TAG = "[Tookit.IO.TReflection.CallbackBuilder] - ";

        private unsafe delegate TReflectionVariable CachedNonGenericDelegate(FieldInfo fieldInfo, int offset);

        private static class TReflectionVariableContainer<T> {
            public static Dictionary<int, SetterVariable<T>> setterByOffset = new Dictionary<int, SetterVariable<T>>();
            public static Dictionary<int, GetterVariable<T>> getterByOffset = new Dictionary<int, GetterVariable<T>>();
            public static Dictionary<int, GetterVariableAsPtr> getterAsPtrByOffset = new Dictionary<int, GetterVariableAsPtr>();

            static TReflectionVariableContainer() {
                //UnityEngine.Debug.LogWarning("Creating a TRefCon cache: " + typeof(T).FullName);
            }
        }

        private static MethodInfo getVariableForClassBase;
        private static MethodInfo getVariableForStructBase;
        private static Dictionary<Type, CachedNonGenericDelegate> cachedNonGenericToGeneric = new Dictionary<Type, CachedNonGenericDelegate>();

        #endregion

        #region Init

        static CallbackBuilder() {
            //UnityEngine.Debug.LogWarning("Creating a CallbackBuilder cache");
            getVariableForClassBase = typeof(CallbackBuilder).GetMethods().FirstOrDefault(x => x.Name == nameof(GetVariableForClass) && x.GetParameters().Length == 2);
            getVariableForStructBase = typeof(CallbackBuilder).GetMethods().FirstOrDefault(x => x.Name == nameof(GetVariableForStruct) && x.GetParameters().Length == 2);
        }

        #endregion

        #region GetCachedVariable

        public static TReflectionVariable<T> GetCachedVariable<T>(FieldInfo fieldInfo)
            => ClassSerializer.Get(fieldInfo.DeclaringType).Get<T>(fieldInfo);

        public static TReflectionVariable GetCachedVariable(FieldInfo fieldInfo)
            => ClassSerializer.Get(fieldInfo.DeclaringType).Get(fieldInfo);

        #endregion

        #region GetVariable

        public static TReflectionVariable GetVariable(FieldInfo fieldInfo)
            => GetVariable(fieldInfo, IsClass(fieldInfo.DeclaringType));

        public static TReflectionVariable GetVariable(FieldInfo fieldInfo, bool isClass) {
            try {
                var fieldOffset = GetFieldOffset(fieldInfo, isClass);
                var fieldType = fieldInfo.FieldType;
                switch(Type.GetTypeCode(fieldType)) {
                    case TypeCode.Byte: return new TReflectionVariable<byte>(fieldInfo, SetterUnmanaged<byte>(fieldOffset), GetterUnmanaged<byte>(fieldOffset), GetterAsPtrUnmanaged<byte>(fieldOffset));
                    case TypeCode.SByte: return new TReflectionVariable<sbyte>(fieldInfo, SetterUnmanaged<sbyte>(fieldOffset), GetterUnmanaged<sbyte>(fieldOffset), GetterAsPtrUnmanaged<sbyte>(fieldOffset));
                    case TypeCode.Int16: return new TReflectionVariable<short>(fieldInfo, SetterUnmanaged<short>(fieldOffset), GetterUnmanaged<short>(fieldOffset), GetterAsPtrUnmanaged<short>(fieldOffset));
                    case TypeCode.UInt16: return new TReflectionVariable<ushort>(fieldInfo, SetterUnmanaged<ushort>(fieldOffset), GetterUnmanaged<ushort>(fieldOffset), GetterAsPtrUnmanaged<ushort>(fieldOffset));
                    case TypeCode.Int32: return new TReflectionVariable<int>(fieldInfo, SetterUnmanaged<int>(fieldOffset), GetterUnmanaged<int>(fieldOffset), GetterAsPtrUnmanaged<int>(fieldOffset));
                    case TypeCode.UInt32: return new TReflectionVariable<uint>(fieldInfo, SetterUnmanaged<uint>(fieldOffset), GetterUnmanaged<uint>(fieldOffset), GetterAsPtrUnmanaged<uint>(fieldOffset));
                    case TypeCode.Int64: return new TReflectionVariable<long>(fieldInfo, SetterUnmanaged<long>(fieldOffset), GetterUnmanaged<long>(fieldOffset), GetterAsPtrUnmanaged<long>(fieldOffset));
                    case TypeCode.UInt64: return new TReflectionVariable<ulong>(fieldInfo, SetterUnmanaged<ulong>(fieldOffset), GetterUnmanaged<ulong>(fieldOffset), GetterAsPtrUnmanaged<ulong>(fieldOffset));

                    case TypeCode.Single: return new TReflectionVariable<float>(fieldInfo, SetterUnmanaged<float>(fieldOffset), GetterUnmanaged<float>(fieldOffset), GetterAsPtrUnmanaged<float>(fieldOffset));
                    case TypeCode.Double: return new TReflectionVariable<double>(fieldInfo, SetterUnmanaged<double>(fieldOffset), GetterUnmanaged<double>(fieldOffset), GetterAsPtrUnmanaged<double>(fieldOffset));
                    case TypeCode.Decimal: return new TReflectionVariable<decimal>(fieldInfo, SetterUnmanaged<decimal>(fieldOffset), GetterUnmanaged<decimal>(fieldOffset), GetterAsPtrUnmanaged<decimal>(fieldOffset));

                    case TypeCode.Boolean: return new TReflectionVariable<bool>(fieldInfo, SetterUnmanaged<bool>(fieldOffset), GetterUnmanaged<bool>(fieldOffset), GetterAsPtrUnmanaged<bool>(fieldOffset));
                    case TypeCode.Char: return new TReflectionVariable<char>(fieldInfo, SetterUnmanaged<char>(fieldOffset), GetterUnmanaged<char>(fieldOffset), GetterAsPtrUnmanaged<char>(fieldOffset));
                    case TypeCode.String: return new TReflectionVariable<string>(fieldInfo, SetterString(fieldOffset), GetterString(fieldOffset), GetterAsPtrString(fieldOffset));

                    case TypeCode.DateTime: return new TReflectionVariable<DateTime>(fieldInfo, SetterUnmanaged<DateTime>(fieldOffset), GetterUnmanaged<DateTime>(fieldOffset), GetterAsPtrUnmanaged<DateTime>(fieldOffset));

                    case TypeCode.Object:
                        if(!cachedNonGenericToGeneric.TryGetValue(fieldType, out var cached)) {
                            if(fieldType.IsClass) {
                                var mi = getVariableForClassBase.MakeGenericMethod(fieldType);
                                cached = (fieldInfo, offset) => mi.InvokeStatic<FieldInfo, int, TReflectionVariable>(fieldInfo, offset);
                            }
                            else {
                                var mi = getVariableForStructBase.MakeGenericMethod(fieldType);
                                cached = (fieldInfo, offset) => mi.InvokeStatic<FieldInfo, int, TReflectionVariable>(fieldInfo, offset);
                            }
                        }
                        return cached(fieldInfo, fieldOffset);

                    default:
                        UnityEngine.Debug.LogError(TAG + "TypeCode is null");
                        return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        #endregion

        #region GetVariable For Struct

        public static TReflectionVariable<T> GetVariableForStruct<T>(FieldInfo fieldInfo) where T : struct {
            return GetVariableForStruct<T>(fieldInfo, GetFieldOffset(fieldInfo));
        }

        public static TReflectionVariable<T> GetVariableForStruct<T>(FieldInfo fieldInfo, int offset) where T : struct {
            var type = fieldInfo.FieldType;
            var fieldTypeIsClass = type.IsClass;
            if(fieldTypeIsClass) {
                Debug.LogError(TAG + "Attempting to get variable for a struct on a class object!");
                return null;
            }

            if(!TReflectionVariableContainer<T>.setterByOffset.TryGetValue(offset, out var setter)) {

                setter = (void* ptr, T value) => {
                    ref T target = ref UnsafeUtility.AsRef<T>((void*)((byte*)ptr + offset));
                    target = value;
                };
                TReflectionVariableContainer<T>.setterByOffset.Add(offset, setter);
            }

            if(!TReflectionVariableContainer<T>.getterByOffset.TryGetValue(offset, out var getter)) {
                getter = (void* ptr) => {
                    ref T source = ref UnsafeUtility.AsRef<T>((void*)((byte*)ptr + offset));
                    return source;
                };
                TReflectionVariableContainer<T>.getterByOffset.Add(offset, getter);
            }

            if(!TReflectionVariableContainer<T>.getterAsPtrByOffset.TryGetValue(offset, out var getterAsPtr)) {
                getterAsPtr = (void* ptr) => {
                    ref T source = ref UnsafeUtility.AsRef<T>((void*)((byte*)ptr + offset));
                    return PointerUtility.GetPointer<T>(source);
                };
                TReflectionVariableContainer<T>.getterAsPtrByOffset.Add(offset, getterAsPtr);
            }

            return new TReflectionVariable<T>(fieldInfo, setter, getter, getterAsPtr);
        }

        #endregion

        #region GetVariable For Class

        public static TReflectionVariable<T> GetVariableForClass<T>(FieldInfo fieldInfo, int offset) where T : class {
            var type = fieldInfo.FieldType;
            var fieldTypeIsClass = type.IsClass;
            if(!fieldTypeIsClass) {
                Debug.LogError(TAG + "Attempting to get variable for a class on a struct object!");
                return null;
            }

            if(!TReflectionVariableContainer<T>.setterByOffset.TryGetValue(offset, out var setter)) {
                setter = (void* ptr, T value) => *((IntPtr*)(((byte*)ptr) + offset)) = UnsafeUtility.As<T, IntPtr>(ref value);
                TReflectionVariableContainer<T>.setterByOffset.Add(offset, setter);
            }

            if(!TReflectionVariableContainer<T>.getterByOffset.TryGetValue(offset, out var getter)) {
                getter = (void* ptr) => {
                    IntPtr rawRef = *((IntPtr*)(((byte*)ptr) + offset));
                    return UnsafeUtility.As<IntPtr, T>(ref rawRef);
                };
                TReflectionVariableContainer<T>.getterByOffset.Add(offset, getter);
            }

            if(!TReflectionVariableContainer<T>.getterAsPtrByOffset.TryGetValue(offset, out var getterAsPtr)) {
                getterAsPtr = (void* ptr) => {
                    IntPtr rawRef = *((IntPtr*)(((byte*)ptr) + offset));
                    var t = UnsafeUtility.As<IntPtr, T>(ref rawRef);
                    return Toolkit.PointerUtility.GetPointer(t);
                };
                TReflectionVariableContainer<T>.getterAsPtrByOffset.Add(offset, getterAsPtr);
            }

            return new TReflectionVariable<T>(fieldInfo, setter, getter, getterAsPtr);
        }

        #endregion

        #region Getter / Setter Unmanaged

        public static SetterVariable<T> SetterUnmanaged<T>(int offset) where T : unmanaged {
            if(!TReflectionVariableContainer<T>.setterByOffset.TryGetValue(offset, out var setter)) {
                setter = (void* ptr, T value) => *((T*)(((byte*)ptr) + offset)) = value;
                TReflectionVariableContainer<T>.setterByOffset.Add(offset, setter);
            }
            return setter;
        }

        public static GetterVariable<T> GetterUnmanaged<T>(int offset) where T : unmanaged {
            if(!TReflectionVariableContainer<T>.getterByOffset.TryGetValue(offset, out var getter)) {
                getter = (void* ptr) => *((T*)(((byte*)ptr) + offset));
                TReflectionVariableContainer<T>.getterByOffset.Add(offset, getter);
            }
            return getter;
        }

        public static GetterVariableAsPtr GetterAsPtrUnmanaged<T>(int offset) where T : unmanaged {
            if(!TReflectionVariableContainer<T>.getterAsPtrByOffset.TryGetValue(offset, out var getter)) {
                getter = (void* ptr) => ((void*)(((byte*)ptr) + offset));
                TReflectionVariableContainer<T>.getterAsPtrByOffset.Add(offset, getter);
            }
            return getter;
        }

        #endregion

        #region Getter / Setter String

        public static SetterVariable<string> SetterString(int offset) {
            if(!TReflectionVariableContainer<string>.setterByOffset.TryGetValue(offset, out var setter)) {
                //setter = (void* ptr, string value) => *((IntPtr*)(((byte*)ptr) + offset)) = UnsafeUtility.As<string, IntPtr>(ref value);
                //setter = (void* ptr, string value) => Debug.Log(value);
                setter = (void* ptr, string value) => {
                    ref string target = ref Toolkit.Internal.UnsafeUtility.AsRef<string>((void*)((byte*)ptr + offset));
                    target = value;
                };
                TReflectionVariableContainer<string>.setterByOffset.Add(offset, setter);
            }
            return setter;
        }

        public static GetterVariable<string> GetterString(int offset) {
            if(!TReflectionVariableContainer<string>.getterByOffset.TryGetValue(offset, out var getter)) {
                //getter = (void* ptr) => {
                //    IntPtr rawRef = *((IntPtr*)(((byte*)ptr) + offset));
                //    return UnsafeUtility.As<IntPtr, string>(ref rawRef);
                //};
                //getter = (void* ptr) => "debug";
                getter = (void* ptr) => {
                    ref string source = ref Toolkit.Internal.UnsafeUtility.AsRef<string>((void*)((byte*)ptr + offset));
                    return source;
                };
                TReflectionVariableContainer<string>.getterByOffset.Add(offset, getter);
            }
            return getter;
        }

        public static GetterVariableAsPtr GetterAsPtrString(int offset) {
            if(!TReflectionVariableContainer<string>.getterAsPtrByOffset.TryGetValue(offset, out var getter)) {
                //getter = (void* ptr) => {
                //    IntPtr rawRef = *((IntPtr*)(((byte*)ptr) + offset));
                //    var t = UnsafeUtility.As<IntPtr, string>(ref rawRef);
                //    return Toolkit.PointerUtility.GetPointer(t);
                //};
                getter = (void* ptr) => {
                    ref string source = ref Toolkit.Internal.UnsafeUtility.AsRef<string>((void*)((byte*)ptr + offset));
                    return Toolkit.PointerUtility.GetPointer(source);
                };
                TReflectionVariableContainer<string>.getterAsPtrByOffset.Add(offset, getter);
            }
            return getter;
        }

        #endregion

        #region Util

        public static unsafe int GetFieldOffset(this FieldInfo field) {
            if(field.DeclaringType.IsValueType)
                return GetFieldOffsetForStruct(field);
            if(field.DeclaringType.IsClass)
                return GetFieldOffsetForClass(field);
            throw new Exception("Parent of FieldInfo is neither valuetype or class");
        }

        public static unsafe int GetFieldOffset(this FieldInfo field, bool isClass) {
            if(!isClass)
                return GetFieldOffsetForStruct(field);
            return GetFieldOffsetForClass(field);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetFieldOffsetForStruct(FieldInfo field) {
            var rhv = (IntPtr*)field.FieldHandle.Value;
            rhv += 3;
            return (*(int*)rhv - IntPtr.Size) - 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetFieldOffsetForClass(FieldInfo field) {
            var rhv = (IntPtr*)field.FieldHandle.Value;
            rhv += 3;
            return *(int*)rhv;
        }

        public static bool IsStruct(Type type)
            => SerializationUtility.IsStruct(type);

        public static bool IsClass(Type type)
            => SerializationUtility.IsClass(type);

        #endregion


        // TODO: TML & Binary Wrapper don't fully support arrays/lists of classes

        #region Create TML Wrappers

        public static TMLReflectionVariable CreateTMLWrapper(TReflectionVariable variable, FieldInfo fieldInfo) {
            try {
                var fieldType = fieldInfo.FieldType;
                switch(Type.GetTypeCode(fieldType)) {
                    case TypeCode.Byte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Byte b)
                                variable.As<byte>().Set(ptr, b.Byte);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<byte>().Get(ptr)); });
                    case TypeCode.SByte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_SByte b)
                                variable.As<sbyte>().Set(ptr, b.SByte);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<sbyte>().Get(ptr)); });
                    case TypeCode.Int16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Short b)
                                variable.As<short>().Set(ptr, b.Short);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<short>().Get(ptr)); });
                    case TypeCode.UInt16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UShort b)
                                variable.As<ushort>().Set(ptr, b.UShort);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<ushort>().Get(ptr)); });
                    case TypeCode.Int32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Int b)
                                variable.As<int>().Set(ptr, b.Int);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<int>().Get(ptr)); });
                    case TypeCode.UInt32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UInt b)
                                variable.As<uint>().Set(ptr, b.UInt);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<uint>().Get(ptr)); });
                    case TypeCode.Int64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Long b)
                                variable.As<long>().Set(ptr, b.Long);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<long>().Get(ptr)); });
                    case TypeCode.UInt64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_ULong b)
                                variable.As<ulong>().Set(ptr, b.ULong);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<ulong>().Get(ptr)); });

                    case TypeCode.Single:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Float b)
                                variable.As<float>().Set(ptr, b.Float);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<float>().Get(ptr)); });
                    case TypeCode.Double:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Double b)
                                variable.As<double>().Set(ptr, b.Double);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<double>().Get(ptr)); });
                    case TypeCode.Decimal:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Decimal b)
                                variable.As<decimal>().Set(ptr, b.Decimal);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<decimal>().Get(ptr)); });

                    case TypeCode.Boolean:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Boolean b)
                                variable.As<bool>().Set(ptr, b.Boolean);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<bool>().Get(ptr)); });
                    case TypeCode.Char:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Char b)
                                variable.As<char>().Set(ptr, b.Char);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<char>().Get(ptr)); });
                    case TypeCode.String:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_String b)
                                variable.As<string>().Set(ptr, b.String);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<string>().Get(ptr)); });

                    case TypeCode.DateTime:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_TKDateTime b)
                                variable.As<DateTime>().Set(ptr, b.DateTime);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<DateTime>().Get(ptr)); });
                    case TypeCode.Object: {
                            if(fieldType.IsArray) {
                                return CreateTMLWrapperForArrays(variable, fieldType.GetElementType());
                            }
                            else if(SerializationUtility.IsList(fieldType, out var elementType)) {
                                return CreateTMLWrapperForList(variable, elementType);
                            }
                        }
                        return null;
                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        public static TMLReflectionVariable CreateTMLWrapperForArrays(TReflectionVariable variable, Type fieldType) {
            try {
                switch(Type.GetTypeCode(fieldType)) {
                    case TypeCode.Byte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Byte_Array b)
                                variable.As<byte[]>().Set(ptr, b.Bytes.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<byte[]>().Get(ptr)); });
                    case TypeCode.SByte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_SByte_Array b)
                                variable.As<sbyte[]>().Set(ptr, b.SBytes.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<sbyte[]>().Get(ptr)); });
                    case TypeCode.Int16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Short_Array b)
                                variable.As<short[]>().Set(ptr, b.Shorts.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<short[]>().Get(ptr)); });
                    case TypeCode.UInt16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UShort_Array b)
                                variable.As<ushort[]>().Set(ptr, b.UShorts.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<ushort[]>().Get(ptr)); });
                    case TypeCode.Int32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Int_Array b)
                                variable.As<int[]>().Set(ptr, b.Ints.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<int[]>().Get(ptr)); });
                    case TypeCode.UInt32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UInt_Array b)
                                variable.As<uint[]>().Set(ptr, b.UInts.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<uint[]>().Get(ptr)); });
                    case TypeCode.Int64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Long_Array b)
                                variable.As<long[]>().Set(ptr, b.Longs.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<long[]>().Get(ptr)); });
                    case TypeCode.UInt64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_ULong_Array b)
                                variable.As<ulong[]>().Set(ptr, b.ULongs.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<ulong[]>().Get(ptr)); });

                    case TypeCode.Single:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Float_Array b)
                                variable.As<float[]>().Set(ptr, b.Floats.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<float[]>().Get(ptr)); });
                    case TypeCode.Double:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Double_Array b)
                                variable.As<double[]>().Set(ptr, b.Doubles.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<double[]>().Get(ptr)); });
                    case TypeCode.Decimal:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Decimal_Array b)
                                variable.As<decimal[]>().Set(ptr, b.Decimals.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<decimal[]>().Get(ptr)); });

                    case TypeCode.Boolean:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Boolean_Array b)
                                variable.As<bool[]>().Set(ptr, b.Booleans.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<bool[]>().Get(ptr)); });
                    case TypeCode.Char:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Char_Array b)
                                variable.As<char[]>().Set(ptr, b.Chars.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<char[]>().Get(ptr)); });
                    case TypeCode.String:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_String_Array b)
                                variable.As<string[]>().Set(ptr, b.Strings.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<string[]>().Get(ptr)); });

                    case TypeCode.DateTime:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_DateTime_Array b)
                                variable.As<DateTime[]>().Set(ptr, b.DateTimes.ToArray());
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<DateTime[]>().Get(ptr)); });
                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        public static TMLReflectionVariable CreateTMLWrapperForList(TReflectionVariable variable, Type elementType) {
            try {
                switch(Type.GetTypeCode(elementType)) {
                    case TypeCode.Byte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Byte_Array b)
                                variable.As<List<byte>>().Set(ptr, (List<byte>)b.Bytes);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<byte>>().Get(ptr)); });
                    case TypeCode.SByte:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_SByte_Array b)
                                variable.As<List<sbyte>>().Set(ptr, (List<sbyte>)b.SBytes);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<sbyte>>().Get(ptr)); });
                    case TypeCode.Int16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Short_Array b)
                                variable.As<List<short>>().Set(ptr, (List<short>)b.Shorts);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<short>>().Get(ptr)); });
                    case TypeCode.UInt16:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UShort_Array b)
                                variable.As<List<ushort>>().Set(ptr, (List<ushort>)b.UShorts);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<ushort>>().Get(ptr)); });
                    case TypeCode.Int32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Int_Array b)
                                variable.As<List<int>>().Set(ptr, (List<int>)b.Ints);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<int>>().Get(ptr)); });
                    case TypeCode.UInt32:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_UInt_Array b)
                                variable.As<List<uint>>().Set(ptr, (List<uint>)b.UInts);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<uint>>().Get(ptr)); });
                    case TypeCode.Int64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Long_Array b)
                                variable.As<List<long>>().Set(ptr, (List<long>)b.Longs);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<long>>().Get(ptr)); });
                    case TypeCode.UInt64:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_ULong_Array b)
                                variable.As<List<ulong>>().Set(ptr, (List<ulong>)b.ULongs);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<ulong>>().Get(ptr)); });

                    case TypeCode.Single:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Float_Array b)
                                variable.As<List<float>>().Set(ptr, (List<float>)b.Floats);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<float>>().Get(ptr)); });
                    case TypeCode.Double:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Double_Array b)
                                variable.As<List<double>>().Set(ptr, (List<double>)b.Doubles);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<double>>().Get(ptr)); });
                    case TypeCode.Decimal:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Decimal_Array b)
                                variable.As<List<decimal>>().Set(ptr, (List<decimal>)b.Decimals);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<decimal>>().Get(ptr)); });

                    case TypeCode.Boolean:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Boolean_Array b)
                                variable.As<List<bool>>().Set(ptr, (List<bool>)b.Booleans);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<bool>>().Get(ptr)); });
                    case TypeCode.Char:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_Char_Array b)
                                variable.As<List<char>>().Set(ptr, (List<char>)b.Chars);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<char>>().Get(ptr)); });
                    case TypeCode.String:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_String_Array b)
                                variable.As<List<string>>().Set(ptr, (List<string>)b.Strings);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<List<string>>().Get(ptr)); });

                    case TypeCode.DateTime:
                        return new TMLReflectionVariable(variable, (ptr, property) => {
                            if(property is TML.Properties.ITMLProperty_DateTime_Array b)
                                variable.As<List<DateTime>>().Set(ptr, (List<DateTime>)b.DateTimes);
                        }, (ptr, node) => { node.AddProperty(variable.Name, variable.As<DateTime[]>().Get(ptr)); });
                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }


        public static TMLReflectionVariableObject CreateTMLWrapperObject(TReflectionVariable variable, FieldInfo fieldInfo) {
            try {
                var fieldType = fieldInfo.FieldType;
                if(Type.GetTypeCode(fieldType) != TypeCode.Object)
                    return null;

                if(fieldType.IsArray) {
                    var fieldElement = fieldType.GetElementType();
                    if(SerializationUtility.IsBaseType(fieldElement))
                        return null;

                    var tmlserializer = TMLClassSerializer.Get(fieldElement);
                    var size = tmlserializer.ClassSerializer.ArraySizeOf;
                    if(IsStruct(fieldElement)) {
                        return new TMLReflectionVariableObject(variable, (ptr, node) => {
                            var count = node.Children.Count;
                            var array = tmlserializer.ClassSerializer.CreateArray(count) as Array;
                            variable.SetObject(ptr, array);
                            var arrayFirstElementPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                            int index = 0;
                            foreach(var c in node.Children) {
                                array.SetValue(tmlserializer.ClassSerializer.Create(), index);
                                var childPtr = (void*)(arrayFirstElementPtr + index * size);
                                tmlserializer.Deserialize(childPtr, c);
                                index++;
                            }
                        },
                        (ptr, node) => {
                            var array = variable.GetObject(ptr) as Array;
                            if(array == null)
                                return;
                            node = node.AddChild(variable.Name);
                            var arrayPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                            var len = array.Length;
                            for(var i = 0; i < len; i++) {
                                var childPtr = (void*)(arrayPtr + i * size);
                                var child = node.AddChild($"{i}");
                                tmlserializer.Serialize(childPtr, child);
                            }
                        });
                    }
                    else {
                        return new TMLReflectionVariableObject(variable, (ptr, node) => {
                            var count = node.Children.Count;
                            var array = tmlserializer.ClassSerializer.CreateArray(count) as Array;
                            variable.SetObject(ptr, array);
                            var arrayFirstElementPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                            int index = 0;
                            foreach(var c in node.Children) {
                                array.SetValue(tmlserializer.ClassSerializer.Create(), index);
                                var childPtr = *(void**)(arrayFirstElementPtr + index * size);
                                tmlserializer.Deserialize(childPtr, c);
                                index++;
                            }
                        },
                        (ptr, node) => {
                            var array = variable.GetObject(ptr) as Array;
                            if(array == null)
                                return;
                            node = node.AddChild(variable.Name);
                            var arrayPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                            var len = array.Length;
                            for(var i = 0; i < len; i++) {
                                var childPtr = *(void**)(arrayPtr + i * size);
                                var child = node.AddChild($"{i}");
                                tmlserializer.Serialize(childPtr, child);
                            }
                        });
                    }
                }
                else if(SerializationUtility.IsList(fieldType, out var fieldElement)) {
                    if(SerializationUtility.IsBaseType(fieldElement))
                        return null;

                    var listGenerator = ClassSerializer.Get(fieldType);
                    var tmlserializer = TMLClassSerializer.Get(fieldElement);
                    var size = SerializationUtility.CalculateArraySizeOf(fieldElement);

                    if(IsStruct(fieldElement)) {
                        return new TMLReflectionVariableObject(variable, (ptr, node) => {
                            var count = node.Children.Count;
                            var array = listGenerator.Create(count) as IList;
                            variable.SetObject(ptr, array);
                            var listPtr = variable.GetPtr(ptr);
                            var arrayPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                            var arrayFirstElementPtr = ((byte*)arrayPtr + SerializationUtility.FirstArrayElementOffset);
                            int index = 0;
                            foreach(var c in node.Children) {
                                array.Add(tmlserializer.ClassSerializer.Create());
                                var childPtr = (void*)(arrayFirstElementPtr + index * size);
                                tmlserializer.Deserialize(childPtr, c);
                                index++;
                            }
                        },
                        (ptr, node) => {
                            var array = variable.GetObject(ptr) as IList;
                            if(array == null)
                                return;
                            node = node.AddChild(variable.Name);
                            var listPtr = variable.GetPtr(ptr);
                            var arrPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                            var arrayPtr = ((byte*)arrPtr + SerializationUtility.FirstArrayElementOffset);

                            var len = array.Count;
                            for(var i = 0; i < len; i++) {
                                var childPtr = (void*)(arrayPtr + i * size);
                                var child = node.AddChild($"{i}");
                                tmlserializer.Serialize(childPtr, child);
                            }
                        });
                    }
                    else {

                        return new TMLReflectionVariableObject(variable, (ptr, node) => {
                            var count = node.Children.Count;
                            var array = listGenerator.Create(count) as IList;
                            variable.SetObject(ptr, array);
                            var listPtr = variable.GetPtr(ptr);
                            var arrayPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                            var arrayFirstElementPtr = ((byte*)arrayPtr + SerializationUtility.FirstArrayElementOffset);
                            int index = 0;
                            foreach(var c in node.Children) {
                                array.Add(tmlserializer.ClassSerializer.Create());
                                var childPtr = *(void**)(arrayFirstElementPtr + index * size);
                                tmlserializer.Deserialize(childPtr, c);
                                index++;
                            }
                        },
                        (ptr, node) => {
                            var array = variable.GetObject(ptr) as IList;
                            if(array == null)
                                return;
                            node = node.AddChild(variable.Name);
                            var listPtr = variable.GetPtr(ptr);
                            var arrPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                            var arrayPtr = ((byte*)arrPtr + SerializationUtility.FirstArrayElementOffset);

                            var len = array.Count;
                            for(var i = 0; i < len; i++) {
                                var childPtr = *(void**)(arrayPtr + i * size);
                                var child = node.AddChild($"{i}");
                                tmlserializer.Serialize(childPtr, child);
                            }
                        });
                    }
                }
                else {
                    var tmlserializer = TMLClassSerializer.Get(fieldType);
                    return new TMLReflectionVariableObject(variable, (ptr, node) => {
                        variable.SetObject(ptr, tmlserializer.ClassSerializer.Create());
                        var childPtr = variable.GetPtr(ptr);
                        tmlserializer.Deserialize(childPtr, node);
                    },
                    (ptr, node) => {
                        var childPtr = variable.GetPtr(ptr);
                        if(childPtr == null || ((IntPtr)childPtr) == IntPtr.Zero)
                            return;
                        var child = node.AddChild(variable.Name);
                        tmlserializer.Serialize(childPtr, child);
                    });
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }


        #endregion

        #region Create Binary Wrappers

        public static BinaryReflectionVariable CreateBinaryWrapper(TReflectionVariable variable, FieldInfo fieldInfo) {
            try {
                var fieldType = fieldInfo.FieldType;
                switch(Type.GetTypeCode(fieldType)) {
                    case TypeCode.Byte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadByte()),
                            (ptr, buffer) => buffer.Write(variable.Get<byte>(ptr)));
                    case TypeCode.SByte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadSByte()),
                            (ptr, buffer) => buffer.Write(variable.Get<sbyte>(ptr)));
                    case TypeCode.Int16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadShort()),
                            (ptr, buffer) => buffer.Write(variable.Get<short>(ptr)));
                    case TypeCode.UInt16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadUShort()),
                            (ptr, buffer) => buffer.Write(variable.Get<ushort>(ptr)));
                    case TypeCode.Int32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadCompressedInt()),
                            (ptr, buffer) => buffer.WriteCompressed(variable.Get<int>(ptr)));
                    case TypeCode.UInt32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadUInt()),
                            (ptr, buffer) => buffer.Write(variable.Get<uint>(ptr)));
                    case TypeCode.Int64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadLong()),
                            (ptr, buffer) => buffer.Write(variable.Get<long>(ptr)));
                    case TypeCode.UInt64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadULong()),
                            (ptr, buffer) => buffer.Write(variable.Get<ulong>(ptr)));

                    case TypeCode.Single:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadFloat()),
                            (ptr, buffer) => buffer.Write(variable.Get<float>(ptr)));
                    case TypeCode.Double:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadDouble()),
                            (ptr, buffer) => buffer.Write(variable.Get<double>(ptr)));
                    case TypeCode.Decimal:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadDecimal()),
                            (ptr, buffer) => buffer.Write(variable.Get<decimal>(ptr)));

                    case TypeCode.Boolean:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadBool()),
                            (ptr, buffer) => buffer.Write(variable.Get<bool>(ptr)));
                    case TypeCode.Char:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadChar()),
                            (ptr, buffer) => buffer.Write(variable.Get<char>(ptr)));
                    case TypeCode.String:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<string>().Set(ptr, buffer.ReadString()),
                            (ptr, buffer) => buffer.Write(variable.Get<string>(ptr)));

                    case TypeCode.DateTime:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.Set(ptr, buffer.ReadDateTime()),
                            (ptr, buffer) => buffer.Write(variable.Get<DateTime>(ptr)));

                    case TypeCode.Object:

                        if(fieldType.IsArray) {
                            return CreateBinaryWrapperForArrays(variable, fieldType.GetElementType());
                        }
                        else if(SerializationUtility.IsList(fieldType, out var elementType)) {
                            return CreateBinaryWrapperForList(variable, elementType);
                        }
                        else {
                            var binarySerializer = BinaryClassSerializer.Get(fieldType);
                            return new BinaryReflectionVariable(variable,
                                (ptr, buffer) => {
                                    var notNull = buffer.ReadBool();
                                    if(notNull) {
                                        variable.SetObject(ptr, binarySerializer.ClassSerializer.Create());
                                        var childPtr = variable.GetPtr(ptr);
                                        binarySerializer.DeserializeInternal(childPtr, buffer);
                                    }
                                },
                                (ptr, buffer) => {
                                    var childPtr = variable.GetPtr(ptr);
                                    if(childPtr == null || ((IntPtr)childPtr) == IntPtr.Zero) {
                                        buffer.Write(false);
                                        return;
                                    }
                                    buffer.Write(true);
                                    binarySerializer.SerializeInternal(childPtr, buffer);
                                });
                        }
                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }


        public static BinaryReflectionVariable CreateBinaryWrapperForArrays(TReflectionVariable variable, Type fieldType) {
            try {
                switch(Type.GetTypeCode(fieldType)) {
                    case TypeCode.Byte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<byte[]>().Set(ptr, buffer.ReadArray<byte>()),
                            (ptr, buffer) => buffer.Write(variable.As<byte[]>().Get(ptr)));
                    case TypeCode.SByte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<sbyte[]>().Set(ptr, buffer.ReadArray<sbyte>()),
                            (ptr, buffer) => buffer.Write(variable.As<sbyte[]>().Get(ptr)));
                    case TypeCode.Int16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<short[]>().Set(ptr, buffer.ReadArray<short>()),
                            (ptr, buffer) => buffer.Write(variable.As<short[]>().Get(ptr)));
                    case TypeCode.UInt16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<ushort[]>().Set(ptr, buffer.ReadArray<ushort>()),
                            (ptr, buffer) => buffer.Write(variable.As<ushort[]>().Get(ptr)));
                    case TypeCode.Int32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<int[]>().Set(ptr, buffer.ReadCompressedArrayInt()),
                            (ptr, buffer) => buffer.WriteCompressed(variable.As<int[]>().Get(ptr)));
                    case TypeCode.UInt32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<uint[]>().Set(ptr, buffer.ReadArray<uint>()),
                            (ptr, buffer) => buffer.Write(variable.As<uint[]>().Get(ptr)));
                    case TypeCode.Int64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<long[]>().Set(ptr, buffer.ReadArray<long>()),
                            (ptr, buffer) => buffer.Write(variable.As<long[]>().Get(ptr)));
                    case TypeCode.UInt64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<ulong[]>().Set(ptr, buffer.ReadArray<ulong>()),
                            (ptr, buffer) => buffer.Write(variable.As<ulong[]>().Get(ptr)));

                    case TypeCode.Single:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<float[]>().Set(ptr, buffer.ReadArray<float>()),
                            (ptr, buffer) => buffer.Write(variable.As<float[]>().Get(ptr)));
                    case TypeCode.Double:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<double[]>().Set(ptr, buffer.ReadArray<double>()),
                            (ptr, buffer) => buffer.Write(variable.As<double[]>().Get(ptr)));
                    case TypeCode.Decimal:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<decimal[]>().Set(ptr, buffer.ReadArray<decimal>()),
                            (ptr, buffer) => buffer.Write(variable.As<decimal[]>().Get(ptr)));

                    case TypeCode.Boolean:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<bool[]>().Set(ptr, buffer.ReadArray<bool>()),
                            (ptr, buffer) => buffer.Write(variable.As<bool[]>().Get(ptr)));
                    case TypeCode.Char:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<char[]>().Set(ptr, buffer.ReadArray<char>()),
                            (ptr, buffer) => buffer.Write(variable.As<char[]>().Get(ptr)));
                    case TypeCode.String:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<string[]>().Set(ptr, buffer.ReadArray<string>()),
                            (ptr, buffer) => buffer.Write(variable.As<string[]>().Get(ptr)));

                    case TypeCode.DateTime:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => variable.As<DateTime[]>().Set(ptr, buffer.ReadArray<DateTime>()),
                            (ptr, buffer) => buffer.Write(variable.As<DateTime[]>().Get(ptr)));

                    case TypeCode.Object: {
                            var binarySerializer = BinaryClassSerializer.Get(fieldType);
                            var size = binarySerializer.ClassSerializer.ArraySizeOf;
                            if(IsStruct(fieldType)) {
                                return new BinaryReflectionVariable(variable, (ptr, buffer) => {
                                    var count = buffer.ReadCompressedInt();
                                    var array = binarySerializer.ClassSerializer.CreateArray(count) as Array;
                                    variable.SetObject(ptr, array);
                                    var arrayFirstElementPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                                    for(int i = 0; i < count; i++) {
                                        array.SetValue(binarySerializer.ClassSerializer.Create(), i);
                                        var childPtr = (void*)(arrayFirstElementPtr + i * size);
                                        binarySerializer.Deserialize(childPtr, buffer);
                                    }
                                },
                                (ptr, buffer) => {
                                    var array = variable.GetObject(ptr) as Array;
                                    if(array == null)
                                        return;
                                    var arrayPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                                    var len = array.Length;
                                    buffer.WriteCompressed(len);
                                    for(var i = 0; i < len; i++) {
                                        var childPtr = (void*)(arrayPtr + i * size);
                                        binarySerializer.Serialize(childPtr, buffer);
                                    }
                                });
                            }
                            else {
                                return new BinaryReflectionVariable(variable, (ptr, buffer) => {
                                    var count = buffer.ReadCompressedInt();
                                    var array = binarySerializer.ClassSerializer.CreateArray(count) as Array;
                                    variable.SetObject(ptr, array);
                                    var arrayFirstElementPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                                    for(int i = 0; i < count; i++) {
                                        array.SetValue(binarySerializer.ClassSerializer.Create(), i);
                                        var childPtr = *(void**)(arrayFirstElementPtr + i * size);
                                        binarySerializer.Deserialize(childPtr, buffer);
                                    }
                                },
                                (ptr, buffer) => {
                                    var array = variable.GetObject(ptr) as Array;
                                    if(array == null)
                                        return;
                                    var arrayPtr = ((byte*)variable.GetPtr(ptr) + SerializationUtility.FirstArrayElementOffset);
                                    var len = array.Length;
                                    buffer.WriteCompressed(len);
                                    for(var i = 0; i < len; i++) {
                                        var childPtr = *(void**)(arrayPtr + i * size);
                                        binarySerializer.Serialize(childPtr, buffer);
                                    }
                                });
                            }
                        }

                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }

        public static BinaryReflectionVariable CreateBinaryWrapperForList(TReflectionVariable variable, Type elementType) {
            try {
                switch(Type.GetTypeCode(elementType)) {
                    case TypeCode.Byte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<byte> list = new List<byte>();
                                buffer.ReadArray(list);
                                variable.As<List<byte>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<byte>>().Get(ptr)));
                    case TypeCode.SByte:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<sbyte> list = new List<sbyte>();
                                buffer.ReadArray(list);
                                variable.As<List<sbyte>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<sbyte>>().Get(ptr)));
                    case TypeCode.Int16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<short> list = new List<short>();
                                buffer.ReadArray(list);
                                variable.As<List<short>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<short>>().Get(ptr)));
                    case TypeCode.UInt16:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<ushort> list = new List<ushort>();
                                buffer.ReadArray(list);
                                variable.As<List<ushort>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<ushort>>().Get(ptr)));
                    case TypeCode.Int32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<int> list = new List<int>();
                                buffer.ReadArray(list);
                                variable.As<List<int>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<int>>().Get(ptr)));
                    case TypeCode.UInt32:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<uint> list = new List<uint>();
                                buffer.ReadArray(list);
                                variable.As<List<uint>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<uint>>().Get(ptr)));
                    case TypeCode.Int64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<long> list = new List<long>();
                                buffer.ReadArray(list);
                                variable.As<List<long>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<long>>().Get(ptr)));
                    case TypeCode.UInt64:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<ulong> list = new List<ulong>();
                                buffer.ReadArray(list);
                                variable.As<List<ulong>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<ulong>>().Get(ptr)));

                    case TypeCode.Single:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<float> list = new List<float>();
                                buffer.ReadArray(list);
                                variable.As<List<float>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<float>>().Get(ptr)));
                    case TypeCode.Double:
                        return new BinaryReflectionVariable(variable,
                           (ptr, buffer) => {
                               List<double> list = new List<double>();
                               buffer.ReadArray(list);
                               variable.As<List<double>>().Set(ptr, list);
                           },
                            (ptr, buffer) => buffer.Write(variable.As<List<double>>().Get(ptr)));
                    case TypeCode.Decimal:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<decimal> list = new List<decimal>();
                                buffer.ReadArray(list);
                                variable.As<List<decimal>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<decimal>>().Get(ptr)));

                    case TypeCode.Boolean:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<bool> list = new List<bool>();
                                buffer.ReadArray(list);
                                variable.As<List<bool>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<bool>>().Get(ptr)));
                    case TypeCode.Char:
                        return new BinaryReflectionVariable(variable,
                            (ptr, buffer) => {
                                List<char> list = new List<char>();
                                buffer.ReadArray(list);
                                variable.As<List<char>>().Set(ptr, list);
                            },
                            (ptr, buffer) => buffer.Write(variable.As<List<char>>().Get(ptr)));
                    case TypeCode.String:
                        return new BinaryReflectionVariable(variable,
                             (ptr, buffer) => {
                                 List<string> list = new List<string>();
                                 buffer.ReadArray(list);
                                 variable.As<List<string>>().Set(ptr, list);
                             },
                            (ptr, buffer) => buffer.Write(variable.As<List<string>>().Get(ptr)));

                    case TypeCode.DateTime:
                        return new BinaryReflectionVariable(variable,
                             (ptr, buffer) => {
                                 List<DateTime> list = new List<DateTime>();
                                 buffer.ReadArray(list);
                                 variable.As<List<DateTime>>().Set(ptr, list);
                             },
                            (ptr, buffer) => buffer.Write(variable.As<List<DateTime>>().Get(ptr)));


                    case TypeCode.Object: {
                            var listGenerator = ClassSerializer.Get(variable.FieldInfo.FieldType);
                            var binarySerializer = BinaryClassSerializer.Get(elementType);
                            var size = SerializationUtility.CalculateArraySizeOf(elementType);

                            if(IsStruct(elementType)) {
                                return new BinaryReflectionVariable(variable, (ptr, buffer) => {
                                    var count = buffer.ReadCompressedInt();
                                    var array = listGenerator.Create(count) as IList;
                                    variable.SetObject(ptr, array);
                                    var listPtr = variable.GetPtr(ptr);
                                    var arrayPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                                    var arrayFirstElementPtr = ((byte*)arrayPtr + SerializationUtility.FirstArrayElementOffset);
                                    for(var i = 0; i < count; i++) {
                                        array.Add(binarySerializer.ClassSerializer.Create());
                                        var childPtr = (void*)(arrayFirstElementPtr + i * size);
                                        binarySerializer.Deserialize(childPtr, buffer);
                                    }
                                },
                                (ptr, buffer) => {
                                    var array = variable.GetObject(ptr) as IList;
                                    if(array == null)
                                        return;

                                    var listPtr = variable.GetPtr(ptr);
                                    var arrPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                                    var arrayPtr = ((byte*)arrPtr + SerializationUtility.FirstArrayElementOffset);

                                    var len = array.Count;
                                    buffer.WriteCompressed(len);
                                    for(var i = 0; i < len; i++) {
                                        var childPtr = (void*)(arrayPtr + i * size);
                                        binarySerializer.Serialize(childPtr, buffer);
                                    }
                                });
                            }
                            else {
                                return new BinaryReflectionVariable(variable, (ptr, buffer) => {
                                    var count = buffer.ReadCompressedInt();
                                    var array = listGenerator.Create(count) as IList;
                                    variable.SetObject(ptr, array);
                                    var listPtr = variable.GetPtr(ptr);
                                    var arrayPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                                    var arrayFirstElementPtr = ((byte*)arrayPtr + SerializationUtility.FirstArrayElementOffset);
                                    for(var i = 0; i < count; i++) {
                                        array.Add(binarySerializer.ClassSerializer.Create());
                                        var childPtr = *(void**)(arrayFirstElementPtr + i * size);
                                        binarySerializer.Deserialize(childPtr, buffer);
                                    }
                                },
                                (ptr, buffer) => {
                                    var array = variable.GetObject(ptr) as IList;
                                    if(array == null)
                                        return;

                                    var listPtr = variable.GetPtr(ptr);
                                    var arrPtr = *((void**)((byte*)listPtr + SerializationUtility.ListBackingFieldOffset));
                                    var arrayPtr = ((byte*)arrPtr + SerializationUtility.FirstArrayElementOffset);

                                    var len = array.Count;
                                    buffer.WriteCompressed(len);
                                    for(var i = 0; i < len; i++) {
                                        var childPtr = *(void**)(arrayPtr + i * size);
                                        binarySerializer.Serialize(childPtr, buffer);
                                    }
                                });
                            }
                        }
                    default: return null;
                }
            }
            catch(Exception e) {
                Debug.LogException(e);
                return null;
            }
        }


        #endregion
    }
}
