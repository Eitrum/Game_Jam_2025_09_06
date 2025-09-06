using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Vector4 : ITMLProperty {
        Vector4 Vector4 { get; }
    }

    public interface ITMLProperty_Vector4_Array : ITMLProperty {
        IReadOnlyList<Vector4> Vector4s { get; }
    }

    public sealed class TMLProperty_Vector4 : TMLProperty_Base<Vector4>, ITMLProperty_Vector3, ITMLProperty_Vector2, ITMLProperty_Vector4, ITMLProperty_Quaternion {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector4]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector4;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value;
        public Vector3 Vector3 => value;
        public Vector4 Vector4 => value;
        public Quaternion Quaternion => new Quaternion(value.x, value.y, value.z, value.w);

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Vector4() { }

        public TMLProperty_Vector4(string name) : base(name) { }

        public TMLProperty_Vector4(string name, Vector4 value) : base(name, value) { }
        
        public TMLProperty_Vector4(string name, Vector3 value) : base(name, value) { }

        public TMLProperty_Vector4(string name, Quaternion value) : base(name, new Vector4(value.x, value.y, value.z, value.w)) { }

        public TMLProperty_Vector4(string name, Vector2 value) : base(name, value) { }

        #endregion
    }
    
    public sealed class TMLProperty_Vector4_Array : TMLProperty_Base_Array<Vector4>, ITMLProperty_Vector4, ITMLProperty_Vector4_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Vector4_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Vector4 | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector4 Vector4 => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Vector4> Vector4s => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Vector4_Array() : base() { }

        public TMLProperty_Vector4_Array(string name) : base(name) { }

        public TMLProperty_Vector4_Array(string name, Vector4 value) : base(name, value) { }

        public TMLProperty_Vector4_Array(string name, IReadOnlyList<Vector4> values) : base(name, values) { }

        public TMLProperty_Vector4_Array(string name, IEnumerable<Vector4> values) : base(name, values) { }

        public TMLProperty_Vector4_Array(string name, List<Vector4> values) : base(name, values) { }

        public TMLProperty_Vector4_Array(string name, List<Vector4> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Vector4_Array(string name, IReadOnlyList<Vector4> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
