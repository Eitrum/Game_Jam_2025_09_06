using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Toolkit.CodeGenerator;
using UnityEditor;
using System;

namespace Toolkit.IO {
    public class BitConverterEditor {
        public static string name = "DefaultConverter";
        public static string path = "Assets/Toolkit/IO";

        public static void Generate() {
            Debug.Log(AccessModifier.Internal | AccessModifier.Unsafe);
            CodeFile file = new CodeFile(name, path);
            file.AddUsing("System.Collections.Generic");
            file.AddUsing("UnityEngine");
            var ns = file.AddNamespace("Toolkit.IO");
            var stat = ns.AddClass(new CodeClass("BitConverter", AccessModifier.PublicStatic | AccessModifier.Unsafe));
            var inter = ns.AddClass(new CodeClass("IBitConverter", AccessModifier.Internal | AccessModifier.Unsafe));
            var c = ns.AddClass(new CodeClass(name, AccessModifier.Internal | AccessModifier.Unsafe, new string[] { "IBitConverter" }));
            var cRev = ns.AddClass(new CodeClass("ReversedConverter", AccessModifier.Internal | AccessModifier.Unsafe, new string[] { "IBitConverter" }));

            AddMethods(stat, c, cRev, inter, "byte", 1);
            AddMethods(stat, c, cRev, inter, "sbyte", 1);
            AddMethods(stat, c, cRev, inter, "bool", 1);
            AddMethods(stat, c, cRev, inter, "short", 2);
            AddMethods(stat, c, cRev, inter, "ushort", 2);
            AddMethods(stat, c, cRev, inter, "uint", 4);
            AddMethods(stat, c, cRev, inter, "int", 4);
            AddMethods(stat, c, cRev, inter, "ulong", 8);
            AddMethods(stat, c, cRev, inter, "long", 8);
            AddMethods(stat, c, cRev, inter, "float", 4);
            AddMethods(stat, c, cRev, inter, "double", 8);
            AddMethods(stat, c, cRev, inter, "decimal", 16);
            AddMethods(stat, c, cRev, inter, "char", 2);
            AddMethods(stat, c, cRev, inter, "Vector2", 4, 4);
            AddMethods(stat, c, cRev, inter, "Vector3", 4, 4, 4);
            AddMethods(stat, c, cRev, inter, "Vector4", 4, 4, 4, 4);
            AddMethods(stat, c, cRev, inter, "Quaternion", 4, 4, 4, 4);

            file.CreateFile();
        }

        #region Interface generation

        private static void GenerateInterface(CodeClass inter, string type) {
            inter.AddCustom(new CodeCustom($"byte[] GetBytes({type} value);"));
            inter.AddCustom(new CodeCustom($"void GetBytes({type} value, byte* array);"));
            inter.AddCustom(new CodeCustom($"void GetBytes({type} value, IList<byte> array, int index);"));
            inter.AddCustom(new CodeCustom($"{type} To{FirstCharToUpper(type)}(byte* array);"));
            inter.AddCustom(new CodeCustom($"void ToValue(byte* array, out {type} oValue);"));
            inter.AddCustom(new CodeCustom($"{type} To{FirstCharToUpper(type)}(IList<byte> array, int index);"));
            inter.AddCustom(new CodeCustom($"void ToValue(IList<byte> array, int index, out {type} oValue);"));
            inter.AddCustom(new CodeCustom(""));
        }

        #endregion

        #region Static Class Generation

        private static void GenerateStatic(CodeClass stat, string type) {
            stat.AddCustom(new CodeCustom($"public static byte[] GetBytes({type} value) => converter.GetBytes(value);"));
            stat.AddCustom(new CodeCustom($"public static void GetBytes({type} value, byte* array) => converter.GetBytes(value, array);"));
            stat.AddCustom(new CodeCustom($"public static void GetBytes({type} value, IList<byte> array, int index) => converter.GetBytes(value, array, index);"));
            stat.AddCustom(new CodeCustom($"public static {type} To{FirstCharToUpper(type)}(byte* array) => converter.To{FirstCharToUpper(type)}(array);"));
            stat.AddCustom(new CodeCustom($"public static void ToValue(byte* array, out {type} oValue) => converter.ToValue(array, out oValue);"));
            stat.AddCustom(new CodeCustom($"public static {type} To{FirstCharToUpper(type)}(IList<byte> array, int index) => converter.To{FirstCharToUpper(type)}(array, index);"));
            stat.AddCustom(new CodeCustom($"public static void ToValue(IList<byte> array, int index, out {type} oValue) => converter.ToValue(array, index, out oValue);"));
            stat.AddCustom(new CodeCustom(""));
        }

        #endregion

        #region Help Methods

        public static int Sum(byte[] numbers) {
            var result = 0;
            for(int i = 0; i < numbers.Length; i++) {
                result += numbers[i];
            }
            return result;
        }

        public static string FirstCharToUpper(string input) {
            switch(input) {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));

