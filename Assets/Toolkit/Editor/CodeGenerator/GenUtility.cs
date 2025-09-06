using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toolkit.CodeGenerator {
    public static class GenUtility {

        internal const string INDENT = "\t";
        internal const string NEWLINE = "\n";

        public static string Indent(int indentValue) {
            string result = "";
            for(int i = 0; i < indentValue; i++) {
                result += INDENT;
            }
            return result;
        }

        /// <summary>
        /// A way to determine if a name is a valid code.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">Must be a positive value or 0 for it to work correctly.</param>
        /// <returns></returns>
        public static string VerifyName(string name, int id = -1) {
            if(name == null) {
                throw new Exception("name variable provided is null");
            }
            if(string.IsNullOrEmpty(name.Trim())) {
                return "_" + (id >= 0 ? id.ToString() : "");
            }
            if(char.IsNumber(name[0])) {
                name = "_" + name;
            }
            // A check to verify no names will clash with c# rules. This is not 100% complete and more need to be added.
            if((name[0] == '_' || name[0] == '@') == false &&
                (name == "class" || name == "namespace" ||
                name == "ref" || name == "enum" || name == "int" || name == "float" ||
                name == "string" || name == "byte" || name == "short" || name == "uint" ||
                name == "sbyte" || name == "ushort" || name == "ulong" || name == "double" ||
                name == "decimal" || (Enum.TryParse<AccessModifier>(name.ToUpper(0), out AccessModifier mod) && mod != AccessModifier.None) || System.Type.GetType(name) != null)) {
                name = "@" + name;
            }
            return name.Replace(' ', '_').Replace('-', '_');
        }

        public static string Generate(this AccessModifier security) {
            if(security == AccessModifier.None)
                return "";
            return Regex.Replace(
                 Regex.Replace(
                     security.ToString().Replace(", ", ""),
                     @"(\P{Ll})(\P{Ll}\p{Ll})",
                     "$1 $2"
                 ),
                 @"(\p{Ll})(\P{Ll})",
                 "$1 $2"
             ).ToLower() + " ";
        }

        public static string GenerateParameterAccessor(this AccessModifier security) {
            if(IsParameterAccessorType(security)) {
                return Generate(security).ToLower();
            }
            return "";
        }

        public static bool IsStatic(this AccessModifier accessModifier) {
            return accessModifier.HasFlag(AccessModifier.Static) || accessModifier.HasFlag(AccessModifier.Const);
        }

        public static AccessModifier CalculateRestrictivePriority(AccessModifier accessModifier0, AccessModifier accessModifier1) {
            return accessModifier0 > accessModifier1 ? accessModifier0 : accessModifier1;
        }

        /// <summary>
        /// Not complete and not tested
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AccessModifier GetAccessModifier(Type type) {
            AccessModifier access = AccessModifier.None;
            if(type.IsPublic) {
                access |= AccessModifier.Public;
            }
            if(!type.IsVisible) {
                access |= AccessModifier.Internal;
            }

            if(type.IsAbstract && type.IsSealed) {
                access |= AccessModifier.Static;
            }
            else if(type.IsAbstract) {
                access |= AccessModifier.Abstract;
            }
            else if(type.IsSealed) {
                access |= AccessModifier.Sealed;
            }
            return access;
        }

        /// <summary>
        /// Not complete and not tested
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AccessModifier GetAccessModifier(System.Reflection.MethodInfo methodInfo) {
            AccessModifier access = AccessModifier.None;
            if(methodInfo.IsPublic) {
                access |= AccessModifier.Public;
            }
            if(methodInfo.IsPrivate) {
                access |= AccessModifier.Private;
            }
            if(methodInfo.IsStatic) {
                access |= AccessModifier.Static;
            }
            if(methodInfo.IsFamily) {
                access |= AccessModifier.Protected;
            }
            if(methodInfo.IsAssembly) {
                access |= AccessModifier.Internal;
            }
            return access;
        }

        public static string Process(this IReadOnlyList<ICode> array, int indent) {
            if(array.Count == 0)
                return "";
            string res = "";
            var count = array.Count - 1;
            for(int i = 0; i < count; i++) {
                res += $"{array[i].GetCode(indent)}{NEWLINE}";
            }
            return res + $"{array[count].GetCode(indent)}";
        }

        public static string ProcessParameterList(this IReadOnlyList<IVariable> variables) {
            string res = "";
            for(int i = 0, count = variables.Count; i < count; i++) {
                var defaultValue = variables[i].DefaultValue;
                var accessor = variables[i].AccessModifier;

                if(i + 1 < count)
                    res += $"{GenerateParameterAccessor(accessor)}{ variables[i].Type} {variables[i].Name}{(string.IsNullOrEmpty(defaultValue) ? "" : " = " + defaultValue)}, ";
                else
                    res += $"{GenerateParameterAccessor(accessor)}{ variables[i].Type} {variables[i].Name}{(string.IsNullOrEmpty(defaultValue) ? "" : " = " + defaultValue)}";
            }
            return res;
        }

        public static bool IsParameterAccessorType(AccessModifier modifier) {
            return modifier == AccessModifier.This || modifier == AccessModifier.Ref || modifier == AccessModifier.Out;
        }

        public static void AddRange<T>(this List<T> list, IReadOnlyList<T> otherArray) where T : ICode {
            if(otherArray == null || list == null)
                return;
            for(int i = 0, length = otherArray.Count; i < length; i++) {
                list.Add(otherArray[i]);
            }
        }

        public struct EnumValues {
            public string name;
            public long value;

            public EnumValues(string name, int value) {
                this.name = name;
                this.value = value;
            }

            public EnumValues(string name, byte value) {
                this.name = name;
                this.value = value;
            }

            public EnumValues(string name, long value) {
                this.name = name;
                this.value = value;
            }
        }

        public enum EnumType {
            Int,
            Byte,
            Short,
            UInt,
            UShort,
            SByte
        }

        public static CodeFile CreateEnumClass(string name, string @namespace, IReadOnlyList<EnumValues> enumValues) {
            return CreateEnumClass(name, @namespace, enumValues, EnumType.Int);
        }

        public static CodeFile CreateEnumClass(string name, string @namespace, IReadOnlyList<EnumValues> enumValues, EnumType enumType) {
            var codeFile = new CodeFile(name);
            var variableType = enumType.ToString().ToLower();
            var @enumClass = new CodeEnum(name,
                    enumValues
                    .Select(x => new CodeVariable(variableType, x.name, x.value.ToString()))
                    .ToArray());
            if(!string.IsNullOrEmpty(@namespace)) {
                var nspace = codeFile.AddNamespace(@namespace);
                nspace.AddEnum(@enumClass);
            }
            else {
                codeFile.AddEnum(@enumClass);
            }

            return codeFile;
        }
    }
}
