
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    public static class CameraExtensions {

        #region Focus Transforms

        /// <summary>
        /// Determines target position and rotation to apply to the perspective <see cref="Camera"/> in order to fit the given <see cref="Bounds"/> into the field of view
        /// </summary>
        /// <param name="camera">The <see cref="Camera"/> for which to determine target transformations</param>
        /// <param name="targetPosition">The resulting target position</param>
        /// <param name="targetRotation">The resulting target orientation</param>
        /// <param name="bounds">The <see cref="Bounds"/> to fit into the view of the <see cref="Camera"/></param>
        /// <param name="additionalSpacing">Additional spacing factor (uses <code>(1 + spacing) * boundsSize</code>) to leave free around the given <see cref="Bounds"/></param>
        public static void GetPerspectiveFocusTransforms(this Camera camera, out Vector3 targetPosition, out Quaternion targetRotation, Bounds bounds, float additionalSpacing = 0f) {
            var objectSizes = bounds.size;
            var objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

            // Visible height 1 meter in front
            var cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);

            // Combined wanted distance from the object
            var distance = (1 + additionalSpacing) * objectSize / cameraView;

            // Estimated offset from the center to the outside of the object
            distance += 0.5f * objectSize;

            targetPosition = bounds.center - distance * camera.transform.forward;
            targetRotation = Quaternion.LookRotation(bounds.center - targetPosition);

            var maxExtent = bounds.extents.magnitude;
            var minDistance = maxExtent * (1 + additionalSpacing) / Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);

            targetPosition = bounds.center - camera.transform.forward * minDistance;

            // camera.nearClipPlane = minDistance - maxExtent;
            targetRotation = Quaternion.LookRotation(bounds.center - targetPosition);
        }

        /// <summary>
        /// Determines target position and size to apply to the orthographic <see cref="Camera"/> in order to fit the given <see cref="Bounds"/> into the field of view
        /// </summary>
        /// <param name="camera">The <see cref="Camera"/> for which to determine target transformations</param>
        /// <param name="targetPosition">The resulting target position</param>
        /// <param name="targetOrthographicSize">The resulting target size</param>
        /// <param name="bounds">The <see cref="Bounds"/> to fit into the view of the <see cref="Camera"/></param>
        /// <param name="additionalSpacing">Additional spacing factor (uses <code>(1 + spacing) * boundsSize</code>) to leave free around the given <see cref="Bounds"/></param>
        public static void GetOrthographicFocusTransforms(this Camera camera, out Vector3 targetPosition, out float targetOrthographicSize, Bounds bounds, float additionalSpacing = 0f) {
            var size = bounds.size;
            var factor = 1 + additionalSpacing;

            var width = size.x * factor;
            var height = size.y * factor;

            if(width > height) {
                targetOrthographicSize = Mathf.Abs(width) / camera.aspect / 2f;
            }
            else {
                targetOrthographicSize = Mathf.Abs(height) / 2f;
            }

            targetPosition = bounds.center;

            targetPosition -= Vector3.Project(targetPosition - camera.transform.position, camera.transform.forward);

            targetOrthographicSize = Mathf.Max(targetOrthographicSize, Mathf.Epsilon);
        }

        #endregion

        #region Corners

        private static Vector3[] nearClipPlaneCornerCache = new Vector3[4];
        private static Vector3[] farClipPlaneCornerCache = new Vector3[4];

        private static void CacheCorners(Camera c) {
            var invproj = Matrix4x4.Inverse( c.worldToCameraMatrix ) * Matrix4x4.Inverse( c.projectionMatrix );

            nearClipPlaneCornerCache[0] = invproj.MultiplyPoint(new Vector3(-1, -1, -1));
            nearClipPlaneCornerCache[1] = invproj.MultiplyPoint(new Vector3(-1, 1, -1));
            nearClipPlaneCornerCache[2] = invproj.MultiplyPoint(new Vector3(1, 1, -1));
            nearClipPlaneCornerCache[3] = invproj.MultiplyPoint(new Vector3(1, -1, -1));

            farClipPlaneCornerCache[0] = invproj.MultiplyPoint(new Vector3(-1, -1, 1));
            farClipPlaneCornerCache[1] = invproj.MultiplyPoint(new Vector3(-1, 1, 1));
            farClipPlaneCornerCache[2] = invproj.MultiplyPoint(new Vector3(1, 1, 1));
            farClipPlaneCornerCache[3] = invproj.MultiplyPoint(new Vector3(1, -1, 1));
        }

        public static Vector3[] GetAllCorners(this Camera c) {
            Vector3[] result = new Vector3[8];
            GetAllCornersNonAlloc(c, result);
            return result;
        }

        public static void GetAllCorners(this Camera c, out Vector3[] nearPlaneCorners, out Vector3[] farPlaneCorners) {
            if(c == null)
                throw new System.ArgumentNullException("camera is null");
            nearPlaneCorners = new Vector3[4];
            farPlaneCorners = new Vector3[4];
            CacheCorners(c);
            for(int i = 0; i < 4; i++)
                nearPlaneCorners[i] = nearClipPlaneCornerCache[i];
            for(int i = 0; i < 4; i++)
                farPlaneCorners[i] = farClipPlaneCornerCache[i];
        }

        public static void GetAllCornersNonAlloc(this Camera c, Vector3[] array) {
            if(array == null)
                throw new System.ArgumentNullException("array");
            if(array.Length < 8)
                throw new System.ArgumentException("array require at least a length of 8");
            if(c == null)
                throw new System.ArgumentNullException("camera is null");
            CacheCorners(c);
            for(int i = 0; i < 4; i++)
                array[i] = nearClipPlaneCornerCache[i];
            for(int i = 0; i < 4; i++)
                array[i + 4] = farClipPlaneCornerCache[i];
        }

        public static void GetAllCornersNonAlloc(this Camera c, Vector3[] nearPlaneCorners, Vector3[] farPlaneCorners) {
            if(nearPlaneCorners == null)
                throw new System.ArgumentNullException("nearPlaneCorners");
            if(farPlaneCorners == null)
                throw new System.ArgumentNullException("farPlaneCorners");
            if(nearPlaneCorners.Length < 4 && farPlaneCorners.Length < 4)
                throw new System.ArgumentException("array require at least a length of 8");
            if(c == null)
                throw new System.ArgumentNullException("camera is null");
            CacheCorners(c);
            for(int i = 0; i < 4; i++)
                nearPlaneCorners[i] = nearClipPlaneCornerCache[i];
            for(int i = 0; i < 4; i++)
                farPlaneCorners[i] = farClipPlaneCornerCache[i];
        }

        public static void GetAllCornersNonAlloc(this Camera c, List<Vector3> list) {
            if(list == null)
                throw new System.ArgumentNullException("list");
            if(c == null)
                throw new System.ArgumentNullException("camera is null");
            CacheCorners(c);
            list.Clear();
            list.AddRange(nearClipPlaneCornerCache);
            list.AddRange(farClipPlaneCornerCache);
        }

        #endregion

        #region AABB

        public static Bounds GetAABB(this Camera c) {
            if(c == null)
                throw new System.ArgumentNullException("camera is null");
            CacheCorners(c);
            Bounds b = new Bounds();
            b.center = nearClipPlaneCornerCache[0];
            for(int i = 1; i < 4; i++)
                b.Encapsulate(nearClipPlaneCornerCache[i]);
            for(int i = 0; i < 4; i++)
                b.Encapsulate(farClipPlaneCornerCache[i]);

            return b;
        }

        public static Bounds GetAABB(this Camera c, float maximumRenderDistance) {
            if(c == null)
                throw new System.ArgumentNullException("camera is null");

            // Temporarily swap maximum to fit within maximum render distance.
            var oldFarClip = c.farClipPlane;
            c.farClipPlane = Mathf.Min(maximumRenderDistance, c.farClipPlane);
            CacheCorners(c);
            c.farClipPlane = oldFarClip;

            Bounds b = new Bounds();
            b.center = nearClipPlaneCornerCache[0];
            for(int i = 1; i < 4; i++)
                b.Encapsulate(nearClipPlaneCornerCache[i]);
            for(int i = 0; i < 4; i++)
                b.Encapsulate(farClipPlaneCornerCache[i]);

            return b;
        }

        #endregion
    }
}
