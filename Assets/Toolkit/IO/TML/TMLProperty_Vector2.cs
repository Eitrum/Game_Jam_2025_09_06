using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Vector2 : ITMLProperty {
        Vector2 Vector2 { get; }
    }

    public interface ITMLProperty_Vector2_Array : ITMLProperty {
        IReadOnlyList<Vector2> Vector2s { get; }
    }

    public sealed class TMLProperty_Vector2 : TMLProperty_Base<Vector2>, ITMLProperty_Vector2, ITMLProperty_Vector3, ITMLProperty_Quaternion {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector2]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector2;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value;
        public Vector3 Vector3 => value;
        public Vector4 Vector4 => value;
        public Quaternion Quaternion => UnityEngine.Quaternion.Euler(value.x, value.y, 0f);

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Vector2() { }

        public TMLProperty_Vector2(string name) : base(name) { }

        public TMLProperty_Vector2(string name, Vector4 value) : base(name, value) { }

        public TMLProperty_Vector2(string name, Vector3 value) : base(name, value) { }

        public TMLProperty_Vector2(string name, Vector2 value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Vector2_Array : TMLProperty_Base_Array<Vector2>, ITMLProperty_Vector2, ITMLProperty_Vector2_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector2_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector2 | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Vector2> Vector2s => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Vector2_Array() : base() { }

        public TMLProperty_Vector2_Array(string name) : base(name) { }

        public TMLProperty_Vector2_Array(string name, Vector2 value) : base(name, value) { }

        public TMLProperty_Vector2_Array(string name, IReadOnlyList<Vector2> values) : base(name, values) { }

        public TMLProperty_Vector2_Array(string name, IEnumerable<Vector2> values) : base(name, values) { }

        public TMLProperty_Vector2_Array(string name, List<Vector2> values) : base(name, values) { }

        public TMLProperty_Vector2_Array(string name, List<Vector2> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Vector2_Array(string name, IReadOnlyList<Vector2> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
