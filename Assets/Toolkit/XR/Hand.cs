using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.XR
{
    public class Hand : MonoBehaviour, IHand
    {
        #region Variables

        private const string TAG = "[XR.Hand] - ";
        private const int MAX_COLLIDERS = 32;

        [SerializeField] private float grabRadius = 0.25f;
        [SerializeField] private bool canGrabMultipleObjects = false;
        [SerializeField] private LayerMask mask = ~0;

        private ITracking tracking;
        private List<IGrabable> grabables = new List<IGrabable>();
        private static Collider[] colliders = new Collider[MAX_COLLIDERS];

        #endregion

        #region Properties

        public float GripStrength => 0f;
        public ITracking Tracking => tracking;
        XRHand IHand.Hand => tracking.Hand;

        #endregion

        #region Init

        void Awake() {
            tracking = GetComponent<ITracking>();
        }

        void OnDisable() {
            Release();
        }

        #endregion

        #region Hand Impl

        void LateUpdate() {
            var dt = Time.deltaTime;
            for(int i = 0, length = grabables.Count; i < length; i++) {
                grabables[i].UpdateGrabable(this, dt);
            }
            if(Input.GetKeyDown(KeyCode.JoystickButton0)) {
                Grab();
            }
            if(Input.GetKeyUp(KeyCode.JoystickButton0)) {
                Release();
            }
        }

        [ContextMenu("Grab")]
        public void Grab() {
            if(!canGrabMultipleObjects && grabables.Count > 0) {
                Debug.LogWarning(TAG + $"unable to grab multiple objects!");
                return;
            }
            Debug.Log(TAG + "Attempting grab!");

            var position = Tracking.Transform.position;
            var hits = Physics.OverlapSphereNonAlloc(position, grabRadius, colliders, mask.value);
            Debug.Log(TAG + $"Found '{hits}' colliders!");

            //Sort.Quick(colliders, (a, b) => (a.transform.position - position).sqrMagnitude.CompareTo((b.transform.position - position).sqrMagnitude), hits);
            for(int i = 0; i < hits; i++) {
                var g = colliders[i].GetComponentInParent<IGrabable>();
                if(g != null && g.Grab(this)) {
                    this.grabables.Add(g);
                    Debug.Log(TAG + "Grabbing object!");
                    if(!canGrabMultipleObjects)
                        break;
                }
            }
        }

        [ContextMenu("Release")]
        public void Release() {
            Debug.Log(TAG + "Attempting release!");
            for(int i = grabables.Count - 1; i >= 0; i--) {
                grabables[i].Release(this);
            }
            grabables.Clear();
        }

        #endregion

        #region Editor

        void OnDrawGizmosSelected() {
            if(tracking == null)
                tracking = GetComponent<ITracking>();

            if(tracking != null) {
                Gizmos.DrawWireSphere(tracking.Pose.position, grabRadius);
            }
        }

        #endregion
    }
}
