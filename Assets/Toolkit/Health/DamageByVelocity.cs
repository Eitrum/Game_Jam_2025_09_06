using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Health
{
    [AddComponentMenu("Toolkit/Health/Damage By Velocity")]
    public class DamageByVelocity : NullableBehaviour
    {
        #region Variables

        [SerializeField] ImpactMode mode = ImpactMode.Raycast;
        [SerializeField] private bool useImpulseVelocity = true;
        [SerializeField] private AnimationCurve damageByVelocity = AnimationCurve.Linear(0f, 10f, 50f, 100f);
        [SerializeField] private DamageType damageType = DamageType.None;
        [SerializeField] private bool destroyOnHit = false;

        // private object source; Figure out how to handle sources
        private Rigidbody body;
        private Vector3 lastFramesVelocity;

        #endregion

        #region Properties

        /* public object Source {
             get => source;
             set => source = value;
         } */

        public Damage CurrentDamage => new Damage(damageByVelocity.Evaluate(body.linearVelocity.magnitude), damageType.ToInt());

        #endregion

        #region Init

        private void Awake() {
            body = GetComponent<Rigidbody>();
        }

        #endregion

        #region Update / Collision

        private void FixedUpdate() {
            switch(mode) {
                case ImpactMode.Raycast: {
                        if(body.Raycast(out RaycastHit hit)) {
                            HandleHit(hit);
                        }
                    }
                    break;
                case ImpactMode.Sweep: {
                        if(body.SweepTest(out RaycastHit hit)) {
                            HandleHit(hit);
                        }
                    }
                    break;
                case ImpactMode.OnCollision:
                    if(!useImpulseVelocity)
                        lastFramesVelocity = body.linearVelocity;
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if(mode != ImpactMode.OnCollision)
                return;

            var health = collision.collider.GetComponentInParent<IHealth>();
            if(health != null) {
                var dmg = useImpulseVelocity ?
                    new DamageInstance(damageByVelocity.Evaluate((collision.impulse / Time.fixedDeltaTime).magnitude), damageType.ToInt()) :
                    new DamageInstance(damageByVelocity.Evaluate(lastFramesVelocity.magnitude), damageType.ToInt());

                health.Damage(dmg);
            }

            if(destroyOnHit)
                Destroy(this.gameObject, 0f);
        }

        private void HandleHit(RaycastHit hit) {
            var health = hit.collider.GetComponentInParent<IHealth>();
            if(health != null) {
                health.Damage(CurrentDamage.DamageData);
                if(destroyOnHit)
                    Destroy(this.gameObject, 0f);
            }
        }

        #endregion
    }

    public enum ImpactMode
    {
        None,
        Raycast,
        Sweep,
        OnCollision
    }
}
