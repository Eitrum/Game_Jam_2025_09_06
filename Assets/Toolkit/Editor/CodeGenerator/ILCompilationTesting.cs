using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Toolkit.Internal {
    internal static class ILCompilationTesting {

        private static void Test() {

        }


        [MenuItem("Toolkit/Editor/Internal/IL Compilation Test")]
        private static void CompileDLL() {
            AssemblyName assemblyName = new AssemblyName("UnsafeUtility");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);

            // Generate internally visible to Toolkit
            ConstructorInfo internalsVisibleToCtor = typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute).GetConstructor(new Type[] { typeof(string) });
            CustomAttributeBuilder attrBuilder = new CustomAttributeBuilder(internalsVisibleToCtor,new object[] { "Toolkit" });
            assemblyBuilder.SetCustomAttribute(attrBuilder);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("UnsafeUtility", "UnsafeUtility.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Toolkit.Internal.UnsafeUtility", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

            CreateGetPointerClass(typeBuilder);
            CreateGetPointerObject(typeBuilder);
            CreateGetPointerStruct(typeBuilder);
            CreateGetPointerUnity(typeBuilder);

            CreateGetClassFromPtr(typeBuilder);
            CreateGetObjectFromPtr(typeBuilder);
            CreateGetStructFromPtr(typeBuilder);

            CreateUnsafeWrite(typeBuilder);
            CreateUnsafeRead(typeBuilder);
            
            CreateAsRef(typeBuilder);

            typeBuilder.CreateType();
            assemblyBuilder.Save("UnsafeUtility.dll");
        }

        private static void CreateGetPointerClass(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetPointerFromClass",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(void*),
                Type.EmptyTypes
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);
            var intype = gen.MakeByRefType();
            methodBuilder.SetParameters(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "obj");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ldind_I);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetPointerObject(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetPointerFromObject",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(void*),
                new Type[]{   typeof(object) }
            );

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "obj");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarga_S, 0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ldind_I);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetPointerStruct(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetPointerFromStruct",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(void*),
                Type.EmptyTypes
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);
            var intype = gen.MakeByRefType();
            methodBuilder.SetParameters(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "obj");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetPointerUnity(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetPointerFromUnityNative",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(void*),
                Type.EmptyTypes
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);
            var intype = gen.MakeByRefType();
            methodBuilder.SetParameters(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "obj");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ldind_I);
            il.Emit(OpCodes.Sizeof, typeof(System.IntPtr));
            il.Emit(OpCodes.Ldc_I4, 2);
            il.Emit(OpCodes.Mul);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Ldind_I);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetStructFromPtr(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetStructFromPointer",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                null,
                new Type[]{typeof(void*) }
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);
            var intype = gen.MakeByRefType();
            methodBuilder.SetReturnType(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetClassFromPtr(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetClassFromPointer",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                null,
                new Type[]{typeof(void*) }
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);
            var intype = gen.MakeByRefType();
            methodBuilder.SetReturnType(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldobj, gen);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateGetObjectFromPtr(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "GetObjectFromPointer",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(object),
                new Type[]{typeof(void*) }
            );

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarga_S, 0);
            il.Emit(OpCodes.Ldobj, typeof(object));
            il.Emit(OpCodes.Ret);
        }

        private static void CreateUnsafeWrite(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
        "Write",
        MethodAttributes.Public | MethodAttributes.Static,
        CallingConventions.Standard,
        typeof(void),
        new Type[] { typeof(void*), typeof(object) } // object will be unboxed
    );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "obj");

            ILGenerator il = methodBuilder.GetILGenerator();

            // Load pointer
            il.Emit(OpCodes.Ldarg_0);

            // Load object and unbox to T
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Unbox_Any, gen);

            // Store to pointer location
            il.Emit(OpCodes.Stobj, gen);

            il.Emit(OpCodes.Ret);
        }

        private static void CreateUnsafeRead(TypeBuilder typeBuilder) {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod( "Read",MethodAttributes.Public | MethodAttributes.Static,CallingConventions.Standard,typeof(object),    new Type[] { typeof(void*) });

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            gen.SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");

            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldobj, gen);
            il.Emit(OpCodes.Box, gen);
            il.Emit(OpCodes.Ret);
        }

        private static void CreateAsRef(TypeBuilder typeBuilder) {
            
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "AsRef",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                null,
                new Type[]{typeof(void*) }
            );

            var gen = methodBuilder.DefineGenericParameters("T")[0];
            var intype = gen.MakeByRefType();
            methodBuilder.SetReturnType(intype);
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "ptr");

            ILGenerator il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ret);
        }
    }
}
