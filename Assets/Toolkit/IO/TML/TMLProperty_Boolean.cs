using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Boolean : ITMLProperty {
        bool Boolean { get; }
    }

    public interface ITMLProperty_Boolean_Array : ITMLProperty {
        IReadOnlyList<bool> Booleans { get; }
    }

    public interface ITMLProperty_Boolean_True : ITMLProperty_Boolean { }
    public interface ITMLProperty_Boolean_False : ITMLProperty_Boolean { }

    public sealed class TMLProperty_Boolean : TMLProperty_Base<bool>, ITMLProperty_Int, ITMLProperty_Byte {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Boolean]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Boolean;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value;

        public sbyte SByte => (sbyte)(value ? 1 : 0);
        public byte Byte => (byte)(value ? 1 : 0);

        public short Short => (short)(value ? 1 : 0);
        public ushort UShort => (ushort)(value ? 1 : 0);

        public int Int => (int)(value ? 1 : 0);
        public uint UInt => (uint)(value ? 1 : 0);

        public long Long => value ? 1 : 0;
        public ulong ULong => (ulong)(value ? 1 : 0);

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Boolean() { }

        public TMLProperty_Boolean(string name) : base(name) { }

        public TMLProperty_Boolean(string name, int value) : base(name, value > 0) { }

        public TMLProperty_Boolean(string name, bool value) : base(name, value) { }

        #endregion

        #region Creation

        public static ITMLProperty_Boolean Create(string name, bool value) {
            return value ? new TMLProperty_Boolean_True(name) : new TMLProperty_Boolean_False(name);
        }

        #endregion
    }

    public sealed class TMLProperty_Boolean_Array : TMLProperty_Base_Array<bool>, ITMLProperty_Boolean, ITMLProperty_Boolean_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Boolean_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Boolean | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public bool Boolean => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<bool> Booleans => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Boolean_Array() : base() { }

        public TMLProperty_Boolean_Array(string name) : base(name) { }

        public TMLProperty_Boolean_Array(string name, bool value) : base(name, value) { }

        public TMLProperty_Boolean_Array(string name, IReadOnlyList<bool> values) : base(name, values) { }

        public TMLProperty_Boolean_Array(string name, IEnumerable<bool> values) : base(name, values) { }

        public TMLProperty_Boolean_Array(string name, List<bool> values) : base(name, values) { }

        public TMLProperty_Boolean_Array(string name, List<bool> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Boolean_Array(string name, IReadOnlyList<bool> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }

    internal class TMLProperty_Boolean_True : ITMLProperty, ITMLProperty_Boolean, ITMLProperty_Boolean_True {
        #region Variables

        public const byte TYPE_ID = TMLBuiltInTypes.Boolean_True;
        private string name;

        #endregion

        #region Properties

        public string Name => name;
        public byte Name_8 => TMLUtility.GetHash8(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public uint Name_32 => TMLUtility.GetHash32(name);

        public byte TypeId => TYPE_ID;
        public bool Boolean => true;

        public string String => "true";
        public int Int => 1;
        public byte Byte => 1;

        #endregion

        #region Constructor

        public TMLProperty_Boolean_True() { }

        public TMLProperty_Boolean_True(string name) {
            this.name = name;
        }

        #endregion

        #region Checks

        public bool IsName(string name) => this.name.Equals(name);

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:TRUE";

        #endregion
    }

    internal class TMLProperty_Boolean_False : ITMLProperty, ITMLProperty_Boolean, ITMLProperty_Boolean_False {
        #region Variables

        public const byte TYPE_ID = TMLBuiltInTypes.Boolean_False;
        private string name;

        #endregion

        #region Properties

        public string Name => name;
        public byte Name_8 => TMLUtility.GetHash8(name);
        public ushort Name_16 => TMLUtility.GetHash16(name);
        public uint Name_32 => TMLUtility.GetHash32(name);

        public byte TypeId => TYPE_ID;
        public bool Boolean => false;

        public string String => "false";
        public int Int => 1;
        public byte Byte => 1;

        #endregion

        #region Constructor

        public TMLProperty_Boolean_False() { }

        public TMLProperty_Boolean_False(string name) {
            this.name = name;
        }

        #endregion

        #region Checks

        public bool IsName(string name) => this.name.Equals(name);

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:false";

        #endregion
    }
}
