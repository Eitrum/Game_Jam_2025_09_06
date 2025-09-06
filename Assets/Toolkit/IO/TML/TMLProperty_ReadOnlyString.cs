using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public sealed class TMLProperty_ReadOnlyString : IDisposable, ITMLProperty, ITMLProperty_String, ITMLProperty_String_Array,
        ITMLProperty_Boolean, ITMLProperty_Boolean_Array, ITMLProperty_Char, ITMLProperty_Char_Array, ITMLProperty_Byte, ITMLProperty_Byte_Array,
        ITMLProperty_SByte, ITMLProperty_SByte_Array, ITMLProperty_Short, ITMLProperty_Short_Array, ITMLProperty_UShort, ITMLProperty_UShort_Array,
        ITMLProperty_Int, ITMLProperty_Int_Array, ITMLProperty_UInt, ITMLProperty_UInt_Array, ITMLProperty_Long, ITMLProperty_Long_Array,
        ITMLProperty_ULong, ITMLProperty_ULong_Array, ITMLProperty_Float, ITMLProperty_Float_Array, ITMLProperty_Double, ITMLProperty_Double_Array,
        ITMLProperty_Decimal, ITMLProperty_Decimal_Array, ITMLProperty_TKDateTime, ITMLProperty_TKDateTime_Array, ITMLProperty_Vector2, ITMLProperty_Vector2_Array,
        ITMLProperty_Vector3, ITMLProperty_Vector3_Array, ITMLProperty_Vector4, ITMLProperty_Vector4_Array, ITMLProperty_Quaternion, ITMLProperty_Quaternion_Array,
        ITMLProperty_Pose, ITMLProperty_Pose_Array, ITMLProperty_Color, ITMLProperty_Color_Array, ITMLProperty_Rect, ITMLProperty_Rect_Array,
        ITMLProperty_Bounds, ITMLProperty_Bounds_Array, ITMLProperty_AnimationCurve, ITMLProperty_AnimationCurve_Array, ITMLProperty_Gradient, ITMLProperty_Gradient_Array {
        #region Variables

        private string name;
        private TMLDocument document;
        private int start;
        private int length;

        #endregion

        #region Properties

        public string Name => name;
        public byte Name_8 => TMLUtility.GetHash8(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public uint Name_32 => TMLUtility.GetHash32(name);

        public byte TypeId => TMLProperty_String.TYPE_ID;

        public System.ReadOnlySpan<char> ReadOnlySpan => document.GetSpan(start, length);

        public string String => ReadOnlySpan.ToString();
        public IReadOnlyList<string> Strings => ParseUtility.TryParse(ReadOnlySpan, out string[] result) ? result : default;

        public bool Boolean => ParseUtility.TryParse(ReadOnlySpan, out bool result) ? result : default;
        public IReadOnlyList<bool> Booleans => ParseUtility.TryParse(ReadOnlySpan, out bool[] result) ? result : default;

        public char Char => ParseUtility.TryParse(ReadOnlySpan, out char result) ? result : default;
        public IReadOnlyList<char> Chars => ParseUtility.TryParse(ReadOnlySpan, out char[] result) ? result : default;

        public byte Byte => ParseUtility.TryParse(ReadOnlySpan, out byte result) ? result : default;
        public IReadOnlyList<byte> Bytes => ParseUtility.TryParse(ReadOnlySpan, out byte[] result) ? result : default;
        public sbyte SByte => ParseUtility.TryParse(ReadOnlySpan, out sbyte result) ? result : default;
        public IReadOnlyList<sbyte> SBytes => ParseUtility.TryParse(ReadOnlySpan, out sbyte[] result) ? result : default;

        public short Short => ParseUtility.TryParse(ReadOnlySpan, out short result) ? result : default;
        public IReadOnlyList<short> Shorts => ParseUtility.TryParse(ReadOnlySpan, out short[] result) ? result : default;
        public ushort UShort => ParseUtility.TryParse(ReadOnlySpan, out ushort result) ? result : default;
        public IReadOnlyList<ushort> UShorts => ParseUtility.TryParse(ReadOnlySpan, out ushort[] result) ? result : default;

        public int Int => ParseUtility.TryParse(ReadOnlySpan, out int result) ? result : default;
        public IReadOnlyList<int> Ints => ParseUtility.TryParse(ReadOnlySpan, out int[] result) ? result : default;
        public uint UInt => ParseUtility.TryParse(ReadOnlySpan, out uint result) ? result : default;
        public IReadOnlyList<uint> UInts => ParseUtility.TryParse(ReadOnlySpan, out uint[] result) ? result : default;

        public long Long => ParseUtility.TryParse(ReadOnlySpan, out long result) ? result : default;
        public IReadOnlyList<long> Longs => ParseUtility.TryParse(ReadOnlySpan, out long[] result) ? result : default;
        public ulong ULong => ParseUtility.TryParse(ReadOnlySpan, out ulong result) ? result : default;
        public IReadOnlyList<ulong> ULongs => ParseUtility.TryParse(ReadOnlySpan, out ulong[] result) ? result : default;

        public float Float => ParseUtility.TryParse(ReadOnlySpan, out float result) ? result : default;
        public IReadOnlyList<float> Floats => ParseUtility.TryParse(ReadOnlySpan, out float[] result) ? result : default;
        public double Double => ParseUtility.TryParse(ReadOnlySpan, out double result) ? result : default;
        public IReadOnlyList<double> Doubles => ParseUtility.TryParse(ReadOnlySpan, out double[] result) ? result : default;
        public decimal Decimal => ParseUtility.TryParse(ReadOnlySpan, out decimal result) ? result : default;
        public IReadOnlyList<decimal> Decimals => ParseUtility.TryParse(ReadOnlySpan, out decimal[] result) ? result : default;

        public TKDateTime DateTime => ParseUtility.TryParse(ReadOnlySpan, out TKDateTime result) ? result : default;
        public IReadOnlyList<TKDateTime> DateTimes => ParseUtility.TryParse(ReadOnlySpan, out TKDateTime[] result) ? result : default;

        public Vector2 Vector2 => ParseUtility.TryParse(ReadOnlySpan, out Vector2 result) ? result : default;
        public IReadOnlyList<Vector2> Vector2s => ParseUtility.TryParse(ReadOnlySpan, out Vector2[] result) ? result : default;
        public Vector3 Vector3 => ParseUtility.TryParse(ReadOnlySpan, out Vector3 result) ? result : default;
        public IReadOnlyList<Vector3> Vector3s => ParseUtility.TryParse(ReadOnlySpan, out Vector3[] result) ? result : default;
        public Vector4 Vector4 => ParseUtility.TryParse(ReadOnlySpan, out Vector4 result) ? result : default;
        public IReadOnlyList<Vector4> Vector4s => ParseUtility.TryParse(ReadOnlySpan, out Vector4[] result) ? result : default;

        public Quaternion Quaternion => ParseUtility.TryParse(ReadOnlySpan, out Quaternion result) ? result : default;
        public IReadOnlyList<Quaternion> Quaternions => ParseUtility.TryParse(ReadOnlySpan, out Quaternion[] result) ? result : default;

        public Pose Pose => ParseUtility.TryParse(ReadOnlySpan, out Pose result) ? result : default;
        public IReadOnlyList<Pose> Poses => ParseUtility.TryParse(ReadOnlySpan, out Pose[] result) ? result : default;

        public Color Color => ParseUtility.TryParse(ReadOnlySpan, out Color result) ? result : default;
        public IReadOnlyList<Color> Colors => ParseUtility.TryParse(ReadOnlySpan, out Color[] result) ? result : default;

        public Rect Rect => ParseUtility.TryParse(ReadOnlySpan, out Rect result) ? result : default;
        public IReadOnlyList<Rect> Rects => ParseUtility.TryParse(ReadOnlySpan, out Rect[] result) ? result : default;

        public Bounds Bound => ParseUtility.TryParse(ReadOnlySpan, out Bounds result) ? result : default;
        public IReadOnlyList<Bounds> Bounds => ParseUtility.TryParse(ReadOnlySpan, out Bounds[] result) ? result : default;

        public AnimationCurve Curve => ParseUtility.TryParse(ReadOnlySpan, out AnimationCurve result) ? result : default;
        public IReadOnlyList<AnimationCurve> Curves => ParseUtility.TryParse(ReadOnlySpan, out AnimationCurve[] result) ? result : default;

        public Gradient Gradient => ParseUtility.TryParse(ReadOnlySpan, out Gradient result) ? result : default;
        public IReadOnlyList<Gradient> Gradients => ParseUtility.TryParse(ReadOnlySpan, out Gradient[] result) ? result : default;

        #endregion

        #region Constructor

        public TMLProperty_ReadOnlyString() { }

        public TMLProperty_ReadOnlyString(string name, TMLDocument doc, int start, int length) {
            this.name = name;
            this.document = doc;
            this.start = start;
            this.length = length;
        }

        public static TMLProperty_ReadOnlyString CreateFromPool(string name, TMLDocument doc, int start, int length) {
            var prop = FastPool<TMLProperty_ReadOnlyString>.Global.Pop();
            prop.name = name;
            prop.document = doc;
            prop.start = start;
            prop.length = length;
            return prop;
        }

        public void Dispose() {
            if(this.document != null) {
                this.name = null;
                this.document = null;
                FastPool<TMLProperty_ReadOnlyString>.Global.Push(this);
            }
        }

        #endregion

        #region Checks

        public bool IsName(string name) => this.name == name;
        public bool IsName(byte name) => this.Name_8 == name;
        public bool IsName(ushort name) => this.Name_16 == name;
        public bool IsName(uint name) => this.Name_32 == name;

        #endregion
    }
}
