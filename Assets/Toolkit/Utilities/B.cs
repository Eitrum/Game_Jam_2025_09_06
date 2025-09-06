namespace Toolkit {
    public static class B {
        #region Byte Mask

        public static class ByteMask {
            public const byte OOOO_OOOI = 1 << 0;
            public const byte OOOO_OOIO = 1 << 1;
            public const byte OOOO_OIOO = 1 << 2;
            public const byte OOOO_IOOO = 1 << 3;

            public const byte OOOI_OOOO = 1 << 4;
            public const byte OOIO_OOOO = 1 << 5;
            public const byte OIOO_OOOO = 1 << 6;
            public const byte IOOO_OOOO = 1 << 7;
        }

        #endregion

        #region Int Mask

        public static class IntMask {
            public const int OO_OO_OO_FF = 0xff;
            public const int OO_OO_FF_OO = 0xff << 8;
            public const int OO_FF_OO_OO = 0xff << 16;
            public const int FF_OO_OO_OO = 0xff << 24;

        }

        #endregion

        #region Generative Binary Value

        public static readonly Val OOOO = 0x0;
        public static readonly Val OOOI = 0x1;
        public static readonly Val OOIO = 0x2;
        public static readonly Val OOII = 0x3;
        public static readonly Val OIOO = 0x4;
        public static readonly Val OIOI = 0x5;
        public static readonly Val OIIO = 0x6;
        public static readonly Val OIII = 0x7;
        public static readonly Val IOOO = 0x8;
        public static readonly Val IOOI = 0x9;
        public static readonly Val IOIO = 0xA;
        public static readonly Val IOII = 0xB;
        public static readonly Val IIOO = 0xC;
        public static readonly Val IIOI = 0xD;
        public static readonly Val IIIO = 0xE;
        public static readonly Val IIII = 0xF;

        public struct Val {
            ulong Value;
            public Val(ulong value) => this.Value = value;
            private Val Shift(ulong value) => new Val((this.Value << 4) + value);

            public Val OOOO { get { return this.Shift(0x0); } }
            public Val OOOI { get { return this.Shift(0x1); } }
            public Val OOIO { get { return this.Shift(0x2); } }
            public Val OOII { get { return this.Shift(0x3); } }

            public Val OIOO { get { return this.Shift(0x4); } }
            public Val OIOI { get { return this.Shift(0x5); } }
            public Val OIIO { get { return this.Shift(0x6); } }
            public Val OIII { get { return this.Shift(0x7); } }

            public Val IOOO { get { return this.Shift(0x8); } }
            public Val IOOI { get { return this.Shift(0x9); } }
            public Val IOIO { get { return this.Shift(0xA); } }
            public Val IOII { get { return this.Shift(0xB); } }

            public Val IIOO { get { return this.Shift(0xC); } }
            public Val IIOI { get { return this.Shift(0xD); } }
            public Val IIIO { get { return this.Shift(0xE); } }
            public Val IIII { get { return this.Shift(0xF); } }

            public static implicit operator Val(ulong value) => new Val(value);
            public static implicit operator ulong(Val this_) => this_.Value;
            public static implicit operator uint(Val this_) => (uint)this_.Value;
            public static implicit operator ushort(Val this_) => (ushort)this_.Value;
            public static implicit operator byte(Val this_) => (byte)this_.Value;
        }

        #endregion

        #region Extensions

        public static bool HasFlag(this byte val, byte flag) => (val & flag) == flag;
        public static bool HasFlag(this sbyte val, byte flag) => (val & flag) == flag;

        public static bool HasFlag(this short val, short flag) => (val & flag) == flag;
        public static bool HasFlag(this ushort val, ushort flag) => (val & flag) == flag;

        public static bool HasFlag(this int mask, int value) => (mask & value) == value;
        public static void SetFlag(this ref int mask, int value, bool active) => mask = (active ? (mask | value) : (mask & (~value)));
        public static bool HasFlag(this uint mask, uint value) => (mask & value) == value;
        public static void SetFlag(this ref uint mask, uint value, bool active) => mask = (active ? (mask | value) : (mask & (~value)));

        public static bool HasFlag(this long val, long flag) => (val & flag) == flag;
        public static bool HasFlag(this ulong val, ulong flag) => (val & flag) == flag;

        #endregion

        #region Visualization

        public static string ToString(byte value) {
            char[] chars = new char[8];
            byte b = 1;
            for(int i = 0; i < 8; i++)
                chars[i] = value.HasFlag((byte)(b << i)) ? 'I' : 'O';
            return new string(chars);
        }

        public static string ToString(int value) {
            char[] chars = new char[32];
            for(int i = 0; i < 32; i++)
                chars[i] = value.HasFlag(1 << i) ? 'I' : 'O';
            return new string(chars);
        }

        public static string ToString(uint value) {
            char[] chars = new char[32];
            uint t = 1;
            for(int i = 0; i < 32; i++)
                chars[i] = value.HasFlag(t << i) ? 'I' : 'O';
            return new string(chars);
        }

        #endregion
    }
}
