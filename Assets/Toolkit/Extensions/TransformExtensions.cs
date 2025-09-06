using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public static class TransformExtensions {
        #region Conversion

        public static RectTransform ToRectTransform(this Transform transform) => transform as RectTransform;

        public static Ray ToRay(this Transform transform) => new Ray(transform.position, transform.forward);

        public static Ray ToRay(this Transform transform, bool ground) => ground ? new Ray(transform.position, Vector3.down) : transform.ToRay();

        #endregion

        #region Reset

        public static void Reset(this Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Copy

        public static void Copy(this Transform transform, Transform other) {
            transform.localPosition = other.localPosition;
            transform.localRotation = other.localRotation;
            transform.localScale = other.localScale;
        }

        public static void Copy(this Transform transform, Transform other, Space space) {
            if(space == Space.Self) {
                transform.Copy(other);
            }
            else {
                transform.SetPositionAndRotation(other.position, other.rotation);
                transform.SetLossyScale(other.lossyScale);
            }
        }

        public static void Copy(this Transform transform, Transform other, TransformMask mask)
            => Copy(transform, other, Space.Self, mask);

        public static void Copy(this Transform transform, Transform other, Space space, TransformMask mask) {
            Vector3 tPosition = space == Space.Self ? transform.localPosition : transform.position;
            Vector3 oPosition = space == Space.Self ? other.localPosition : other.position;
            Vector3 tRotation = space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
            Vector3 oRotation = space == Space.Self ? other.localEulerAngles : other.eulerAngles;
            Vector3 tScale = space == Space.Self ? transform.localScale : transform.lossyScale;
            Vector3 oScale = space == Space.Self ? other.localScale : other.lossyScale;

            if(mask.HasFlag(TransformMask.PositionX))
                tPosition.x = oPosition.x;
            if(mask.HasFlag(TransformMask.PositionY))
                tPosition.y = oPosition.y;
            if(mask.HasFlag(TransformMask.PositionZ))
                tPosition.z = oPosition.z;

            if(mask.HasFlag(TransformMask.RotationX))
                tRotation.x = oRotation.x;
            if(mask.HasFlag(TransformMask.RotationY))
                tRotation.y = oRotation.y;
            if(mask.HasFlag(TransformMask.RotationZ))
                tRotation.z = oRotation.z;

            if(mask.HasFlag(TransformMask.ScaleX))
                tScale.x = oScale.x;
            if(mask.HasFlag(TransformMask.ScaleY))
                tScale.y = oScale.y;
            if(mask.HasFlag(TransformMask.ScaleZ))
                tScale.z = oScale.z;


            if(space == Space.Self) {
                transform.localPosition = tPosition;
                transform.localEulerAngles = tRotation;
                transform.localScale = tScale;
            }
            else {
                transform.position = tPosition;
                transform.eulerAngles = tRotation;
                transform.SetLossyScale(tScale);
            }
        }

        public static void Copy(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale, Space space, TransformMask mask) {
            Vector3 tPosition = space == Space.Self ? transform.localPosition : transform.position;
            Vector3 oPosition = position;
            Vector3 tRotation = space == Space.Self ? transform.localEulerAngles : transform.eulerAngles;
            Vector3 oRotation = rotation.eulerAngles;
            Vector3 tScale = space == Space.Self ? transform.localScale : transform.lossyScale;
            Vector3 oScale = scale;

            if(mask.HasFlag(TransformMask.PositionX))
                tPosition.x = oPosition.x;
            if(mask.HasFlag(TransformMask.PositionY))
                tPosition.y = oPosition.y;
            if(mask.HasFlag(TransformMask.PositionZ))
                tPosition.z = oPosition.z;

            if(mask.HasFlag(TransformMask.RotationX))
                tRotation.x = oRotation.x;
            if(mask.HasFlag(TransformMask.RotationY))
                tRotation.y = oRotation.y;
            if(mask.HasFlag(TransformMask.RotationZ))
                tRotation.z = oRotation.z;

            if(mask.HasFlag(TransformMask.ScaleX))
                tScale.x = oScale.x;
            if(mask.HasFlag(TransformMask.ScaleY))
                tScale.y = oScale.y;
            if(mask.HasFlag(TransformMask.ScaleZ))
                tScale.z = oScale.z;


            if(space == Space.Self) {
                transform.localPosition = tPosition;
                transform.localEulerAngles = tRotation;
                transform.localScale = tScale;
            }
            else {
                transform.position = tPosition;
                transform.eulerAngles = tRotation;
                transform.SetLossyScale(tScale);
            }
        }

        #endregion

        #region Lossy Scale

        public static void SetLossyScale(this Transform transform, Vector3 scale) {
            var parent = transform.parent;
            if(parent) {
                var parentLossyScale = parent.lossyScale;
                transform.localScale = new Vector3(
                    scale.x / parentLossyScale.x,
                    scale.y / parentLossyScale.y,
                    scale.z / parentLossyScale.z);
            }
            else {
                transform.localScale = scale;
            }
        }

        #endregion

        #region Pose Util

        /// <summary>
        /// Get pose from world location.
        /// </summary>
        /// <returns>Pose using world position and rotation.</returns>
        public static Pose GetPose(this Transform transform)
            => new Pose(transform.position, transform.rotation);


        public static Pose GetPose(this Transform transform, Space space) {
            if(space == Space.Self) {
                return new Pose(transform.localPosition, transform.localRotation);
            }
            else {
                return GetPose(transform);
            }
        }

        #endregion

        #region Tranform Data Util

        public static TransformData GetTransformData(this Transform transform)
            => new TransformData(transform);

        public static TransformData GetTransformData(this Transform transform, Space space)
            => new TransformData(transform, space);

        /// <summary>
        /// Other is considered previous location
        /// </summary>
        public static TransformData GetDelta(this Transform transform, Transform other)
            => new TransformData(transform).GetDelta(other);

        #endregion

        #region Set Position And Rotation

        /// <summary>
        /// Set transforms world position from pose.
        /// </summary>
        /// <param name="pose">World position and rotation.</param>
        public static void SetPositionAndRotation(this Transform transform, Pose pose) {
            transform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        public static void SetPositionAndRotation(this Transform transform, Pose pose, Space space) {
            if(space == Space.Self) {
                transform.localPosition = pose.position;
                transform.localRotation = pose.rotation;
            }
            else
                transform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        #endregion

        #region Mirror

        public static void Mirror(this Transform transform, Transform reference, Transform mirrorReference)
            => Mirror(transform, reference, mirrorReference.GetPose());

        public static void Mirror(this Transform transform, Transform reference, Vector3 mirrorDirection) {
            Mirror(transform, reference, new Plane(mirrorDirection, 0f));
        }

        public static void Mirror(this Transform transform, Transform reference, Plane mirror) {
            var mirrorPosition = mirror.normal * mirror.distance;
            var pos = mirrorPosition + Vector3.Reflect(reference.position - mirrorPosition, mirror.normal);
            var rot = Quaternion.LookRotation(Vector3.Reflect(reference.forward, mirror.normal), Vector3.Reflect(reference.up, mirror.normal));
            var lossyScale = reference.lossyScale;
            transform.SetPositionAndRotation(pos, rot);
            transform.SetLossyScale(lossyScale);
        }

        public static void Mirror(this Transform transform, Transform reference, Pose pose) {
            var norm = (pose.rotation) * new Vector3(0, 0, 1);
            var pos = pose.position + Vector3.Reflect(reference.position - pose.position, norm);
            var rot = Quaternion.LookRotation(Vector3.Reflect(reference.forward, norm), Vector3.Reflect(reference.up, norm));
            var lossyScale = reference.lossyScale;
            transform.SetPositionAndRotation(pos, rot);
            transform.SetLossyScale(lossyScale);
        }

        #endregion

        #region Destroy

        public static void Destroy(this Transform transform) {
            transform.Destroy(0f);
        }

        public static void Destroy(this Transform transform, float delay) {
            MonoBehaviour.Destroy(transform.gameObject, delay);
        }

        public static void DestroyImmediate(this Transform transform) {
            MonoBehaviour.DestroyImmediate(transform.gameObject);
        }

        public static void DestroyAllChildren(this Transform transform) {
            var childCount = transform.childCount;
            if(childCount == 0)
                return;
            for(int i = childCount - 1; i >= 0; i--) {
                transform.GetChild(i).Destroy(0f);
            }
        }

        public static void DestroyAllChildrenImmediate(this Transform transform) {
            var childCount = transform.childCount;
            if(childCount == 0)
                return;
            for(int i = childCount - 1; i >= 0; i--) {
                transform.GetChild(i).DestroyImmediate();
            }
        }

        public static void DestroyAllChildren(this Transform transform, float delay) {
            var childCount = transform.childCount;
            if(childCount == 0)
                return;
            for(int i = childCount - 1; i >= 0; i--) {
                transform.GetChild(i).Destroy(delay);
            }
        }

        public static void DestroyAllChildrenAfter(this Transform transform, int startIndex) {
            var childCount = transform.childCount;
            if(childCount == 0)
                return;
            for(int i = childCount - 1; i >= startIndex; i--) {
                transform.GetChild(i).Destroy(0f);
            }
        }

        public static void DestroyAllChildrenAfter(this Transform transform, int startIndex, float delay) {
            var childCount = transform.childCount;
            if(childCount == 0)
                return;
            for(int i = childCount - 1; i >= startIndex; i--) {
                transform.GetChild(i).Destroy(delay);
            }
        }

        public static void DestroyAllChildrenBefore(this Transform transform, int index) {
            var childCount = Math.Min(transform.childCount, index);
            for(int i = childCount - 1; i >= 0; i--) {
                transform.GetChild(i).Destroy(0f);
            }
        }

        public static void DestroyAllChildrenBefore(this Transform transform, int index, float delay) {
            var childCount = Math.Min(transform.childCount, index);
            for(int i = childCount - 1; i >= 0; i--) {
                transform.GetChild(i).Destroy(delay);
            }
        }

        public static void DestroyAllChildrenBetween(this Transform transform, int startIndex, int endIndex) {
            var childCount = Math.Min(transform.childCount, endIndex);
            for(int i = childCount - 1; i >= startIndex; i--) {
                transform.GetChild(i).Destroy(0f);
            }
        }

        public static void DestroyAllChildrenBetween(this Transform transform, int startIndex, int endIndex, float delay) {
            var childCount = Math.Min(transform.childCount, endIndex);
            for(int i = childCount - 1; i >= startIndex; i--) {
                transform.GetChild(i).Destroy(delay);
            }
        }

        #endregion

        #region Set

        public static void SetY(this Transform t, float y) {
            var p = t.localPosition;
            p.y = y;
            t.localPosition = p;
        }

        public static void SetY(this Transform t, float y, Space space) {
            if(space == Space.World) {
                var p = t.position;
                p.y = y;
                t.position = p;
            }
            else {
                var p = t.localPosition;
                p.y = y;
                t.localPosition = p;
            }
        }

        public static void SetX(this Transform t, float x) {
            var p = t.localPosition;
            p.x = x;
            t.localPosition = p;
        }

        public static void SetX(this Transform t, float x, Space space) {
            if(space == Space.World) {
                var p = t.position;
                p.x = x;
                t.position = p;
            }
            else {
                var p = t.localPosition;
                p.x = x;
                t.localPosition = p;
            }
        }

        public static void SetZ(this Transform t, float z) {
            var p = t.localPosition;
            p.z = z;
            t.localPosition = p;
        }

        public static void SetZ(this Transform t, float z, Space space) {
            if(space == Space.World) {
                var p = t.position;
                p.z = z;
                t.position = p;
            }
            else {
                var p = t.localPosition;
                p.z = z;
                t.localPosition = p;
            }
        }

        #endregion

        #region RectTransform

        private static Vector3[] worldCornerCache = new Vector3[4];

        public static Vector2 GetCenterPositionInCanvas(this RectTransform rt) {
            rt.GetWorldCorners(worldCornerCache);
            var canvas = rt.GetComponentInParent<Canvas>();
            if(canvas == null)
                return Vector2.zero;
            var root = canvas.rootCanvas;

            Vector3 center = Vector3.zero;
            for(int i = 0; i < 4; i++) {
                center += root.transform.InverseTransformPoint(worldCornerCache[i]);
            }
            center /= 4f;
            return new Vector2(center.x, center.y);
        }

        public static RectTransform CreateChildRectTransform(this RectTransform rt, string name) {
            var go = new GameObject(name, typeof(RectTransform));
            var nrt = go.transform.ToRectTransform();
            nrt.transform.SetParent(rt, false);

            // Configure
            nrt.anchorMin = Vector2.zero;
            nrt.anchorMax = Vector2.one;
            nrt.sizeDelta = Vector2.zero;
            nrt.anchoredPosition = Vector2.zero;

            return nrt;
        }

        #endregion

        #region Detatch

        /// <summary>
        /// World Position Stay by default
        /// </summary>
        public static void DetachChildren(this Transform transform, Transform newParent) {
            if(newParent == null)
                transform.DetachChildren();
            else {
                for(int i = transform.childCount - 1; i >= 0; i--) {
                    transform.GetChild(i).SetParent(newParent);
                }
            }
        }
        /// <summary>
        /// World Position Stay by default
        /// </summary>
        public static void DetachChildren(this Transform transform, Transform newParent, bool worldPositionStay) {
            if(newParent == null)
                transform.DetachChildren();
            else {
                for(int i = transform.childCount - 1; i >= 0; i--) {
                    transform.GetChild(i).SetParent(newParent, worldPositionStay);
                }
            }
        }

        #endregion

        #region GetChildren

        public static IEnumerable<Transform> GetChildren(this Transform transform) {
            foreach(Transform t in transform)
                yield return t;
        }

        #endregion
    }
}
