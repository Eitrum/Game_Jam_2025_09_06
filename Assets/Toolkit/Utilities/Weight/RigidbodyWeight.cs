using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Toolkit/Weight (Rigidbody)")]
    public class RigidbodyWeight : MonoBehaviour, IWeight {
        [SerializeField] private float weight = 1f;
        [SerializeField, Tooltip("Scale weight with the size of the object")] private bool scaleWeight = false;
        
        private Rigidbody body;

        public float Weight {
            get {
                if(scaleWeight) {
                    var s = transform.lossyScale;
                    return weight * (s.x * s.y * s.z);
                }
                return weight;
            }set {
                weight = value;
                if(scaleWeight) {
                    var s = transform.lossyScale;
                    body.mass = weight * (s.x * s.y * s.z);
                }
                else {
                    body.mass = weight;
                }
            }
        }

        void Awake() {
            body = GetComponent<Rigidbody>();
            body.mass = Weight;
        }

        void OnValidate() {
            var comps = GetComponents<IWeight>();
            if(comps != null && comps.Length > 1) {
                Debug.LogWarning($"{gameObject.name} contains multiple weight interfaces.");
            }
        }
    }
}
