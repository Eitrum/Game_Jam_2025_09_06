using UnityEngine;
using Toolkit;

namespace Game {
    public class CameraAnchorFromInput : MonoBehaviour {

        [SerializeField] private Transform stickyJumpAnchor;
        [SerializeField] private Transform target;
        [SerializeField] private float range = 0f;
        [SerializeField] private float speed = 16f;

        [Header("Multiplier")]
        [SerializeField] private float xMultiplier = 1f;
        [SerializeField] private float yMultiplier = 1f;

        [Header("Falling Prediciton")]
        [SerializeField] private bool enablePrediction = false;
        [SerializeField] private float predictSpeed = 4f;
        [SerializeField] private float predictRange = 4f;

        private Vector2 current;
        private float lastGroundHeight;
        private float fallingCameraPrediction = 0f;

        private Rigidbody body;

        private void Awake() {
            body = GetComponent<Rigidbody>();
        }

        void Update() {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            var target = new Vector2(input.x * range * xMultiplier, input.y * range * yMultiplier);


            current = Vector2.MoveTowards(current, target, Time.deltaTime * speed);
            this.target.localPosition = current;


            if(enablePrediction) {
                var cameraHeight = lastGroundHeight;
                cameraHeight -= Mathf.Clamp01(fallingCameraPrediction) * predictRange;

                stickyJumpAnchor.position = new Vector3(body.transform.position.x, cameraHeight);
            }
        }

        private void FixedUpdate() {
            if(body.IsGrounded(0.1f)) {
                lastGroundHeight = body.position.y;
                fallingCameraPrediction = 0f;
            }

            if(body.linearVelocity.y < Mathf.Epsilon) {
                var groundCheck = new Ray(body.position + new Vector3(0, 0.4f, 0), Vector3.down);
                if(Physics.Raycast(groundCheck, out RaycastHit hit, predictRange + 0.4f)) {
                    lastGroundHeight = Mathf.Max(hit.point.y, body.position.y - 8);
                    fallingCameraPrediction = 0f;
                }
                else if(body.position.y < lastGroundHeight) {
                    lastGroundHeight = body.position.y;
                    fallingCameraPrediction += Time.deltaTime * predictSpeed;
                }
                else if(fallingCameraPrediction > 0.01f) {
                    fallingCameraPrediction += Time.deltaTime * predictSpeed;
                }
            }
        }
    }
}
