using System;
using System.Collections.Generic;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Char : ITMLProperty {
        char Char { get; }
    }

    public interface ITMLProperty_Char_Array : ITMLProperty {
        IReadOnlyList<char> Chars { get; }
    }

    public class TMLProperty_Char : TMLProperty_Base<char>, ITMLProperty_Char {
        #region Variables

        public const byte TYPE_ID = TMLBuiltInTypes.Char;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public char Char => value;

        #endregion

        #region Constructor

        public TMLProperty_Char() : base() { }

        public TMLProperty_Char(string name) : base(name) { }

        public TMLProperty_Char(string name, char value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Char_Array : TMLProperty_Base_Array<char>, ITMLProperty_Char, ITMLProperty_Char_Array {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Byte_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Char | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public char Char => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<char> Chars => value;

        #endregion

        #region Constructor

        public TMLProperty_Char_Array() : base() { }

        public TMLProperty_Char_Array(string name) : base(name) { }

        public TMLProperty_Char_Array(string name, char value) : base(name, value) { }

        public TMLProperty_Char_Array(string name, IReadOnlyList<char> values) : base(name, values) { }

        public TMLProperty_Char_Array(string name, IEnumerable<char> values) : base(name, values) { }

        public TMLProperty_Char_Array(string name, List<char> values) : base(name, values) { }

        public TMLProperty_Char_Array(string name, List<char> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Char_Array(string name, IReadOnlyList<char> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
