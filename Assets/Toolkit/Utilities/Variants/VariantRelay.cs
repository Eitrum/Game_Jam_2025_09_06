using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [DefaultExecutionOrder(-2001)]
    [AddComponentMenu("Toolkit/Utility/Variants/Variant Relay")]
    public class VariantRelay : MonoBehaviour, IVariant
    {
        #region Variables

        [SerializeField] private bool activateAtAwake = true;
        [SerializeField, TypeFilter(typeof(IVariant))] private UnityEngine.Object[] variants = { };

        #endregion

        #region Properties

        public int VariantCount => 1;

        #endregion

        #region Unity Methods

        void Awake() {
            if(activateAtAwake)
                SetVariant();
        }

        #endregion

        #region Utility Methods

        public void SetVariant() {
            SetVariant(Toolkit.Mathematics.Random.Int);
        }

        public void SetVariant(int index) {
            foreach(var vari in variants)
                if(vari is IVariant ivar)
                    ivar.SetVariant(index);
        }

        #endregion
    }
}
