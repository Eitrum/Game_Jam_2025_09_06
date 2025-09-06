using System;
using Toolkit;
using Toolkit.Health;
using UnityEngine;

namespace Game {
    public class Attack : MonoBehaviour {

        public float attackDelay = 0.1f;

        public float attackCooldown = 1.2f;
        public float range = 2f;

        private float cooldown = 0f;

        private Player player;

        private static Collider[] collidersCache = new Collider[32];

        private void Awake() {
            player = GetComponent<Player>();
        }

        private void Update() {
            if(cooldown > 0f) {
                cooldown -= Time.deltaTime;
                return;
            }
            if(Input.GetKey(KeyCode.K)) {
                var attack = player.AttackDirection;
                Timer.Once(attackDelay, () => DealDamage(attack));
            }
        }

        private void DealDamage(Player.Direction attackDirection) {
            if(attackDirection == Player.Direction.None) {
                return;
            }
            Vector3 offset = new Vector3(0, 1, 0);
            if(attackDirection == Player.Direction.Left) {
                offset.x = -range;
            }
            else if(attackDirection == Player.Direction.Right) {
                offset.x = range;
            }
            var hits = Physics.OverlapSphereNonAlloc(transform.position + offset, 0.5f, collidersCache);
            for(int i = 0; i < hits; i++) {
                var health = collidersCache[i].GetComponentInParent<IHealth>();
                if(health == null)
                    return;

                health.Damage(1f);
            }
        }
    }
}
