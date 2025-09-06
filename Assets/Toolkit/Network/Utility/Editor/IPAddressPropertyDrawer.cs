using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Network
{
    [CustomPropertyDrawer(typeof(IPAddress), true)]
    public class IPAddressPropertyDrawer : PropertyDrawer
    {
        private static byte[] ipv4 = new byte[4];
        private static byte[] ipv6 = new byte[16];
        private static IPAddress addressCache;
        private bool doCustom = false;
        private static string[] presetValues;

        static IPAddressPropertyDrawer() {
            var temp = System.Enum.GetNames(typeof(IPAddress.Preset));
            presetValues = new string[temp.Length + 2];
            presetValues[0] = "Custom";
            presetValues[1] = "";
            for(int i = 0; i < temp.Length; i++)
                presetValues[2 + i] = temp[i];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var address = property.FindPropertyRelative("address");
            var isNone = property.FindPropertyRelative("isNone");
            var isValid = address.arraySize == 4 || address.arraySize == 16;
            var isIPv6 = address.arraySize == 16;
            if(!isValid) {

                return;
            }

            if(isIPv6) {
                for(int i = 0; i < 16; i++)
                    ipv6[i] = (byte)address.GetArrayElementAtIndex(i).intValue;
            }
            else {
                for(int i = 0; i < 4; i++)
                    ipv4[i] = (byte)address.GetArrayElementAtIndex(i).intValue;
            }

            addressCache = isIPv6 ? new IPAddress(ipv6) : new IPAddress(ipv4);
            //addressCache.IsNone = isNone;
            var preset = addressCache.GetPreset;
            if(preset == IPAddress.Preset.IPv6Any && isNone.boolValue)
                preset = IPAddress.Preset.IPv6None;
            if(preset == IPAddress.Preset.Broadcast && isNone.boolValue)
                preset = IPAddress.Preset.None;

            if((int)preset != -1 && !doCustom) {
                var index = (int)(preset) + 2;
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label.text, index, presetValues);
                if(EditorGUI.EndChangeCheck()) {
                    if(index == 0)
                        doCustom = true;
                    else {
                        addressCache = IPAddress.Create((IPAddress.Preset)(index - 2));
                        doCustom = false;
                    }
                    if(addressCache.IsIPv6) {
                        unsafe {
                            fixed(byte* p = &ipv6[0]) {
                                *((long*)p) = addressCache.IPv6.Item1;
                                *(((long*)p) + 1) = addressCache.IPv6.Item2;
                            }
                        }
                        address.arraySize = 16;
                        for(int i = 0; i < 16; i++)
                            address.GetArrayElementAtIndex(i).intValue = ipv6[i];
                    }
                    else {
                        unsafe {
                            fixed(byte* p = &ipv4[0])
                                *((int*)p) = addressCache.IPv4;
                        }
                        address.arraySize = 4;
                        for(int i = 0; i < 4; i++)
                            address.GetArrayElementAtIndex(i).intValue = ipv4[i];
                    }
                    isNone.boolValue = addressCache.IsNone;
                }
            }
            else {
                var labelArea = new Rect(position);
                labelArea.width -= isIPv6 ? 450f : 250f;
                var numberArea = new Rect(position);
                numberArea.x = position.width - (isIPv6 ? 450f : 250f);
                numberArea.width = 48; // Split it in 5

                EditorGUI.LabelField(labelArea, label.text);
                // Handle IPv4
                if(isIPv6) {
                    for(int i = 0; i < 8; i++) {
                        EditorGUI.BeginChangeCheck();
                        var res = EditorGUI.TextField(numberArea, ToHex(ipv6[i * 2], ipv6[i * 2 + 1]));
                        numberArea.x += 50f;
                        if(EditorGUI.EndChangeCheck()) {
                            if(res.Length == 0)
                                res = "0000";
                            else if(res.Length == 1)
                                res = "000" + res;
                            else if(res.Length == 2)
                                res = "00" + res;
                            else if(res.Length == 3)
                                res = "0" + res;
                            else if(res.Length > 4) {
                                if(System.Net.IPAddress.TryParse(res, out System.Net.IPAddress t)) {
                                    addressCache = new IPAddress(t);
                                    if(addressCache.IsIPv6) {
                                        res = "";
                                        unsafe {
                                            fixed(byte* p = &ipv6[0]) {
                                                *((long*)p) = addressCache.IPv6.Item1;
                                                *(((long*)p) + 1) = addressCache.IPv6.Item2;
                                            }
                                        }
                                        for(int x = 0; x < 8; x++)
                                            address.GetArrayElementAtIndex(x).intValue = ipv6[x];
                                    }
                                }
                                else
                                    res = res.Substring(0, 4);
                            }
                            if(IsHex(res)) {
                                if(i == 0 && res.Length == 32) {
                                    ipv6[0] = HexToByte(res, 0);
                                    ipv6[1] = HexToByte(res, 2);
                                    ipv6[2] = HexToByte(res, 4);
                                    ipv6[3] = HexToByte(res, 6);
                                    ipv6[4] = HexToByte(res, 8);
                                    ipv6[5] = HexToByte(res, 10);
                                    ipv6[6] = HexToByte(res, 12);
                                    ipv6[7] = HexToByte(res, 14);
                                    ipv6[8] = HexToByte(res, 16);
                                    ipv6[9] = HexToByte(res, 18);
                                    ipv6[10] = HexToByte(res, 20);
                                    ipv6[11] = HexToByte(res, 22);
                                    ipv6[12] = HexToByte(res, 24);
                                    ipv6[13] = HexToByte(res, 26);
                                    ipv6[14] = HexToByte(res, 28);
                                    ipv6[15] = HexToByte(res, 30);

                                    for(int x = 0; x < 16; x++)
                                        address.GetArrayElementAtIndex(x).intValue = ipv6[x];
                                }
                                else {
                                    ipv6[i * 2] = HexToByte(res, 0);
                                    ipv6[i * 2 + 1] = HexToByte(res, 2);
                                    address.GetArrayElementAtIndex(i * 2).intValue = ipv6[i * 2];
                                    address.GetArrayElementAtIndex(i * 2 + 1).intValue = ipv6[i * 2 + 1];
                                }
                            }
                        }
                    }
                }
                else {
                    EditorGUI.BeginChangeCheck();
                    for(int i = 0; i < 4; i++) {
                        ipv4[i] = (byte)Mathf.Clamp(EditorGUI.IntField(numberArea, (int)ipv4[i]), 0, 255);
                        numberArea.x += 50f;
                    }
                    if(EditorGUI.EndChangeCheck()) {
                        for(int i = 0; i < 4; i++)
                            address.GetArrayElementAtIndex(i).intValue = ipv4[i];
                        isNone.boolValue = false;
                    }
                }
                numberArea.x += 4f;
                if(isIPv6) {
                    if(GUI.Button(numberArea, "IPv4")) {
                        addressCache = IPAddress.Any;
                        unsafe {
                            fixed(byte* p = &ipv4[0])
                                *((int*)p) = addressCache.IPv4;
                        }
                        address.arraySize = 4;
                        for(int i = 0; i < 4; i++)
                            address.GetArrayElementAtIndex(i).intValue = ipv4[i];
                    }
                }
                else if(GUI.Button(numberArea, "IPv6")) {
                    addressCache = IPAddress.IPv6Any;
                    unsafe {
                        fixed(byte* p = &ipv6[0]) {
                            *((long*)p) = addressCache.IPv6.Item1;
                            *(((long*)p) + 1) = addressCache.IPv6.Item2;
                        }
                    }
                    address.arraySize = 16;
                    for(int i = 0; i < 16; i++)
                        address.GetArrayElementAtIndex(i).intValue = ipv6[i];
                }
            }
        }

        #region Utility

        private static string ToHex(byte b, byte b2) {
            const string HEX = "0123456789ABCDEF";
            var lhs = (b >> 4) & 0xf;
            var rhs = (b) & 0xf;
            var lhs2 = (b2 >> 4) & 0xf;
            var rhs2 = (b2) & 0xf;
            return $"{HEX[lhs]}{HEX[rhs]}{HEX[lhs2]}{HEX[rhs2]}";
        }

        private static byte HexToByte(string str) {
            return byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        private static byte HexToByte(string str, int index) {
            return byte.Parse(str.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
        }

        private static bool IsHex(string test) {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        #endregion
    }
}
