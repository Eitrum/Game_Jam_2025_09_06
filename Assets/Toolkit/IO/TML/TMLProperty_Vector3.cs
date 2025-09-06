using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Vector3 : ITMLProperty {
        Vector3 Vector3 { get; }
    }

    public interface ITMLProperty_Vector3_Array : ITMLProperty {
        IReadOnlyList<Vector3> Vector3s { get; }
    }

    public sealed class TMLProperty_Vector3 : TMLProperty_Base<Vector3>, ITMLProperty_Vector3, ITMLProperty_Vector2, ITMLProperty_Vector4, ITMLProperty_Quaternion {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector3]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector3;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value;
        public Vector3 Vector3 => value;
        public Vector4 Vector4 => value;
        public Quaternion Quaternion => UnityEngine.Quaternion.Euler(value.x, value.y, value.z);

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Vector3() { }

        public TMLProperty_Vector3(string name) : base(name) { }

        public TMLProperty_Vector3(string name, Vector4 value) : base(name, value) { }
        
        public TMLProperty_Vector3(string name, Vector3 value) : base(name, value) { }

        public TMLProperty_Vector3(string name, Quaternion value) : base(name, value.eulerAngles) { }

        public TMLProperty_Vector3(string name, Vector2 value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Vector3_Array : TMLProperty_Base_Array<Vector3>, ITMLProperty_Vector3, ITMLProperty_Vector3_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector3_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector3 | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector3 Vector3 => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Vector3> Vector3s => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Vector3_Array() : base() { }

        public TMLProperty_Vector3_Array(string name) : base(name) { }

        public TMLProperty_Vector3_Array(string name, Vector3 value) : base(name, value) { }

        public TMLProperty_Vector3_Array(string name, IReadOnlyList<Vector3> values) : base(name, values) { }

        public TMLProperty_Vector3_Array(string name, IEnumerable<Vector3> values) : base(name, values) { }

        public TMLProperty_Vector3_Array(string name, List<Vector3> values) : base(name, values) { }

        public TMLProperty_Vector3_Array(string name, List<Vector3> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Vector3_Array(string name, IReadOnlyList<Vector3> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
