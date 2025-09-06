using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.XR
{
    public class Grabable : MonoBehaviour, IGrabable
    {
        public enum SnapOffsetType
        {
            None = 0,
            Offset = 1,
            Absolute = 2,
        }

        #region Variables

        [SerializeField] private bool canSwitchHands = false;
        [SerializeField] private bool grabable = true;
        [SerializeField] private bool setAsChildOfHand = true;

        [SerializeField] private bool disableRigidbodyOnGrab = true;

        [Header("Snap")]
        [SerializeField] private SnapType snapType = SnapType.Instant;
        [SerializeField] private float snapDelay = 1f;
        [SerializeField] private SnapOffsetType snapPosition = SnapOffsetType.Absolute;
        [SerializeField] private Vector3 snapPositionOffset = Vector3.zero;
        [SerializeField] private SnapOffsetType snapRotation = SnapOffsetType.Absolute;
        [SerializeField] private Vector3 snapRotationOffset = Vector3.zero;

        private Pose inversedGrabPosition;
        private RigidbodyConstraints constraints;
        private IHand hand;
        private Rigidbody body;
        private Transform oldParent;

        #endregion

        #region Properties

        public Transform Transform => transform;
        public Rigidbody Body => body;
        public bool IsHeld => hand != null;
        public IHand Holder => hand;

        public float SnapDelay => snapDelay;
        public SnapOffsetType SnapPosition => snapPosition;
        public SnapOffsetType SnapRotation => snapRotation;

        public bool IsNull => this == null;

        public event OnGrabDelegate OnGrab;
        public event OnReleaseDelegate OnRelease;
        public event OnUpdateDelegate OnUpdate;

        #endregion

        #region Init

        void Awake() {
            body = GetComponent<Rigidbody>();
        }

        void OnDisable() {
            Release(hand);
        }

        #endregion

        #region IGrabable Impl

        public bool CanGrab(IHand hand) {
            return grabable && canSwitchHands || !IsHeld;
        }

        public bool Grab(IHand hand) {
            if(!CanGrab(hand))
                return false;
            if(setAsChildOfHand) {
                if(!IsHeld)
                    oldParent = this.transform.parent;
                this.transform.SetParent(hand.Tracking.Transform, true);
            }
            if(snapType == SnapType.Instant) {
                this.transform.SetPositionAndRotation(hand.Tracking.Transform.GetPose(Space.World), Space.World);
            }
            this.inversedGrabPosition = GrabUtility.InverseGrabPose(hand.Tracking.Pose, transform, Space.Self);
            if(disableRigidbodyOnGrab && body) {
                if(!IsHeld)
                    constraints = body.constraints;
                body.Freeze(true);
            }
            this.hand = hand;
            OnGrab?.Invoke(hand);
            return true;
        }

        public bool Release(IHand hand) {
            if(!IsHeld || hand != this.hand)
                return false;
            this.hand = null;
            this.transform.SetParent(oldParent);
            if(disableRigidbodyOnGrab && body) {
                body.constraints = constraints;
            }
            OnRelease?.Invoke(hand);
            return true;
        }

        public void UpdateGrabable(IHand hand, float dt) {
            if(this.hand != hand)
                return;


            if(setAsChildOfHand)
                return;
            var localPose = GrabUtility.UpdateGrabPose(hand.Tracking.Pose, this.inversedGrabPosition);
            this.transform.localPosition = localPose.position;
            this.transform.localRotation = localPose.rotation;
            OnUpdate?.Invoke(hand, dt);
        }

        #endregion
    }
}
