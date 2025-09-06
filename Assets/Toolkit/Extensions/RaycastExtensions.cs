using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public static class RaycastExtensions
    {
        #region Const

        public const float HIT_DISTANCE_FAIL = -1;

        #endregion

        #region Quick Hit 

        public static bool Hit(this Ray ray)
            => Physics.Raycast(ray);

        public static bool Hit(this Ray ray, out RaycastHit hit)
            => Physics.Raycast(ray, out hit);

        public static bool Hit(this Ray ray, out RaycastHit hit, float maxDistance)
            => Physics.Raycast(ray, out hit, maxDistance);

        public static bool Hit(this Ray ray, out RaycastHit hit, float maxDistance, int layerMask)
            => Physics.Raycast(ray, out hit, maxDistance, layerMask);

        public static bool Hit(this Ray ray, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            => Physics.Raycast(ray, out hit, maxDistance, layerMask, queryTriggerInteraction);

        #endregion

        #region Component Hit Check

        private static T GetComponent<T>(RaycastHit hit, bool checkParents)
            => checkParents ? hit.collider.GetComponentInParent<T>() : hit.collider.GetComponent<T>();


        public static bool Hit<T>(this Ray ray, out T component)
            => (component = Physics.Raycast(ray, out RaycastHit hit) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents)
            => (component = Physics.Raycast(ray, out RaycastHit hit) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, float maxDistance)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, float maxDistance, int layerMask)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, queryTriggerInteraction) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, queryTriggerInteraction) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, int layerMask)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~0, queryTriggerInteraction) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask, queryTriggerInteraction) ? hit.collider.GetComponent<T>() : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, float maxDistance)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, float maxDistance, int layerMask)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, queryTriggerInteraction) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, queryTriggerInteraction) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, int layerMask)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~0, queryTriggerInteraction) ? GetComponent<T>(hit, checkParents) : default) != null;

        public static bool Hit<T>(this Ray ray, out T component, bool checkParents, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            => (component = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask, queryTriggerInteraction) ? GetComponent<T>(hit, checkParents) : default) != null;

        #endregion

        #region Hit Distance

        public static float HitDistance(this Ray ray)
            => Physics.Raycast(ray, out RaycastHit hit, ray.direction.magnitude, ~0, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;

        public static float HitDistance(this Ray ray, float maxDistance)
            => Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;

        public static float HitDistance(this Ray ray, out RaycastHit hit, float maxDistance)
            => Physics.Raycast(ray, out hit, maxDistance, ~0, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;

        public static float HitDistance(this Ray ray, out RaycastHit hit, float maxDistance, int layerMask)
            => Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;

        public static float HitDistance(this Ray ray, int layerMask)
            => Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;

        public static float HitDistance(this Ray ray, float maxDistance, int layerMask)
            => Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore) ? hit.distance : HIT_DISTANCE_FAIL;
        #endregion

        #region Vision

        public static bool HasVision(this Transform source, Transform target)
            => HasVision(source, target, ~0, QueryTriggerInteraction.UseGlobal);

        public static bool HasVision(this Transform source, Transform target, int layerMask)
            => HasVision(source, target, layerMask, QueryTriggerInteraction.UseGlobal);

        public static bool HasVision(this Transform source, Transform target, int layerMask, QueryTriggerInteraction queryTriggerInteraction) {
            var sVec = source.position;
            var tVec = target.position;
            var dir = tVec - sVec;
            var distance = dir.magnitude;
            dir /= distance;

            var ray = new Ray(sVec, dir);
            if(Physics.Raycast(ray, out RaycastHit hit, distance, layerMask, queryTriggerInteraction)) {
                return hit.transform == target;
            }
            return true;
        }

        public static bool HasVision(this Vector3 source, Vector3 target)
            => HasVision(source, target, ~0, QueryTriggerInteraction.UseGlobal);

        public static bool HasVision(this Vector3 source, Vector3 target, int layerMask)
            => HasVision(source, target, layerMask, QueryTriggerInteraction.UseGlobal);

        public static bool HasVision(this Vector3 source, Vector3 target, int layerMask, QueryTriggerInteraction queryTriggerInteraction) {
            var dir = source - target;
            var distance = dir.magnitude;
            dir /= distance;

            var ray = new Ray(source, dir);
            if(Physics.Raycast(ray, out RaycastHit hit, distance, layerMask, queryTriggerInteraction)) {
                return false;
            }
            return true;
        }

        #endregion
    }
}
