using Toolkit;
using UnityEngine;

namespace Game {
    public class RangedEnemies : MonoBehaviour {
        public Enemy enemy;

        public float range = 15f;
        private bool isInRange = false;

        public float cooldown = 5f;
        private float remainingCooldown = 0f;

        public float rotationSpeed = 45;

        public GameObject projectilePrefab;
        private Coroutine routine;

        private void OnDestroy() {
            Timer.Stop(routine);
        }

        private void Update() {
            var dt = Time.deltaTime;
            var distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
            if(distance < range) {
                if(!isInRange) {
                    isInRange = true;
                    Timer.Once(0.5f, () => enemy.TakeControl(0.1f), ref routine);
                    //enemy.enabled = false;
                }
            }
            else {
                if(isInRange) {
                    isInRange = false;
                    Timer.Once(0.5f, () => enemy.RevertControl(), ref routine);
                }
            }
            if(remainingCooldown > 0)
                remainingCooldown -= dt;

            if(!isInRange)
                return;


            var targetRotation = Quaternion.LookRotation((Player.Instance.transform.position + new Vector3(0, 1, 0)) - transform.position, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dt * rotationSpeed);

            if(remainingCooldown <= 0) {
                remainingCooldown = cooldown;
                var go = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.LookRotation((Player.Instance.transform.position + new Vector3(0, 1, 0)) - transform.position));
            }
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
