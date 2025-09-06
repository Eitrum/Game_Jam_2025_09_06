using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Toolkit.XR
{
    public enum XRHand
    {
        Left = 1,
        Right = 2,
    }

    public enum XRHandMask
    {
        None = 0,

        Left = 1,

        Right = 2,

        /// <summary>
        /// Used for filters that allows for both hands.
        /// </summary>
        Both = Left | Right
    }

    public static class HandUtility
    {
        #region Conversion XRNode

        /// <summary>
        /// Conversion between Toolkit Hand to Unity XRNode
        /// </summary>
        /// <returns>If no specific hand is specified, returns -1</returns>
        public static UnityEngine.XR.XRNode ToXRNode(this XRHandMask hand) {
            switch(hand) {
                case XRHandMask.Left: return XRNode.LeftHand;
                case XRHandMask.Right: return XRNode.RightHand;
            }
            return (XRNode)(-1);
        }

        /// <summary>
        /// Conversion from Unity XRNode to Hand
        /// </summary>
        /// <returns>If left or right hand is not specified, returns Hand.none</returns>
        public static XRHandMask ToHandMask(this UnityEngine.XR.XRNode node) {
            switch(node) {
                case XRNode.LeftHand: return XRHandMask.Left;
                case XRNode.RightHand: return XRHandMask.Right;
            }
            return XRHandMask.None;
        }

        /// <summary>
        /// Conversion between Toolkit Hand to Unity XRNode
        /// </summary>
        /// <returns>If no specific hand is specified, returns -1</returns>
        public static UnityEngine.XR.XRNode ToXRNode(this XRHand hand) {
            switch(hand) {
                case XRHand.Left: return XRNode.LeftHand;
                case XRHand.Right: return XRNode.RightHand;
            }
            return (XRNode)(-1);
        }

        /// <summary>
        /// Conversion from Unity XRNode to Hand
        /// </summary>
        /// <returns>If left or right hand is not specified, returns Hand.none</returns>
        public static XRHand ToHand(this UnityEngine.XR.XRNode node) {
            switch(node) {
                case XRNode.LeftHand: return XRHand.Left;
                case XRNode.RightHand: return XRHand.Right;
            }
            return (XRHand)(0);
        }

        #endregion

        #region Conversion Mask

        public static XRHand ToHand(this XRHandMask mask) {
            switch(mask) {
                case XRHandMask.Left: return XRHand.Left;
                case XRHandMask.Right: return XRHand.Right;
            }
            return (XRHand)(0);
        }

        public static XRHandMask ToMask(this XRHand mask) {
            switch(mask) {
                case XRHand.Left: return XRHandMask.Left;
                case XRHand.Right: return XRHandMask.Right;
            }
            return XRHandMask.None;
        }

        #endregion

        #region Simplified Check

        public static bool IsLeft(this XRHandMask hand)
            => hand.HasFlag(XRHandMask.Left);

        public static bool IsRight(this XRHandMask hand)
            => hand.HasFlag(XRHandMask.Right);

        public static bool IsValid(this XRHandMask hand)
            => hand > XRHandMask.None && hand < XRHandMask.Both;

        #endregion
    }
}
