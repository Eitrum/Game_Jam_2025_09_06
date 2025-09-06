using System;
using UnityEngine;

namespace Toolkit {
    [Serializable]
    public struct PropertyEvent<T> {
        #region Variables

        [SerializeField] private T value;
        private Action<T> onChange;

        #endregion

        #region Properties

        public T Value {
            get => value;
            set {
                if(!this.value.Equals(value)) {
                    this.value = value;
                    onChange?.Invoke(value);
                }
            }
        }

        public event Action<T> OnChange {
            add => onChange += value;
            remove => onChange -= value;
        }

        #endregion

        #region Constructor

        public PropertyEvent(T value) {
            this.value = value;
            onChange = default;
        }

        #endregion

        #region Set Value

        public void SetValue(T newValue) {
            Value = newValue;
        }

        public void SetValue(T newValue, bool trigger) {
            if(trigger)
                Value = newValue;
            else
                value = newValue;
        }

        #endregion
    }
}
