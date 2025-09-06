using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Bounds : ITMLProperty {
        Bounds Bound { get; }
    }

    public interface ITMLProperty_Bounds_Array : ITMLProperty {
        IReadOnlyList<Bounds> Bounds { get; }
    }

    public sealed class TMLProperty_Bounds : TMLProperty_Base<Bounds>, ITMLProperty_Bounds, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Bounds]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Bounds;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Bounds Bound => value;
        public string String => $"[{Stringify(value)}]";

        #endregion

        #region Constructor

        public TMLProperty_Bounds() { }

        public TMLProperty_Bounds(string name) : base(name) { }

        public TMLProperty_Bounds(string name, Bounds value) : base(name, value) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        private static string Stringify(Bounds b) {
            var c = b.center;
            var e = b.size;
            return $"{c.x}|{c.y}|{c.z}|{e.x}|{e.y}|{e.z}";
        }

        #endregion
    }

    public sealed class TMLProperty_Bounds_Array : TMLProperty_Base_Array<Bounds>, ITMLProperty_Bounds, ITMLProperty_Bounds_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Bounds_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Bounds | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Bounds Bound => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Bounds> Bounds => value;

        public string String => $"[{string.Join('|', value.Select(Stringify))}]";

        #endregion

        #region Constructor

        public TMLProperty_Bounds_Array() : base() { }

        public TMLProperty_Bounds_Array(string name) : base(name) { }

        public TMLProperty_Bounds_Array(string name, Bounds value) : base(name, value) { }

        public TMLProperty_Bounds_Array(string name, IReadOnlyList<Bounds> values) : base(name, values) { }

        public TMLProperty_Bounds_Array(string name, IEnumerable<Bounds> values) : base(name, values) { }

        public TMLProperty_Bounds_Array(string name, List<Bounds> values) : base(name, values) { }

        public TMLProperty_Bounds_Array(string name, List<Bounds> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Bounds_Array(string name, IReadOnlyList<Bounds> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        private static string Stringify(Bounds b) {
            var c = b.center;
            var e = b.size;
            return $"{c.x}|{c.y}|{c.z}|{e.x}|{e.y}|{e.z}";
        }

        #endregion
    }
}
