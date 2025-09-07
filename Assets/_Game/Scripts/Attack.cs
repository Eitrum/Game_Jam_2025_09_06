using System;
using Toolkit;
using Toolkit.Health;
using UnityEngine;

namespace Game {
    public class Attack : MonoBehaviour {

        public float attackDelay = 0.1f;
        public GameObject attackVfx;
        public GameObject hitVfx;

        public float attackCooldown = 1.2f;
        public float range = 2f;
        public float attackRadius = 1f;

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
            if(Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Mouse0)) {
                cooldown = attackCooldown;
                var attack = player.AttackDirection;
                var attackVfxPosition = GetWorldLocationWithOffset(attack);
                player.animator.SetTrigger("attack");
                if(attackVfx)
                    Instantiate(attackVfx, attackVfxPosition, attack == Player.Direction.Right ? Quaternion.identity : Quaternion.Euler(0, 180, 0));
                Timer.Once(attackDelay, () => DealDamage(attack));
            }
        }

        private Vector3 GetWorldLocationWithOffset(Player.Direction direction) {
            return transform.position + GetOffset(direction);
        }

        private Vector3 GetOffset(Player.Direction direction) {
            Vector3 offset = new Vector3(0, 1, 0);
            if(direction == Player.Direction.Left) {
                offset.x = -range;
            }
            else if(direction == Player.Direction.Right) {
                offset.x = range;
            }
            return offset;
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
            var attackLocation = GetWorldLocationWithOffset(attackDirection);
            var hits = Physics.OverlapSphereNonAlloc(attackLocation, attackRadius, collidersCache);
            for(int i = 0; i < hits; i++) {
                var health = collidersCache[i].GetComponentInParent<IHealth>();
                if(health == null)
                    continue;
                health.Damage(1f);

                if(hitVfx)
                    Instantiate(hitVfx, collidersCache[i].ClosestPoint(attackLocation), Quaternion.identity);
            }
        }
    }
}
