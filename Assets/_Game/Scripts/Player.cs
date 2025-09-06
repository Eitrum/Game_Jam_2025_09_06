using UnityEngine;
using Toolkit;
using Toolkit.Audio;

namespace Game {
    public class Player : MonoBehaviour {

        public enum Direction {
            None,
            Left,
            Right,
        }

        [SerializeField] private float speed = 8f;
        [SerializeField] private float acceleration = 20;
        [SerializeField] private float sprintSpeed = 12f;

        [Header("Jump")]
        [SerializeField] private float jumpStrength = 4f;
        [SerializeField] private float horizontalJumpMultiplier = 0.25f;
        [SerializeField] private float holdDuration = 0.5f;
        [SerializeField] private float jumpHoldAcceleration = 4f;
        [SerializeField] private float fallAcceleration = 3f;
        [SerializeField] private AudioPlayer jumpSound;
        [SerializeField] private AudioPlayer landSound;

        [Header("Skin Orientation")]
        [SerializeField] private float rotationSpeed = 30;
        [SerializeField] private Transform skinRotation;

        public float CalculatedSpeed => sprinting ? sprintSpeed : speed;


        private Rigidbody body;
        private float inputHorizontal;
        private bool wantToJump;
        private bool isGrounded;
        private float timeSinceJumped = 0f;
        private bool didStartJump = false;
        private bool sprinting;
        private float airbornTime;

        private Direction attackDirection = Direction.Right;

        public Direction AttackDirection => attackDirection;

        private void Awake() {
            body = GetComponent<Rigidbody>();
           // body.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void Update() {
            inputHorizontal = Input.GetAxis("Horizontal");
            wantToJump = Input.GetKey(KeyCode.Space);
            sprinting = Input.GetKey(KeyCode.LeftShift);
        }

        private void FixedUpdate() {
            isGrounded = body.IsGrounded();
            timeSinceJumped += Time.deltaTime;

            if(!wantToJump) {
                didStartJump = false;
            }

            if(isGrounded) {
                if(wantToJump) {
                    timeSinceJumped = 0f;
                    body.AddForce(new Vector3(inputHorizontal * CalculatedSpeed * horizontalJumpMultiplier, jumpStrength, 0), ForceMode.VelocityChange);
                    didStartJump = true;
                    if(jumpSound)
                        jumpSound.PlayAt(transform.position);
                }

                var currentXVel = body.linearVelocity.x;
                var currentSpeed = Mathf.Abs(currentXVel);
                var targetXVel = inputHorizontal * CalculatedSpeed;
                if(Mathf.Approximately(currentSpeed, 0f)) {
                    body.AddForce(new Vector3(targetXVel, 0, 0), ForceMode.VelocityChange);
                }
                if(Mathf.Abs(targetXVel) < 0.05f) {
                    body.AddForce(new Vector3(((targetXVel - currentXVel) + (targetXVel - currentXVel)) * acceleration, 0, 0), ForceMode.Acceleration);
                }
                else
                    body.AddForce(new Vector3((targetXVel - currentXVel) * acceleration, 0, 0), ForceMode.Acceleration);
                if(airbornTime > 0.1f) {
                    if(landSound)
                        landSound.PlayAt(transform.position);
                }
                airbornTime = 0f;

            }
            else {
                if(!didStartJump || (timeSinceJumped > holdDuration)) {
                    body.AddForce(new Vector3(0, Physics.gravity.y * fallAcceleration, 0), ForceMode.Acceleration);
                }

                var currentXVel = body.linearVelocity.x;
                var currentSpeed = Mathf.Abs(currentXVel);
                var targetXVel = inputHorizontal * CalculatedSpeed;

                var slidProtection = Mathf.Clamp01(currentSpeed * 20f + 0.1f);

                body.AddForce(new Vector3((targetXVel - currentXVel) * acceleration * slidProtection, 0, 0), ForceMode.Acceleration);
                airbornTime += Time.deltaTime;
            }
            if(timeSinceJumped < holdDuration) {
                if(wantToJump && didStartJump)
                    body.AddForce(new Vector3(0, jumpHoldAcceleration, 0), ForceMode.Acceleration);

            }

            if(body.linearVelocity.x > 0.1f) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, 90, 0), Time.fixedDeltaTime * rotationSpeed);
                attackDirection = Direction.Right;
            }
            else if(body.linearVelocity.x < -0.1f) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, -90, 0), Time.fixedDeltaTime * rotationSpeed);
                attackDirection = Direction.Left;
            }
            else if(isGrounded && attackDirection == Direction.None) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, 180, 0), Time.fixedDeltaTime * rotationSpeed);
                //attackDirection = Direction.None;
            }
            //body.linearVelocity = new Vector3(targetXVel, body.linearVelocity.y);
        }

        [ContextMenu("ResetRotation")]
        public void ResetRotation() {
            //skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, 180, 0), Time.fixedDeltaTime * rotationSpeed);
            attackDirection = Direction.None;
        }
    }
}
