using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.IO.TML.Properties {
    public interface ITMLProperty_Pose : ITMLProperty {
        Pose Pose { get; }
    }
    
    public interface ITMLProperty_Pose_Array : ITMLProperty {
        IReadOnlyList<Pose> Poses { get; }
    }

    public sealed class TMLProperty_Pose : TMLProperty_Base<Pose>, ITMLProperty_Vector3, ITMLProperty_Vector2, ITMLProperty_Vector4, ITMLProperty_Quaternion, ITMLProperty_Pose {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Pose]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Pose;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Vector2 Vector2 => value.position;
        public Vector3 Vector3 => value.position;
        public Vector4 Vector4 => value.position;
        public Quaternion Quaternion => value.rotation;
        public Pose Pose => value;

        public string String => $"{value}";

        #endregion

        #region Constructor

        public TMLProperty_Pose() { }

        public TMLProperty_Pose(string name) : base(name) { }

        public TMLProperty_Pose(string name, Vector3 value) : base(name, new Pose(value, Quaternion.identity)) { }

        public TMLProperty_Pose(string name, Quaternion value) : base(name, new Pose(UnityEngine.Vector3.zero, value)) { }

        public TMLProperty_Pose(string name, Pose value) : base(name, value) { }

        #endregion
    }

    public sealed class TMLProperty_Pose_Array : TMLProperty_Base_Array<Pose>, ITMLProperty_Pose, ITMLProperty_Pose_Array, ITMLProperty_String {
        #region Variables

        private const string TAG = TMLUtility.COLOR_TAG + "[TMLProperty_Pose_Array]</color> - ";
        public const byte TYPE_ID = TMLBuiltInTypes.Pose | TMLBuiltInTypes.Array_Mask;

        #endregion

        #region Properties

        public override byte TypeId => TYPE_ID;

        public Pose Pose => value.Count > 0 ? value[0] : default;
        public IReadOnlyList<Pose> Poses => value;

        public string String => DefaultToString(this);

        #endregion

        #region Constructor

        public TMLProperty_Pose_Array() : base() { }

        public TMLProperty_Pose_Array(string name) : base(name) { }

        public TMLProperty_Pose_Array(string name, Pose value) : base(name, value) { }

        public TMLProperty_Pose_Array(string name, IReadOnlyList<Pose> values) : base(name, values) { }

        public TMLProperty_Pose_Array(string name, IEnumerable<Pose> values) : base(name, values) { }

        public TMLProperty_Pose_Array(string name, List<Pose> values) : base(name, values) { }

        public TMLProperty_Pose_Array(string name, List<Pose> values, bool createCopy) : base(name, values, createCopy) { }

        public TMLProperty_Pose_Array(string name, IReadOnlyList<Pose> values, bool createCopy) : base(name, values, createCopy) { }

        #endregion
    }
}
