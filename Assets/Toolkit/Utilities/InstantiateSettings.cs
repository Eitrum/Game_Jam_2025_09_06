using System;
using UnityEngine;

namespace Toolkit {
    [Serializable]
    public struct InstantiateSettings {

        #region Variables

        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public Transform parent;

        #endregion

        #region Properties

        public Vector3 WorldPosition => parent == null ? localPosition : parent.position + parent.rotation * localPosition;
        public Quaternion WorldRotation => parent == null ? localRotation : parent.rotation * localRotation;
        public Vector3 WorldScale => parent == null ? localScale : parent.lossyScale.Multiply(localScale);

        #endregion

        #region Constructor

        public InstantiateSettings(Vector3 localPosition)
            : this(localPosition, Quaternion.identity, Vector3.one, null) { }

        public InstantiateSettings(Vector3 localPosition, Transform parent)
            : this(localPosition, Quaternion.identity, Vector3.one, parent) { }

        public InstantiateSettings(Vector3 localPosition, Quaternion localRotation)
            : this(localPosition, localRotation, Vector3.one, null) { }

        public InstantiateSettings(Vector3 localPosition, Quaternion localRotation, Transform parent)
            : this(localPosition, localRotation, Vector3.one, parent) { }

        public InstantiateSettings(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            : this(localPosition, localRotation, localScale, null) { }

        public InstantiateSettings(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent) {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.localScale = localScale;
            this.parent = parent;
        }

        #endregion

        #region Conversion

        public Pose GetPose() => new Pose(localPosition, localRotation);
        public Pose GetPose(Space space) => space == Space.Self ? GetPose() : new Pose(WorldPosition, WorldRotation);

        #endregion
    }
}
