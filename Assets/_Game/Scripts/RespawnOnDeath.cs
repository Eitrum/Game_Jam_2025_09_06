using System;
using Toolkit.Health;
using UnityEngine;

namespace Game {
    public class RespawnOnDeath : MonoBehaviour {

        private IHealth health;

        private void Awake() {
            health = GetComponent<IHealth>();
            health.OnDeath += OnDeath;
        }

        private void OnDeath() {
            transform.position = Checkpoints.lastUsedCheckpoint.transform.position;
            health.Restore(true);
        }
    }
}
