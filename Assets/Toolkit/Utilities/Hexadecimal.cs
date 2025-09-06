using System;
using UnityEngine;

namespace Toolkit {
    public static class Hexadecimal {
        #region Variables

        private static uint[] unsafeLookup;

        #endregion

        #region Constructor

        static Hexadecimal() {
            unsafeLookup = new uint[256];
            for(int i = 0; i < 256; i++) {
                string s=i.ToString("X2");
                if(BitConverter.IsLittleEndian)
                    unsafeLookup[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                else
                    unsafeLookup[i] = ((uint)s[1]) + ((uint)s[0] << 16);
            }
        }

        #endregion

        #region Utility

        private static byte GetByte(char c0, char c1) {
            return (byte)((GetByte(c0) << 4) | GetByte(c1));
        }

        private static byte GetByte(char c) {
            switch(c) {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A': return 10;
                case 'b':
                case 'B': return 11;
                case 'c':
                case 'C': return 12;
                case 'd':
                case 'D': return 13;
                case 'e':
                case 'E': return 14;
                case 'f':
                case 'F': return 15;
            }
            return 0;
        }

        #endregion

        #region ToString

        public static unsafe string ToString(byte[] bytes) {
            var result = new string((char)0, bytes.Length * 2);
            fixed(byte* bPtr = bytes) {
                fixed(char* rPtr = result) {
                    uint* ruPtr = (uint*)rPtr;
                    for(int i = 0; i < bytes.Length; i++)
                        ruPtr[i] = unsafeLookup[bPtr[i]];
                }
            }
            return result;
        }

        public static unsafe string ToString(long ticks) => $"{ticks:X16}";

        #endregion

        #region ToArray

        public static byte[] ToArray(string hex)
            => ToArray(hex, 0, hex.Length);

        public static byte[] ToArray(string hex, int index, int length) {
            byte[] bytes = new byte[length / 2];
            var end = index + length;
            var bindex = 0;
            for(int i = index; i < end; i += 2)
                bytes[bindex++] = GetByte(hex[i], hex[i + 1]);
            return bytes;
        }

        #endregion

        #region ToLong

        public static long ToLong(string hex)
            => ToLong(hex, 0, hex.Length);

        public static long ToLong(string hex, int index, int length) {
            try {
                if(length % 2 != 0)
                    length--;
                long value = 0;
                var end = index + length;
                for(int i = index; i < end; i += 2) {
                    value = (value << 8) | GetByte(hex[i], hex[i + 1]);
                }
                return value;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return default;
            }
        }

        #endregion
    }
}
