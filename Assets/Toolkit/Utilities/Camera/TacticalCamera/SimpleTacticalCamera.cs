using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Camera/Simple Tactical Camera (Mouse)")]
    public class SimpleTacticalCamera : MonoBehaviour
    {
        #region Variables

        [Header("Input")]
        [SerializeField] private KeyCode rotation = KeyCode.Mouse1;
        [SerializeField] private bool hideMouse = true;
        [SerializeField] private bool functionKeys = true;

        [Header("Rotation")]
        [SerializeField] private Transform rotationTransform = null;
        [SerializeField] private float rotationSpeedMultiplier = 10f;
        [SerializeField] private KeyCode rotateCameraLeft = KeyCode.Q;
        [SerializeField] private KeyCode rotateCameraRight = KeyCode.E;

        [Header("Tilt Settings")]
        [SerializeField] private Transform tiltTransform = null;
        [SerializeField] private MinMax tiltRange = new MinMax(8, 85);
        [SerializeField] private float tiltSpeedMultiplier = -10f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform = null;
        [SerializeField] private MinMax zoomRange = new MinMax(5, 50);
        [SerializeField] private float zoomSpeedMultiplier = 5f;
        [SerializeField] private bool raycastMaxDistance = false;
        [SerializeField] private float safetyDistance = 0.2f;
        [SerializeField] private LayerMask environmentMask = ~1;

        [Header("Movement")]
        [SerializeField] private Transform movementTransform = null;
        [SerializeField] private MinMax movementSpeed = new MinMax(5, 15);
        [Space]
        [SerializeField] private bool allowFollowTarget = true;
        [SerializeField] private bool canBreakFollowTarget = true;
        [SerializeField] private FollowMode mode = FollowMode.Lerp;
        [SerializeField] private float speed = 4f;
        [SerializeField] private bool allowEdgeScroll = false;

        [System.NonSerialized] private Transform followTarget = null;
        [System.NonSerialized] private Camera camReference = null;

        private float targetDistance = 0f;
        private int rotationInputStack = 0;
        private float tiltRotSpeedMultiplier = 0f;

        #endregion

        #region Properties

        public float RotationSpeed {
            get => rotationSpeedMultiplier;
            set {
                rotationSpeedMultiplier = Mathf.Clamp(value, 1f, 40f);
                tiltSpeedMultiplier = rotationSpeedMultiplier * tiltRotSpeedMultiplier;
            }
        }

        public Camera Camera => camReference;
        public Transform FollowTarget => followTarget;
        public bool AllowEdgeScroll {
            get => allowEdgeScroll;
            set {
                allowEdgeScroll = value;
                if(Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = allowEdgeScroll ? CursorLockMode.Confined : CursorLockMode.None;
            }
        }

        #endregion

        #region Init / Disable

        private void Awake() {
            camReference = cameraTransform.GetComponentInChildren<Camera>();
            targetDistance = cameraTransform.localPosition.z;
            tiltRotSpeedMultiplier = tiltSpeedMultiplier / rotationSpeedMultiplier;
        }

        private void OnEnable() {
            if(allowEdgeScroll)
                Cursor.lockState = CursorLockMode.Confined;
            if(Input.GetKey(rotation))
                rotationInputStack++;
            if(Input.GetKey(KeyCode.Mouse1))
                rotationInputStack++;
        }

        private void OnDisable() {
            rotationInputStack = 0;
            if(hideMouse) {
                if(Input.GetKey(rotation)) {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        #endregion

        #region Update

        private void Update() {
            if(hideMouse) {
                if(UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                    if(Input.GetKeyDown(rotation) || Input.GetKeyDown(KeyCode.Mouse1)) {
                        if(rotationInputStack == 0) {
                            Cursor.lockState = CursorLockMode.Locked;
                            Cursor.visible = false;
                        }
                        rotationInputStack++;
                        if(rotationInputStack > 2)
                            rotationInputStack = 2;
                    }
                }
                if(Input.GetKeyUp(rotation) || Input.GetKeyUp(KeyCode.Mouse1)) {
                    rotationInputStack--;
                    if(rotationInputStack == 0) {
                        Cursor.lockState = allowEdgeScroll ? CursorLockMode.Confined : CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    if(rotationInputStack < 0)
                        rotationInputStack = 0;
                }
                if(rotationInputStack > 0) {
                    if(!(Input.GetKey(rotation) || Input.GetKey(KeyCode.Mouse1))) {
                        rotationInputStack = 0;
                        Cursor.lockState = allowEdgeScroll ? CursorLockMode.Confined : CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
            }

            var keyRot = (Input.GetKey(rotateCameraLeft) ? 1f : 0f) + (Input.GetKey(rotateCameraRight) ? -1f : 0f);
            if(keyRot != 0f) {
                rotationTransform.localRotation *= Quaternion.Euler(0, keyRot * (rotationSpeedMultiplier), 0);
            }

            if(rotationInputStack > 0) {
                rotationTransform.localRotation *= Quaternion.Euler(0, (Input.GetAxisRaw("Mouse X")) * (rotationSpeedMultiplier), 0);

                var tilt = Toolkit.Mathematics.MathUtility.ClampRotation(tiltTransform.localRotation.eulerAngles.x + Input.GetAxisRaw("Mouse Y") * tiltSpeedMultiplier, tiltRange);
                tiltTransform.localRotation = Quaternion.Euler(tilt, 0, 0);
            }

            var pos = cameraTransform.localPosition;
            targetDistance = -zoomRange.Clamp(-targetDistance + Input.GetAxisRaw("Mouse ScrollWheel") * -zoomSpeedMultiplier);
            pos.z = targetDistance;
            if(raycastMaxDistance) {
                Ray ray = new Ray(tiltTransform.position, -tiltTransform.forward);
                var maxDistance = ray.HitDistance(zoomRange.max, environmentMask) - safetyDistance;
                if(maxDistance > Mathf.Epsilon && maxDistance < -pos.z) {
                    pos.z = -maxDistance;
                }
            }
            cameraTransform.localPosition = pos;
#if UNITY_EDITOR
            //recording quicktools for editor (will)
            if(functionKeys == true) {
                if(Input.GetKey(KeyCode.F1)) // Reset to default
                {
                    rotationSpeedMultiplier = 5f;
                    movementSpeed.min = 5f;
                    movementSpeed.max = 15f;
                    Cursor.visible = true;

                }
                if(Input.GetKey(KeyCode.F3)) // Rotate speed Low
                {
                    rotationSpeedMultiplier = 0.5f;
                }
                if(Input.GetKey(KeyCode.F4)) // Rotate speed High
                {
                    rotationSpeedMultiplier = 5f;
                }
                if(Input.GetKey(KeyCode.F5)) // Go Down Slowly
                {
                    transform.Translate(0f, -0.02f, 0f);
                }
                if(Input.GetKey(KeyCode.F6)) //  Go Up Slowly
                {
                    transform.Translate(0f, 0.02f, 0f);
                }
                if(Input.GetKey(KeyCode.F7)) // Go Down Quick
                {
                    transform.Translate(0f, -1f, 0f);
                }
                if(Input.GetKey(KeyCode.F8)) //  Go Up Quick
                {
                    transform.Translate(0f, 1f, 0f);
                }
                if(Input.GetKey(KeyCode.F9)) // Set movespeed to Slow
                {
                    movementSpeed.min = 1.5f;
                    movementSpeed.max = 1.5f;
                }
                if(Input.GetKey(KeyCode.F10)) // Set movespeed to Default/fast
                {
                    movementSpeed.min = 5f;
                    movementSpeed.max = 15f;
                }
                if(Input.GetKey(KeyCode.F11)) // Set movespeed to Superfast
                {
                    movementSpeed.min = 35f;
                    movementSpeed.max = 60f;
                }
                if(Input.GetKey(KeyCode.F12)) // hide cursor
                {
                    if(Cursor.visible == true) {
                        Cursor.visible = false;
                    }
                    else { Cursor.visible = true; }
                }
            }
#endif

            // Handle movement
            var vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized.ToVector3_XZ();
            if(allowEdgeScroll) {
                var w = Screen.width - 3;
                var h = Screen.height - 3;
                var inputPosition = Input.mousePosition;
                if(inputPosition.x < 2 && inputPosition.x > -1)
                    vector.x -= 1;
                else if(inputPosition.x > w && inputPosition.x < w + 5)
                    vector.x += 1;

                if(inputPosition.y < 2 && inputPosition.y > -1)
                    vector.z -= 1;
                else if(inputPosition.y > h && inputPosition.y < h + 5)
                    vector.z += 1;
            }

            bool hasFollow = followTarget != null;
            if(vector.sqrMagnitude > Mathf.Epsilon) {
                if(canBreakFollowTarget && hasFollow) {
                    followTarget = null;
                    hasFollow = false;
                }
            }

            if(!hasFollow) {
                movementTransform.localPosition += movementTransform.localRotation * vector * (Time.deltaTime * movementSpeed.Evaluate(zoomRange.InverseEvaluate(-pos.z)));
            }
            if(hasFollow) {
                switch(mode) {
                    case FollowMode.Constant:
                        movementTransform.position += (followTarget.position - movementTransform.position).ClampMagnitude(Time.deltaTime * speed);
                        break;
                    case FollowMode.Lerp:
                        movementTransform.position = Vector3.Lerp(movementTransform.position, followTarget.position, Time.deltaTime * speed);
                        break;
                    case FollowMode.Snap:
                        movementTransform.position = followTarget.position;
                        break;
                }
            }
        }

        #endregion

        #region Utility

        public void SetFollowTarget(Transform target) {
            if(allowFollowTarget)
                followTarget = target;
        }

        #endregion

        public enum FollowMode
        {
            Snap,
            Lerp,
            Constant,
        }
    }
}
