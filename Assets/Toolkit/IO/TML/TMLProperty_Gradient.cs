using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Gradient : ITMLProperty {
        Gradient Gradient { get; }
    }

    public interface ITMLProperty_Gradient_Array : ITMLProperty {
        IReadOnlyList<Gradient> Gradients { get; }
    }

    public sealed class TMLProperty_Gradient : TMLProperty_Base<Gradient>, ITMLProperty_Gradient, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Gradient]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Gradient;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Gradient Gradient => value;
        public string String => Stringify(value);

        #endregion

        #region Constructor

        public TMLProperty_Gradient() { }

        public TMLProperty_Gradient(string name) : base(name) { }

        public TMLProperty_Gradient(string name, Gradient value) : base(name, value) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        #endregion

        #region Utility

        public static string Stringify(Gradient value) {
            return $"[{(int)value.mode}|{(int)value.colorSpace}|[{string.Join('|', value.colorKeys.Select(Stringify))}]|[{string.Join('|', value.alphaKeys.Select(Stringify))}]";
        }

        private static string Stringify(GradientColorKey cKey) {
            var c32 = (Color32)cKey.color;
            return $"{cKey.time},{c32.r},{c32.g},{c32.b}";
        }

        private static string Stringify(GradientAlphaKey aKey) => $"{aKey.time},{aKey.alpha}";

        #endregion
    }

    public sealed class TMLProperty_Gradient_Array : TMLProperty_Base_Array<Gradient>, ITMLProperty_Gradient, ITMLProperty_Gradient_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Gradient_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Gradient | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Gradient Gradient => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Gradient> Gradients => value;

        public string String => $"[{string.Join(',', value.Select(TMLProperty_Gradient.Stringify))}]";

        #endregion

        #region Constructor

        public TMLProperty_Gradient_Array() : base() { }

        public TMLProperty_Gradient_Array(string name) : base(name) { }

        public TMLProperty_Gradient_Array(string name, Gradient value) : base(name, value) { }

        public TMLProperty_Gradient_Array(string name, IReadOnlyList<Gradient> values) : base(name, values) { }

        public TMLProperty_Gradient_Array(string name, IEnumerable<Gradient> values) : base(name, values) { }

        public TMLProperty_Gradient_Array(string name, List<Gradient> values) : base(name, values) { }

        public TMLProperty_Gradient_Array(string name, List<Gradient> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Gradient_Array(string name, IReadOnlyList<Gradient> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
