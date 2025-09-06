using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Toolkit.XR
{
    public static class XRSystem
    {
        #region Variables

        private static List<XRNodeState> nodeStates = new List<XRNodeState>();

        #endregion

        #region Properties

        public static bool IsDeviceActive => UnityEngine.XR.XRSettings.isDeviceActive;
        public static UnityEngine.XR.XRSettings.StereoRenderingMode RenderingMode => XRSettings.stereoRenderingMode;

        #endregion

        #region Initialize
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {

        }

        #endregion

        #region Input

        public static bool GetTrackingState(XRHand hand, out XRNodeState state)
            => GetTrackingState(hand.ToXRNode(), out state);

        public static bool GetTrackingState(XRHandMask hand, out XRNodeState state)
            => GetTrackingState(hand.ToXRNode(), out state);

        public static bool GetTrackingState(XRNode node, out XRNodeState state) {
            UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
            for(int i = 0, length = nodeStates.Count; i < length; i++) {
                if(nodeStates[i].nodeType == node) {
                    state = nodeStates[i];
                    return true;
                }
            }
            state = default;
            return false;
        }

        public static PoseFlag GetTrackingPose(Toolkit.XR.XRHand hand, out Pose pose)
            => GetTrackingPose(hand.ToXRNode(), out pose);

        public static PoseFlag GetTrackingPose(Toolkit.XR.XRHandMask hand, out Pose pose)
           => GetTrackingPose(hand.ToXRNode(), out pose);

        public static PoseFlag GetTrackingPose(XRNode node, out Pose pose) {
            UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
            for(int i = 0, length = nodeStates.Count; i < length; i++) {
                if(nodeStates[i].nodeType == node) {
                    var state = nodeStates[i];
                    PoseFlag result = PoseFlag.None;

                    if(state.TryGetPosition(out pose.position))
                        result |= PoseFlag.Position;
                    if(state.TryGetRotation(out pose.rotation))
                        result |= PoseFlag.Rotation;

                    return result;
                }
            }

            pose = Pose.identity;
            return PoseFlag.None;
        }

        public static PoseFlag GetTrackingVelocity(Toolkit.XR.XRHand hand, out Vector3 velocity, out Vector3 angularVelocity)
            => GetTrackingVelocity(hand.ToXRNode(), out velocity, out angularVelocity);

        public static PoseFlag GetTrackingVelocity(Toolkit.XR.XRHandMask hand, out Vector3 velocity, out Vector3 angularVelocity)
           => GetTrackingVelocity(hand.ToXRNode(), out velocity, out angularVelocity);

        public static PoseFlag GetTrackingVelocity(XRNode node, out Vector3 velocity, out Vector3 angularVelocity) {
            UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
            for(int i = 0, length = nodeStates.Count; i < length; i++) {
                if(nodeStates[i].nodeType == node) {
                    var state = nodeStates[i];
                    PoseFlag result = PoseFlag.None;

                    if(state.TryGetVelocity(out velocity))
                        result |= PoseFlag.Position;
                    if(state.TryGetAngularVelocity(out angularVelocity))
                        result |= PoseFlag.Rotation;

                    return result;
                }
            }

            velocity = new Vector3();
            angularVelocity = new Vector3();
            return PoseFlag.None;
        }

        #endregion
    }
}
