using System.Collections;
using System.Collections.Generic;
using Toolkit.UI;
using UnityEngine;

namespace Toolkit.Trigger {
    [AddComponentMenu("Toolkit/Trigger/Cause/Trigger - (Button Event)")]
    public class TriggerOnButtonEvent : MonoBehaviour, ITrigger {
        #region Variables

        [SerializeField] private bool repeatable = true;

        private IButton button;
        private UnityEngine.UI.Button unityButton;

        private bool triggered;

        public event OnTriggerDelegate OnTrigger;

        #endregion

        #region Properties

        public bool Repeatable {
            get => repeatable;
            set => repeatable = value;
        }

        public bool HasTriggered {
            get => triggered;
            set => triggered = value;
        }

        public IButton Button {
            get => button;
            set {
                if(button == value)
                    return;
                if(button != null)
                    button.OnClick -= CauseTrigger;
                button = value;
                if(button != null)
                    button.OnClick += CauseTrigger;
            }
        }

        public UnityEngine.UI.Button UnityButton {
            get => unityButton;
            set {
                if(unityButton == value)
                    return;
                if(unityButton != null)
                    unityButton.onClick.RemoveListener(CauseTrigger);
                unityButton = value;
                if(unityButton != null)
                    unityButton.onClick.AddListener(CauseTrigger);
            }
        }

        #endregion

        #region Init

        void Awake() {
            button = GetComponentInParent<IButton>();
            unityButton = GetComponentInParent<UnityEngine.UI.Button>();
        }

        void OnEnable() {
            if(button != null)
                button.OnClick += CauseTrigger;
            if(unityButton)
                unityButton.onClick.AddListener(CauseTrigger);
        }

        void OnDisable() {
            if(button != null)
                button.OnClick -= CauseTrigger;
            if(unityButton)
                unityButton.onClick.RemoveListener(CauseTrigger);
        }

        #endregion

        #region Itrigger Impl

        [Button]
        public void CauseTrigger() {
            using(var s = Source.Create(this as ITrigger))
                HandleTrigger(s);
        }

        public void CauseTrigger(Source source) => HandleTrigger(source.AddChild(this as ITrigger));

        private void HandleTrigger(Source source) {
            if(!repeatable && triggered)
                return;

            triggered = true;
            OnTrigger?.Invoke(source);

            source.Dispose();
        }

        #endregion
    }
}
