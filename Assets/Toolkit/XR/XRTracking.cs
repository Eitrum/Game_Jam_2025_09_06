using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.XR
{
    [AddComponentMenu("Toolkit/XR/Tracking")]
    public class XRTracking : NullableBehaviour, IEarlyUpdate, IFixedUpdate, ITracking
    {
        #region Variables

        [SerializeField] private XRHand hand = XRHand.Left;
        [SerializeField] private bool highFrequencyUpdate = true;

        [SerializeField] private bool useOffset = false;
        [SerializeField] private Pose offset = Pose.identity;

        private bool doUpdates = true;
        private bool isCamera = false;

        #endregion

        #region Properties

        public XRHand Hand {
            get => isCamera ? (XRHand)0 : hand;
            set => hand = value;
        }

        public bool Enabled {
            get => enabled;
            set => enabled = value;
        }

        public bool Update {
            get => doUpdates;
            set => doUpdates = value;
        }

        public bool IsCamera {
            get {
#if UNITY_EDITOR
                if(!Application.isPlaying) {
                    return GetComponent<Camera>() != null;
                }
#endif
                return isCamera;
            }
        }

        public bool IsHighFrequncyUpdate {
            get => highFrequencyUpdate;
            set => highFrequencyUpdate = value;
        }

        public bool UseOffset {
            get => useOffset;
            set => useOffset = value;
        }

        public Transform Transform => transform;
        public Pose Pose => transform.GetPose(Space.Self);

        public Pose Offset {
            get => offset;
            set => offset = value;
        }

        bool INullable.IsNull => this == null;

        #endregion

        #region Init

        void Awake() {
            var cam = GetComponent<Camera>();
            if(cam) {
                isCamera = true;
                UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(cam, true);
                useOffset = false;
            }
        }

        private void OnEnable() {
            UpdateSystem.Subscribe(this as IEarlyUpdate);
            UpdateSystem.Subscribe(this as IFixedUpdate);
            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable() {
            Application.onBeforeRender -= OnBeforeRender;
            UpdateSystem.Unsubscribe(this as IEarlyUpdate);
            UpdateSystem.Unsubscribe(this as IFixedUpdate);
        }

        #endregion

        #region Update

        void IEarlyUpdate.EarlyUpdate(float dt) {
            if(doUpdates)
                UpdateTracking();
        }

        void IFixedUpdate.FixedUpdate(float dt) {
            if(doUpdates && highFrequencyUpdate)
                UpdateTracking();
        }

        [BeforeRenderOrder(-30000)]
        private void OnBeforeRender() {
            if(doUpdates && (highFrequencyUpdate || isCamera))
                UpdateTracking();
        }

        public void UpdateTracking() {
            var filter = XRSystem.GetTrackingPose(isCamera ? UnityEngine.XR.XRNode.Head : hand.ToXRNode(), out Pose pose);
            if(filter.HasFlag(PoseFlag.Position))
                transform.localPosition = useOffset ?
                    pose.position + (pose.rotation * offset.position) :
                    pose.position;
            if(filter.HasFlag(PoseFlag.Rotation))
                transform.localRotation = useOffset ?
                    pose.rotation * offset.rotation :
                    pose.rotation;
        }

        #endregion
    }
}
