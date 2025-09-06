using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CreateAssetMenu(menuName = "Toolkit/Physics/Follow Target Settings")]
    public class FollowTargetWithPhysicsSettings : ScriptableObject
    {
        #region Variables

        [SerializeField] private float maxVelocity = 80f;
        [SerializeField] private float maxTorque = 360f;
        [SerializeField, RangeEx(0f, 1f, 0.01f)] private float sensitivity = 1f;
        [SerializeField, RangeEx(0f, 4f, 0.05f)] private float prediction = 1f;
        [SerializeField, Min(0.001f)] private float shakinessSmoothing = 1f;

        private static FollowTargetWithPhysicsSettings @default;

        #endregion

        #region Properties

        public static FollowTargetWithPhysicsSettings Default {
            get {
                if(!@default) {
                    @default = CreateInstance<FollowTargetWithPhysicsSettings>();
                }
                return @default;
            }
        }

        public float MaxVelocity => maxVelocity;
        public float MaxTorque => maxTorque;
        public float Sensitivity => sensitivity;
        public float Prediction => prediction;
        public float ShakinessSmoothing => shakinessSmoothing;

        #endregion
    }
}
