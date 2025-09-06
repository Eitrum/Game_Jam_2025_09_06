using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Rect: ITMLProperty {
        Rect Rect { get; }
    }

    public interface ITMLProperty_Rect_Array : ITMLProperty {
        IReadOnlyList<Rect> Rects { get; }
    }

    public sealed class TMLProperty_Rect : TMLProperty_Base<Rect>, ITMLProperty_Rect, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Rect]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Rect;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Rect Rect => value;
        public string String => $"[{Stringify(value)}]";

        #endregion

        #region Constructor

        public TMLProperty_Rect() { }

        public TMLProperty_Rect(string name) : base(name) { }

        public TMLProperty_Rect(string name, Rect value) : base(name, value) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        private static string Stringify(Rect b) {
            var c = b.center;
            var e = b.size;
            return $"{c.x}|{c.y}|{e.x}|{e.y}";
        }

        #endregion
    }

    public sealed class TMLProperty_Rect_Array : TMLProperty_Base_Array<Rect>, ITMLProperty_Rect, ITMLProperty_Rect_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Rect_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Bounds | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Rect Rect => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Rect> Rects => value;

        public string String => $"[{string.Join('|', value.Select(Stringify))}]";

        #endregion

        #region Constructor

        public TMLProperty_Rect_Array() : base() { }

        public TMLProperty_Rect_Array(string name) : base(name) { }

        public TMLProperty_Rect_Array(string name, Rect value) : base(name, value) { }

        public TMLProperty_Rect_Array(string name, IReadOnlyList<Rect> values) : base(name, values) { }

        public TMLProperty_Rect_Array(string name, IEnumerable<Rect> values) : base(name, values) { }

        public TMLProperty_Rect_Array(string name, List<Rect> values) : base(name, values) { }

        public TMLProperty_Rect_Array(string name, List<Rect> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Rect_Array(string name, IReadOnlyList<Rect> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";
        
        private static string Stringify(Rect b) {
            var c = b.center;
            var e = b.size;
            return $"{c.x}|{c.y}|{e.x}|{e.y}";
        }

        #endregion
    }
}