                default: return char.ToUpper(input[0], System.Globalization.CultureInfo.CurrentCulture) + input.Substring(1);
            }
        }

        static void AddMethods(CodeClass stat, CodeClass c, CodeClass cRev, CodeClass inter, string type, params byte[] lengths) {
            AddDefaultGetBytes(c, type, false, lengths);
            AddUnsafeGetBytes(c, type, false, lengths);
            AddArrayGetBytes(c, type, false, lengths);

            AddToTypeUnsafe(c, type, false, lengths);
            AddToValueUnsafe(c, type, false, lengths);
            AddToTypeArray(c, type, false, lengths);
            AddToValueArray(c, type, false, lengths);


            AddDefaultGetBytes(cRev, type, true, lengths);
            AddUnsafeGetBytes(cRev, type, true, lengths);
            AddArrayGetBytes(cRev, type, true, lengths);

            AddToTypeUnsafe(cRev, type, true, lengths);
            AddToValueUnsafe(cRev, type, true, lengths);
            AddToTypeArray(cRev, type, true, lengths);
            AddToValueArray(cRev, type, true, lengths);


            GenerateInterface(inter, type);
            GenerateStatic(stat, type);
        }

        #endregion

        #region Add GetBytes

        static void AddDefaultGetBytes(CodeClass c, string type, bool reversed, params byte[] lengths) {
            List<string> code = new List<string>();
            code.Add("var val = (byte*)&value;");
            code.Add($"var bytes = new byte[{Sum(lengths)}];");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.Add($"bytes[{index + t}] = *(val + {index + (reversed ? l - (t + 1) : t)});");
                }
                index += l;
            }
            code.Add("return bytes;");

            var m = c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    typeof(byte[]),
                    "GetBytes",
                    new CodeVariable[] {
                        new CodeVariable(type, "value")
                    },
                    new CodeBlock(code)));
        }

        static void AddUnsafeGetBytes(CodeClass c, string type, bool reversed, params byte[] lengths) {
            List<string> code = new List<string>();
            code.Add("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.Add($"*(array + {index + t}) = *(val + {index + (reversed ? l - (t + 1) : t)});");
                }
                index += l;
            }

            var m = c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "GetBytes",
                    new CodeVariable[] {
                        new CodeVariable(type, "value"),
                        new CodeVariable("byte*", "array")
                    },
                    new CodeBlock(code)));
        }

        static void AddArrayGetBytes(CodeClass c, string type, bool reversed, params byte[] lengths) {
            List<string> code = new List<string>();
            code.Add("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.Add($"array[index + {index + t}] = *(val + {index + (reversed ? l - (t + 1) : t)});");
                }
                index += l;
            }

            var m = c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "GetBytes",
                    new CodeVariable[] {
                        new CodeVariable(type, "value"),
                        new CodeVariable("IList<byte>", "array"),
                        new CodeVariable("int", "index")
                    },
                    new CodeBlock(code)));
        }

        #endregion

        #region ToDataType

        // byte ToByte(byte* array, int index);
        // void ToValue(byte* array, int index, out byte value);
        // byte ToByte(IList<byte> array, int index);
        // void ToValue(IList<byte> array, int index, out byte value);

        static void AddToTypeUnsafe(CodeClass c, string type, bool reversed, params byte[] lengths) {
            var code = new CodeBlock();
            code.AddCode(type + " value = default;");
            code.AddCode("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.AddCode($"*(val + {index + t}) = *(array + {index + (reversed ? l - (t + 1) : t)});");
                }
                index += l;
            }
            code.AddCode("return value;");

            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    type,
                    "To" + FirstCharToUpper(type),
                    new CodeVariable[]{
                        new CodeVariable("byte*", "array")
                    },
                    code));
        }

        static void AddToValueUnsafe(CodeClass c, string type, bool reversed, params byte[] lengths) {
            var code = new CodeBlock();
            code.AddCode(type + " value = default;");
            code.AddCode("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.AddCode($"*(val + {index + t}) = *(array + {index + (reversed ? l - (t + 1) : t)});");
                }
                index += l;
            }
            code.AddCode("oValue = value;");

            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "ToValue",
                    new CodeVariable[]{
                        new CodeVariable("byte*", "array"),
                        new CodeVariable("out "+ type, "oValue")
                    },
                    code));
        }

        static void AddToTypeArray(CodeClass c, string type, bool reversed, params byte[] lengths) {
            var code = new CodeBlock();
            code.AddCode(type + " value = default;");
            code.AddCode("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.AddCode($"*(val + {index + t}) = array[index + {index + (reversed ? l - (t + 1) : t)}];");
                }
                index += l;
            }
            code.AddCode("return value;");

            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    type,
                    "To" + FirstCharToUpper(type),
                    new CodeVariable[]{
                        new CodeVariable("IList<byte>", "array"),
                        new CodeVariable("int", "index")
                    },
                    code));
        }

        static void AddToValueArray(CodeClass c, string type, bool reversed, params byte[] lengths) {
            var code = new CodeBlock();
            code.AddCode(type + " value = default;");
            code.AddCode("var val = (byte*)&value;");
            int index = 0;
            for(int i = 0; i < lengths.Length; i++) {
                var l = lengths[i];
                for(int t = 0; t < l; t++) {
                    code.AddCode($"*(val + {index + t}) = array[index + {index + (reversed ? l - (t + 1) : t)}];");
                }
                index += l;
            }
            code.AddCode("oValue = value;");

            c.AddMethod(
                new CodeMethod(
                    AccessModifier.Public,
                    "ToValue",
                    new CodeVariable[]{
                        new CodeVariable("IList<byte>", "array"),
                        new CodeVariable("int", "index"),
                        new CodeVariable("out "+ type, "oValue")
                    },
                    code));
        }

        #endregion
    }
}
