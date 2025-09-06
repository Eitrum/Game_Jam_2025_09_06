using UnityEngine;

namespace Toolkit {
    [AddComponentMenu("Toolkit/Weight")]
    public class SimpleWeight : MonoBehaviour, IWeight {

        [SerializeField] private float weight = 1f;
        [SerializeField, Tooltip("Scale weight with the size of the object")] private bool scaleWeight = false;

        public float Weight {
            get {
                if(scaleWeight) {
                    var s = transform.lossyScale;
                    return weight * (s.x * s.y * s.z);
                }
                return weight;
            }
        }

        void OnValidate() {
            var comps= GetComponents<IWeight>();
            if(comps != null && comps.Length > 1) {
                Debug.LogWarning($"{gameObject.name} contains multiple weight interfaces.");
            }
        }
    }
}
