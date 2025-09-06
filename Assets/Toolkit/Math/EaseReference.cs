using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public class EaseReference : ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] private Ease.Type type = Ease.Type.InOut;
        [SerializeField] private Ease.Function function = Ease.Function.Linear;

        [System.NonSerialized] private Ease.EaseFunction funcCache;

        #endregion

        #region Properties

        public float this[float input] => funcCache?.Invoke(input) ?? input;

        public Ease.Type Type {
            get => type;
            set => Set(value);
        }

        public Ease.Function Function {
            get => function;
            set => Set(value);
        }

        public System.Func<float, float> Func => Ease.GetEaseFunctionAsFunc(function, type);
        public Ease.EaseFunction EaseFunction => funcCache;

        #endregion

        #region Constructor

        public EaseReference() { }

        public EaseReference(Ease.Function function) : this(Ease.Type.InOut, function) { }

        public EaseReference(Ease.Function function, Ease.Type type) : this(type, function) { }

        public EaseReference(Ease.Type type, Ease.Function function) {
            this.type = type;
            this.function = function;
        }

        #endregion

        #region Set

        public void Set(Ease.Type type) {
            this.type = type;
            funcCache = Ease.GetEaseFunction(function, type);
        }

        public void Set(Ease.Function function) {
            this.function = function;
            funcCache = Ease.GetEaseFunction(function, type);
        }

        public void Set(Ease.Function function, Ease.Type type)
            => Set(type, function);

        public void Set(Ease.Type type, Ease.Function function) {
            this.type = type;
            this.function = function;
            this.funcCache = Ease.GetEaseFunction(function, type);
        }

        #endregion

        #region Evaluate

        public float Evaluate(float t) => funcCache?.Invoke(t) ?? t;

        #endregion

        #region ISerialization Impl

        public void OnAfterDeserialize() {
            funcCache = Ease.GetEaseFunction(function, type);
        }

        public void OnBeforeSerialize() { }

        #endregion
    }
}
