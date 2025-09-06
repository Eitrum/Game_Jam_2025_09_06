using Toolkit.Health;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour {
    public float damage = 0;
    public float force = 0;

    private void OnCollisionEnter(Collision collision) {
        var health = collision.gameObject.GetComponentInParent<IHealth>();
        if(health == null)
            return;

        health.Damage(1f);

        if(force > Mathf.Epsilon) {
            var body = collision.gameObject.GetComponentInParent<Rigidbody>();
            if(body)
                body.AddForce((collision.GetContact(0).point - transform.position).normalized * force, ForceMode.VelocityChange);
        }
    }
}
