using UnityEngine;
using Toolkit;
using Toolkit.Audio;
using Toolkit.Health;

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
        [SerializeField] private int maxJumps = 2;
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

        public static Player Instance;

        private Rigidbody body;
        private float inputHorizontal;
        private bool wantToJump;
        private bool isGrounded;
        private float timeSinceJumped = 0f;
        private bool didStartJump = false;
        private bool sprinting;
        private float airbornTime;
        private float uncontrollable;
        private IHealth health;
        private int jumps = 0;

        public Animator animator;

        private Direction attackDirection = Direction.Right;

        public Direction AttackDirection => attackDirection;

        private void Awake() {
            Instance = this;
            body = GetComponent<Rigidbody>();
            health = GetComponent<IHealth>();
            health.OnHealthChanged += OnHealthChanged;
            animator = GetComponentInChildren<Animator>();

            // body.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void OnHealthChanged(IHealth source, float oldHealth, float newHealth) {
            if(newHealth > oldHealth)
                return;
            uncontrollable = 0.1f;
        }

        private void Update() {
            inputHorizontal = Input.GetAxis("Horizontal");
            wantToJump = Input.GetKey(KeyCode.Space);
            sprinting = Input.GetKey(KeyCode.LeftShift);
            animator.SetFloat("horizontal", Mathf.Abs(inputHorizontal));
        }

        private void FixedUpdate() {
            var dt = Time.fixedDeltaTime;
            isGrounded = body.IsGrounded();
            timeSinceJumped += dt;

            if(uncontrollable > 0f) {
                uncontrollable -= dt;
                return;
            }

            if(!wantToJump) {
                didStartJump = false;
            }


            if(wantToJump && !didStartJump && timeSinceJumped > 0.3f && jumps < maxJumps) {
                timeSinceJumped = 0f;
                jumps++;
                body.linearVelocity = new Vector3(body.linearVelocity.x + inputHorizontal * CalculatedSpeed * horizontalJumpMultiplier, jumpStrength);
                //body.AddForce(new Vector3(, jumpStrength, 0), ForceMode.VelocityChange);
                didStartJump = true;
                if(jumpSound)
                    jumpSound.PlayAt(transform.position);

                animator.SetTrigger("fall");
            }

            if(isGrounded) {
                if(timeSinceJumped > 0.3f)
                    jumps = 0;
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
                if(airbornTime > 0.2f) {
                    animator.SetTrigger("land");
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
                airbornTime += dt;

                if(jumps == 0 && airbornTime > 0.2f)
                    jumps++;
            }
            if(timeSinceJumped < holdDuration) {
                if(wantToJump && didStartJump)
                    body.AddForce(new Vector3(0, jumpHoldAcceleration, 0), ForceMode.Acceleration);

            }

            if(body.linearVelocity.x > 0.1f) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, 90, 0), dt * rotationSpeed);
                attackDirection = Direction.Right;
            }
            else if(body.linearVelocity.x < -0.1f) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, -90, 0), dt * rotationSpeed);
                attackDirection = Direction.Left;
            }
            else if(isGrounded && attackDirection == Direction.None) {
                skinRotation.localRotation = Quaternion.Slerp(skinRotation.localRotation, Quaternion.Euler(0, 180, 0), dt * rotationSpeed);
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
