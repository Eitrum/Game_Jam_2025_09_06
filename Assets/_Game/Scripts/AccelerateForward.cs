using UnityEngine;

public class AccelerateForward : MonoBehaviour {
    public Rigidbody body;
    public float acceleration = 5f;
    public float initialSpeed = 2f;

    void Start() {
        body = GetComponent<Rigidbody>();
        body.AddForce(transform.forward * initialSpeed, ForceMode.VelocityChange);
    }

    private void FixedUpdate() {
        body.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
    }
}
