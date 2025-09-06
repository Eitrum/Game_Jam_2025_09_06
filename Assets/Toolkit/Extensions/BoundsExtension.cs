using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit {
    public static class BoundsExtension {
        #region ColliderToBounds

        public static Bounds GetBounds(this IEnumerable<Collider> colliders)
            => GetBounds(colliders, false, false);

        public static Bounds GetBounds(this IEnumerable<Collider> colliders, bool includeTriggers)
            => GetBounds(colliders, includeTriggers, false);

        public static Bounds GetBounds(this IEnumerable<Collider> colliders, bool includeTriggers, bool includeDisabled) {
            float up = 0f, down = 0f, left = 0f, right = 0f, forward = 0f, backward = 0f;
            foreach(var col in colliders) {
                if((!includeTriggers && col.isTrigger) || (!includeDisabled && !col.enabled))
                    continue;

                var bound = col.bounds;
                var extents = bound.extents;
                var center = bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        public static Bounds GetBounds(this IEnumerable<Collider> colliders, Quaternion rotate)
            => GetBounds(colliders, rotate, false, false);

        public static Bounds GetBounds(this IEnumerable<Collider> colliders, Quaternion rotate, bool includeTriggers)
            => GetBounds(colliders, rotate, includeTriggers, false);

        public static Bounds GetBounds(this IEnumerable<Collider> colliders, Quaternion rotate, bool includeTriggers, bool includeDisabled) {
            float up = 0f, down = 0f, left = 0f, right = 0f, forward = 0f, backward = 0f;
            foreach(var col in colliders) {
                if((!includeTriggers && col.isTrigger) || (!includeDisabled && !col.enabled))
                    continue;

                var bound = col.bounds;
                var extents = rotate * bound.extents;
                var center = rotate * bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders)
            => GetBounds(colliders, false, false);

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders, bool includeTriggers)
            => GetBounds(colliders, includeTriggers, false);

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders, bool includeTriggers, bool includeDisabled) {
            float up = 0f, down = 0f, left = 0f, right = 0f, forward = 0f, backward = 0f;
            for(int i = 0, length = colliders.Count; i < length; i++) {
                var col = colliders[i];

                if((!includeTriggers && col.isTrigger) || (!includeDisabled && !col.enabled))
                    continue;

                var bound = col.bounds;
                var extents = bound.extents;
                var center = bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders, Quaternion rotate)
            => GetBounds(colliders, rotate, false, false);

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders, Quaternion rotate, bool includeTriggers)
            => GetBounds(colliders, rotate, includeTriggers, false);

        public static Bounds GetBounds(this IReadOnlyList<Collider> colliders, Quaternion rotate, bool includeTriggers, bool includeDisabled) {
            float up = 0f, down = 0f, left = 0f, right = 0f, forward = 0f, backward = 0f;
            for(int i = 0, length = colliders.Count; i < length; i++) {
                var col = colliders[i];

                if((!includeTriggers && col.isTrigger) || (!includeDisabled && !col.enabled))
                    continue;

                var bound = col.bounds;
                var extents = rotate * bound.extents;
                var center = rotate * bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        #endregion

        #region Combine

        public static Bounds Combine(this IEnumerable<Bounds> bounds) {
            float up = float.MinValue, down = float.MaxValue, left = float.MaxValue, right = float.MinValue, forward = float.MinValue, backward = float.MaxValue;
            foreach(var bound in bounds) {
                var extents = bound.extents;
                var center = bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        public static Bounds Combine(this IReadOnlyList<Bounds> bounds) {
            float up = float.MinValue, down = float.MaxValue, left = float.MaxValue, right = float.MinValue, forward = float.MinValue, backward = float.MaxValue;
            for(int i = 0, length = bounds.Count; i < length; i++) {
                var bound = bounds[i];
                var extents = bound.extents;
                var center = bound.center;

                up = Math.Max(up, extents.y + center.y);
                down = Math.Min(down, -extents.y + center.y);
                right = Math.Max(right, extents.x + center.x);
                left = Math.Min(left, -extents.x + center.x);
                forward = Math.Max(forward, extents.z + center.z);
                backward = Math.Min(backward, -extents.z + center.z);
            }
            return new Bounds(
                new Vector3(
                    (right + left) / 2f,
                    (up + down) / 2f,
                    (forward + backward) / 2f),
                new Vector3(
                    right - left,
                    up - down,
                    forward - backward));
        }

        #endregion

        #region Subdivide

        public static void Subdivide(this Bounds bounds, out Bounds nwb, out Bounds neb, out Bounds swb, out Bounds seb, out Bounds nwt, out Bounds net, out Bounds swt, out Bounds set) {
            var halfSize = bounds.extents;
            var qrtSize = halfSize / 2f;
            var center = bounds.center;
            nwb = new Bounds(center + new Vector3(-qrtSize.x, -qrtSize.y, qrtSize.z), halfSize);
            neb = new Bounds(center + new Vector3(qrtSize.x, -qrtSize.y, qrtSize.z), halfSize);
            swb = new Bounds(center + new Vector3(-qrtSize.x, -qrtSize.y, -qrtSize.z), halfSize);
            seb = new Bounds(center + new Vector3(qrtSize.x, -qrtSize.y, -qrtSize.z), halfSize);
            nwt = new Bounds(center + new Vector3(-qrtSize.x, qrtSize.y, qrtSize.z), halfSize);
            net = new Bounds(center + new Vector3(qrtSize.x, qrtSize.y, qrtSize.z), halfSize);
            swt = new Bounds(center + new Vector3(-qrtSize.x, qrtSize.y, -qrtSize.z), halfSize);
            set = new Bounds(center + new Vector3(qrtSize.x, qrtSize.y, -qrtSize.z), halfSize);
        }

        public static Bounds[] Subdivide(this Bounds bounds) {
            var halfSize = bounds.extents;
            var qrtSize = halfSize / 2f;
            var center = bounds.center;
            var arr = new Bounds[8];
            arr[0] = new Bounds(center + new Vector3(-qrtSize.x, -qrtSize.y, qrtSize.z), halfSize);
            arr[1] = new Bounds(center + new Vector3(qrtSize.x, -qrtSize.y, qrtSize.z), halfSize);
            arr[2] = new Bounds(center + new Vector3(-qrtSize.x, -qrtSize.y, -qrtSize.z), halfSize);
            arr[3] = new Bounds(center + new Vector3(qrtSize.x, -qrtSize.y, -qrtSize.z), halfSize);
            arr[4] = new Bounds(center + new Vector3(-qrtSize.x, qrtSize.y, qrtSize.z), halfSize);
            arr[5] = new Bounds(center + new Vector3(qrtSize.x, qrtSize.y, qrtSize.z), halfSize);
            arr[6] = new Bounds(center + new Vector3(-qrtSize.x, qrtSize.y, -qrtSize.z), halfSize);
            arr[7] = new Bounds(center + new Vector3(qrtSize.x, qrtSize.y, -qrtSize.z), halfSize);
            return arr;
        }

        #endregion

        #region Intersection

        public static bool Intersects(this Bounds bounds, Bounds other, out Bounds intersection) {
            var c0 = bounds.min;
            var c1 = bounds.max;

            var o0 = other.min;
            var o1 = other.max;

            intersection = new Bounds();
            intersection.min = new Vector3(
                Mathf.Max(c0.x, o0.x),
                Mathf.Max(c0.y, o0.y),
                Mathf.Max(c0.z, o0.z));
            intersection.max = new Vector3(
                Mathf.Min(c1.x, o1.x),
                Mathf.Min(c1.y, o1.y),
                Mathf.Min(c1.z, o1.z));

            var size = intersection.size;

            return size.x < 0f || size.y < 0f || size.z < 0f;
        }

        #endregion

        #region Rotate

        public static Bounds Rotate(this Bounds bounds, Quaternion rotation) {
            var extents = bounds.extents;

            var c000 = bounds.center + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            var c100 = bounds.center + rotation * new Vector3(extents.x, -extents.y, -extents.z);
            var c101 = bounds.center + rotation * new Vector3(extents.x, -extents.y, extents.z);
            var c001 = bounds.center + rotation * new Vector3(-extents.x, -extents.y, extents.z);

            var c010 = bounds.center + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            var c110 = bounds.center + rotation * new Vector3(extents.x, extents.y, -extents.z);
            var c111 = bounds.center + rotation * new Vector3(extents.x, extents.y, extents.z);
            var c011 = bounds.center + rotation * new Vector3(-extents.x, extents.y, extents.z);

            bounds.Encapsulate(c000);
            bounds.Encapsulate(c100);
            bounds.Encapsulate(c101);
            bounds.Encapsulate(c001);

            bounds.Encapsulate(c010);
            bounds.Encapsulate(c110);
            bounds.Encapsulate(c111);
            bounds.Encapsulate(c011);
            return bounds;
        }

        #endregion

        #region GetPointInside

        public static Vector3 GetPointFromLerp(this Bounds bounds, float x, float y, float z) {
            var c = bounds.center;
            var s = bounds.extents;
            return c + new Vector3(
                Mathf.Lerp(-s.x, s.x, x),
                Mathf.Lerp(-s.y, s.y, y),
                Mathf.Lerp(-s.z, s.z, z));
        }

        #endregion
    }
}
