using System;
using UnityEngine;

namespace Toolkit
{
    public static class RigidbodyExtensions
    {
        #region Freeze

        public static void Freeze(this Rigidbody2D body) {
            if(body == null)
                return;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        public static void Freeze(this Rigidbody2D body, bool includeConstraints) {
            if(body == null)
                return;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            if(includeConstraints)
                body.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public static void Freeze(this Rigidbody body) {
            if(body == null)
                return;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        public static void Freeze(this Rigidbody body, bool includeConstraints) {
            if(body == null)
                return;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if(includeConstraints)
                body.constraints = RigidbodyConstraints.FreezeAll;
        }

        // NOT WHAT ITS SUPPOSED TO DO, but good for spaghetti
        public static void Unfreeze(this Rigidbody body, bool includeConstraints) {
            if(body == null)
                return;
            body.isKinematic = false;
            body.useGravity = true;
            if(includeConstraints)
                body.constraints = RigidbodyConstraints.None;
        }

        #endregion

        #region Ground Check

        public const float DEFAULT_GROUND_DISTANCE = 0.05f;

        public static bool IsGrounded(this Rigidbody body)
            => IsGrounded(body, out RaycastHit hit, DEFAULT_GROUND_DISTANCE);

        public static bool IsGrounded(this Rigidbody body, float distance)
            => IsGrounded(body, out RaycastHit hit, distance);

        public static bool IsGrounded(this Rigidbody body, out RaycastHit hit) {
            body.position += (new Vector3(0f, 0.02f, 0f));
            var res = body.SweepTest(Vector3.down, out hit, DEFAULT_GROUND_DISTANCE);
            body.position += (new Vector3(0f, -0.02f, 0f));
            return res;
        }

        public static bool IsGrounded(this Rigidbody body, out RaycastHit hit, float distance) {
            body.position += (new Vector3(0f, 0.02f, 0f));
            var res = body.SweepTest(Vector3.down, out hit, distance);
            body.position += (new Vector3(0f, -0.02f, 0f));
            return res;
        }

        #endregion

        #region SweepTest

        /// <summary>
        /// Does a sweep test in the velocity direction for 1 frame forward
        /// </summary>
        public static bool SweepTest(this Rigidbody body, out RaycastHit hit) {
            return body.SweepTest(body.linearVelocity, out hit, Time.fixedDeltaTime);
        }

        /// <summary>
        /// Does a raycast in the velocity of rigidbody for 1 frame forward
        /// </summary>
        public static bool Raycast(this Rigidbody body, out RaycastHit hit) {
            return Physics.Raycast(body.position, body.linearVelocity, out hit, Time.fixedDeltaTime, ~0, QueryTriggerInteraction.Ignore);
        }

        #endregion

        #region Change Direction

        public static void ChangeDirection(this Rigidbody body, Vector3 newDirection) {
            body.linearVelocity = newDirection.normalized * body.linearVelocity.magnitude;
        }

        #endregion

        #region Angular Velocity

        /// <summary>
        /// Sets the angular velocity in radians
        /// </summary>
        /// <param name="body"></param>
        /// <param name="angularVelocity"></param>
        public static void SetAngularVelocity(this Rigidbody body, Vector3 angularVelocity) {
            body.angularVelocity = angularVelocity;
        }

        /// <summary>
        /// Sets the angular velocity by the rotation values
        /// </summary>
        /// <param name="body"></param>
        /// <param name="rotation"></param>
        public static void SetAngularVelocity(this Rigidbody body, Quaternion rotation) {
            body.angularVelocity = rotation.ToAngularVelocity();
        }

        /// <summary>
        /// In degrees per second.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="angle"></param>
        public static void SetAngularVelocity(this Rigidbody2D body, float angle) {
            body.angularVelocity = angle;
        }

        /// <summary>
        /// Untested, maybe works
        /// </summary>
        /// <param name="body"></param>
        /// <param name="rotation"></param>
        public static void SetAngularVelocity(this Rigidbody2D body, Quaternion rotation) {
            body.angularVelocity = rotation.ToAngularVelocity().z * Mathf.Rad2Deg;
        }

        #endregion
    }
}
