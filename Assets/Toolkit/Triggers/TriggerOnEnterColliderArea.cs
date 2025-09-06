using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (Enter Collider Area)")]
    public class TriggerOnEnterColliderArea : MonoBehaviour, ITrigger {
        #region Variables

        [SerializeField] private bool repeatable = false;
        [SerializeField] private bool requiresEntity = false;
        [SerializeField] private EntityType entityFilter = EntityType.Player;

        private bool hasTriggered = false;

        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Properties

        public bool IsRepeatable => repeatable;
        public bool HasTriggered => hasTriggered;
        public bool RequiresEntity => requiresEntity;
        public EntityType EntityFilter => entityFilter;

        #endregion

        #region Init

#if UNITY_EDITOR
        void Awake() {
            var c = GetComponent<Collider>();
            var body = GetComponent<Rigidbody>();
            if(!c && !body) {
                Debug.LogError("[Trigger] - Unable to find trigger area!");
            }
        }
#endif

        #endregion

        #region Collider

        private void OnTriggerEnter(Collider other)
            => HandleTrigger(other.attachedRigidbody);

        private void OnTriggerEnter2D(Collider2D collision)
            => HandleTrigger(collision.attachedRigidbody);

        #endregion

        #region Handle Trigger

        [Button]
        public void CauseTrigger() {
            HandleTrigger(this);
        }

        public void CauseTrigger(Source source) {
            if(!repeatable && hasTriggered)
                return;

            hasTriggered = true;
            using(var t = Source.Create(this as ITrigger))
                OnTrigger?.Invoke(t);
        }

        private void HandleTrigger<T>(T body) where T : Component {
            if(!repeatable && hasTriggered)
                return;

            if(!body)
                return;

            if(requiresEntity) {
                var entity = body.GetComponentInParent<Entity>();
                if(!entity)
                    return;
                if(!entityFilter.HasFlag(entity.Type))
                    return;
            }

            hasTriggered = true;
            using(var t = body == this ? Source.Create(this as ITrigger) : Source.Create(body).AddChild(this as ITrigger))
                OnTrigger?.Invoke(t);
        }

        #endregion
    }
}
