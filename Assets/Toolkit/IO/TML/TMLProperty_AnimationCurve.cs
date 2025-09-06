using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_AnimationCurve : ITMLProperty {
        AnimationCurve Curve { get; }
    }
    
    public interface ITMLProperty_AnimationCurve_Array : ITMLProperty {
        IReadOnlyList<AnimationCurve> Curves { get; }
    }

    public sealed class TMLProperty_AnimationCurve : TMLProperty_Base<AnimationCurve>, ITMLProperty_AnimationCurve, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_AnimationCurve]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.AnimationCurve;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public AnimationCurve Curve => value;
        public string String => Stringify(value);

        #endregion

        #region Constructor

        public TMLProperty_AnimationCurve() { }

        public TMLProperty_AnimationCurve(string name) : base(name) { }

        public TMLProperty_AnimationCurve(string name, AnimationCurve value) : base(name, value) { }

        #endregion

        #region Overrides

        public override string ToString() => $"{name}:{String}";

        #endregion

        #region Utility

        public static string Stringify(AnimationCurve value) {
            throw new NotImplementedException(TAG + "Not implemented string writing yet...");
            //return $"[{(int)value.mode}|{(int)value.colorSpace}|[{string.Join('|', value.colorKeys.Select(Stringify))}]|[{string.Join('|', value.alphaKeys.Select(Stringify))}]";
        }

        #endregion
    }

    public sealed class TMLProperty_AnimationCurve_Array : TMLProperty_Base_Array<AnimationCurve>, ITMLProperty_AnimationCurve, ITMLProperty_AnimationCurve_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_AnimationCurve_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.AnimationCurve | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public AnimationCurve Curve => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<AnimationCurve> Curves => value;

        public string String => $"[{string.Join(',', value.Select(TMLProperty_AnimationCurve.Stringify))}]";

        #endregion

        #region Constructor

        public TMLProperty_AnimationCurve_Array() : base() { }

        public TMLProperty_AnimationCurve_Array(string name) : base(name) { }

        public TMLProperty_AnimationCurve_Array(string name, AnimationCurve value) : base(name, value) { }

        public TMLProperty_AnimationCurve_Array(string name, IReadOnlyList<AnimationCurve> values) : base(name, values) { }

        public TMLProperty_AnimationCurve_Array(string name, IEnumerable<AnimationCurve> values) : base(name, values) { }

        public TMLProperty_AnimationCurve_Array(string name, List<AnimationCurve> values) : base(name, values) { }

        public TMLProperty_AnimationCurve_Array(string name, List<AnimationCurve> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_AnimationCurve_Array(string name, IReadOnlyList<AnimationCurve> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
