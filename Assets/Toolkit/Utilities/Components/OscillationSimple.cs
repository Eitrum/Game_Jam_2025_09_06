using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Utility/Oscillation (Simple)")]
    public class OscillationSimple : MonoBehaviour, IUpdate
    {
        #region Variables

        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private MinMax range = new MinMax(-90, 90);
        [SerializeField, Min(0.001f)] private float intervalDuration = 1f;
        [SerializeField] private bool smoothing = true;

        private TLinkedListNode<IUpdate> node;
        private Quaternion startRotation;
        private float timer = 0f;

        #endregion

        #region Properties

        bool INullable.IsNull => this == null;

        #endregion

        #region Unity Methods

        void Start() {
            startRotation = transform.localRotation;
            timer = range.InverseEvaluate(0f);
        }

        void OnEnable() {
            node = UpdateSystem.Subscribe(this as IUpdate);
        }

        void OnDisable() {
            UpdateSystem.Unsubscribe(node);
        }

        #endregion

        #region Update

        void IUpdate.Update(float dt) {
            timer = (timer + dt / intervalDuration) % 2f;
            var value = Mathf.PingPong(timer, 1f);
            if(smoothing)
                value = Mathematics.Ease.Quad.InOut(value);
            transform.localRotation = startRotation * Quaternion.AngleAxis(range.Evaluate(value), axis);
        }

        #endregion
    }
}
