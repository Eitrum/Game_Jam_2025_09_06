using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Utility/Instantiate (OnTrigger)")]
    public class InstantiateOnTrigger : MonoBehaviour {
        #region Variables

        private const string TAG = "[InstantiateOnTrigger] - ";

        [SerializeField] private TriggerSources optionalSources;

        [SerializeField] private GameObject prefab = null;

        // Spawn Location
        [SerializeField] private TransformMask copyTransform = TransformMask.PositionRotation;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 rotationOffset;

        // Time
        [SerializeField, Min(0)] private float delay = 0f;

        private ITrigger trigger;

        #endregion

        #region Properties

        public float Delay {
            get => delay;
            set => delay = Mathf.Max(0f, value);
        }

        public GameObject Prefab {
            get => prefab;
            set => prefab = value;
        }

        #endregion

        #region Init

        private void Awake() {
            trigger = GetComponentInParent<ITrigger>();
#if UNITY_EDITOR
            if(trigger == null && optionalSources.IsAnyValid)
                Debug.LogError(TAG + $"No trigger found in parents of '{gameObject.name}'");
#endif
        }

        private void OnEnable() {
            if(trigger != null)
                trigger.OnTrigger += OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger += OnTrigger;
        }

        private void OnDisable() {
            if(trigger != null)
                trigger.OnTrigger -= OnTrigger;
            if(optionalSources != null)
                optionalSources.OnTrigger -= OnTrigger;
        }

        #endregion

        #region Trigger Callback

        [Button, ContextMenu("Trigger")]
        private void EditorTrigger() {
            using(var s = Source.Create("editor"))
                OnTrigger(s);
        }

        private void OnTrigger(Source source) {
            if(!prefab) {
                Debug.LogError(TAG + $"No prefab to instantiate on '{gameObject.name}'");
                return;
            }

            if(delay > Mathf.Epsilon) {
                var p = prefab;
                var pos = transform.position + (transform.rotation * offset);
                var rot = transform.rotation * Quaternion.Euler(rotationOffset);
                var sca = transform.lossyScale;

                Timer.Once(delay, () => Internal_Spawn(p, pos, rot, sca, copyTransform));
            }
            else
                Spawn();
        }

        public void Spawn()
            => Internal_Spawn(prefab,
                transform.position + (transform.rotation * offset),
                transform.rotation * Quaternion.Euler(rotationOffset),
                transform.lossyScale, copyTransform);

        private void Internal_Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, TransformMask mask) {
            var go = Instantiate(prefab);
            go.transform.Copy(position, rotation, scale, Space.World, mask);
        }

        #endregion
    }
}
