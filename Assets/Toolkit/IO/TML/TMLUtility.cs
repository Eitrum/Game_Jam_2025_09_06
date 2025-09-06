using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML {
    public static class TMLUtility {

        #region Variables

        public const string COLOR_TAG = "<color=#808000>";

        internal static Dictionary<string, TMLNode> DebugNodes = new Dictionary<string, TMLNode>();

        #endregion

        #region Debugging

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Debug(string id, TMLNode node) {
            DebugNodes[id] = node;
        }

        #endregion

        #region Hashing

        public static byte GetHash8(string str) {
            unchecked {
                int hash1 = (531 << 3) + 531;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return (byte)((hash1 + (hash2 * 1566083941)) >> (3));
            }
        }

        public static ushort GetHash16(string str) {
            unchecked {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return (ushort)((hash1 + (hash2 * 1566083941)));
            }
        }

        public static uint GetHash32(string str) {
            unchecked {
                uint hash1 = (5381 << 16) + 5381;
                uint hash2 = hash1;

                for(int i = 0; i < str.Length; i += 2) {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if(i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        #endregion

        #region Find Node

        public static bool FindNodeByPath(TMLNode node, string path, out TMLNode result) {
            var paths = path.Split('\\','/');
            if(paths.Length == 0) {
                result = null;
                return false;
            }
            int start = paths[0] == "root" ? 1: 0;
            result = null;
            for(int i = start, length = paths.Length; i < length; i++) {
                if(!node.TryGetNode(paths[i], out node))
                    return false;
            }
            result = node;
            return true;
        }

        public static bool FindNodeByDeepSearch(TMLNode node, string nodeName, out TMLNode result) {
            string outputPath = string.Empty;
            return Internal_FindNodeByDeepSearch(node, nodeName, ref outputPath, out result);
        }
        public static bool FindNodeByDeepSearch(TMLNode node, string nodeName, out string outputPath, out TMLNode result) {
            string outputPath2 = string.Empty;
            var res = Internal_FindNodeByDeepSearch(node, nodeName, ref outputPath2, out result);
            outputPath = outputPath2;
            return res;
        }

        private static bool Internal_FindNodeByDeepSearch(TMLNode node, string nodeName, ref string outputPath, out TMLNode result) {
            if(node.IsName(nodeName)) {
                result = node;
                outputPath = $"{node.Name}";
                return true;
            }
            foreach(TMLNode child in node.Children) {
                if(Internal_FindNodeByDeepSearch(child, nodeName, ref outputPath, out result)) {
                    outputPath = $"{node.Name}/{outputPath}";
                    return true;
                }
            }

            result = null;
            return false;
        }

        #endregion

        #region Create Generic Property

        public static Properties.ITMLProperty CreateProperty<T>(string name, T value) {
            return value switch {
                bool boolValue => new Properties.TMLProperty_Boolean(name, boolValue),
                byte byteValue => new Properties.TMLProperty_Byte(name, byteValue),
                sbyte sbyteValue => new Properties.TMLProperty_SByte(name, sbyteValue),
                short shortValue => new Properties.TMLProperty_Short(name, shortValue),
                ushort ushortValue => new Properties.TMLProperty_UShort(name, ushortValue),
                int intValue => new Properties.TMLProperty_Int(name, intValue),
                uint uintValue => new Properties.TMLProperty_UInt(name, uintValue),
                long longValue => new Properties.TMLProperty_Long(name, longValue),
                ulong ulongValue => new Properties.TMLProperty_ULong(name, ulongValue),
                float floatValue => new Properties.TMLProperty_Float(name, floatValue),
                double doubleValue => new Properties.TMLProperty_Double(name, doubleValue),
                decimal decimalValue => new Properties.TMLProperty_Decimal(name, decimalValue),
                DateTime dateTimeValue => new Properties.TMLProperty_DateTime(name, dateTimeValue),
                string stringValue => new Properties.TMLProperty_String(name, stringValue),
                Enum enumValue => new Properties.TMLProperty_Int(name, enumValue.ToInt()),
                _ => throw new NotImplementedException(),
            };
        }

        public static T GetPropertyValue<T>(TMLNode node, string propertyName) {
            return TryGetPropertyValue(node, propertyName, out T value) ? value : default;
        }

        public static bool TryGetPropertyValue<T>(TMLNode node, string propertyName, out T value) {
            value = default;
            if(!node.TryGetProperty(propertyName, out Properties.ITMLProperty property)) {
                return false;
            }

            if(value is null) {
                if(typeof(T) == typeof(string)) {
                    if(property is Properties.ITMLProperty_String stringprop) {
                        value = (T)(object)stringprop.String;
                        return true;
                    }
                    return false;
                }
                throw new NotImplementedException();
            }

            switch(value) {
                case bool:
                    if(property is Properties.ITMLProperty_Boolean boolprop) {
                        value = (T)(object)boolprop.Boolean;
                        return true;
                    }
                    break;
                case byte:
                    if(property is Properties.ITMLProperty_Byte byteprop) {
                        value = (T)(object)byteprop.Byte;
                        return true;
                    }
                    break;
                case sbyte:
                    if(property is Properties.ITMLProperty_SByte sbyteprop) {
                        value = (T)(object)sbyteprop.SByte;
                        return true;
                    }
                    break;
                case short:
                    if(property is Properties.ITMLProperty_Short shortprop) {
                        value = (T)(object)shortprop.Short;
                        return true;
                    }
                    break;
                case ushort:
                    if(property is Properties.ITMLProperty_UShort ushortprop) {
                        value = (T)(object)ushortprop.UShort;
                        return true;
                    }
                    break;
                case int:
                    if(property is Properties.ITMLProperty_Int intprop) {
                        value = (T)(object)intprop.Int;
                        return true;
                    }
                    break;
                case uint:
                    if(property is Properties.ITMLProperty_UInt uintprop) {
                        value = (T)(object)uintprop.UInt;
                        return true;
                    }
                    break;
                case long:
                    if(property is Properties.ITMLProperty_Long longprop) {
                        value = (T)(object)longprop.Long;
                        return true;
                    }
                    break;
                case ulong:
                    if(property is Properties.ITMLProperty_ULong ulongprop) {
                        value = (T)(object)ulongprop.ULong;
                        return true;
                    }
                    break;
                case float:
                    if(property is Properties.ITMLProperty_Float floatprop) {
                        value = (T)(object)floatprop.Float;
                        return true;
                    }
                    break;
                case double:
                    if(property is Properties.ITMLProperty_Double doubleprop) {
                        value = (T)(object)doubleprop.Double;
                        return true;
                    }
                    break;
                case decimal:
                    if(property is Properties.ITMLProperty_Decimal decimalprop) {
                        value = (T)(object)decimalprop.Decimal;
                        return true;
                    }
                    break;
                case DateTime:
                    if(property is Properties.ITMLProperty_TKDateTime dateTimeprop) {
                        value = (T)(object)dateTimeprop.DateTime;
                        return true;
                    }
                    break;
                case Enum:
                    if(property is Properties.ITMLProperty_Int intenumprop) {
                        value = (T)Convert.ChangeType(intenumprop.Int, FastEnum.GetUnderlyingType(typeof(T)));
                        return true;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return false;
        }

        #endregion
    }
}
