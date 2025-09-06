using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Quaternion : ITMLProperty {
        Quaternion Quaternion { get; }
    }

    public interface ITMLProperty_Quaternion_Array : ITMLProperty {
        IReadOnlyList<Quaternion> Quaternions { get; }
    }

    public sealed class TMLProperty_Quaternion : TMLProperty_Base<Quaternion>, ITMLProperty_Vector3, ITMLProperty_Vector2, ITMLProperty_Vector4, ITMLProperty_Quaternion {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Quaternion]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Quaternion;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value.eulerAngles.To_XZ();
        public Vector3 Vector3 => value.eulerAngles;
        public Vector4 Vector4 => value.ToVector4();
        public Quaternion Quaternion => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Quaternion() { }

        public TMLProperty_Quaternion(string name) : base(name) { }

        public TMLProperty_Quaternion(string name, Quaternion value) : base(name, value) { }
        
        public TMLProperty_Quaternion(string name, Vector3 value) : base(name, Quaternion.Euler( value)) { }

        public TMLProperty_Quaternion(string name, Vector4 value) : base(name, new Quaternion(value.x, value.y, value.z, value.w)) { }

        public TMLProperty_Quaternion(string name, Vector2 value) : base(name, Quaternion.Euler(value.x, value.y, 0)) { }

        #endregion
    }
    
    public sealed class TMLProperty_Quaternion_Array : TMLProperty_Base_Array<Quaternion>, ITMLProperty_Quaternion, ITMLProperty_Quaternion_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Quaternion_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Quaternion | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Quaternion Quaternion => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Quaternion> Quaternions => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Quaternion_Array() : base() { }

        public TMLProperty_Quaternion_Array(string name) : base(name) { }

        public TMLProperty_Quaternion_Array(string name, Quaternion value) : base(name, value) { }

        public TMLProperty_Quaternion_Array(string name, IReadOnlyList<Quaternion> values) : base(name, values) { }

        public TMLProperty_Quaternion_Array(string name, IEnumerable<Quaternion> values) : base(name, values) { }

        public TMLProperty_Quaternion_Array(string name, List<Quaternion> values) : base(name, values) { }

        public TMLProperty_Quaternion_Array(string name, List<Quaternion> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Quaternion_Array(string name, IReadOnlyList<Quaternion> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
