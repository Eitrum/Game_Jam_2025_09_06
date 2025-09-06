using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit
{
    public static class AnimationCurveExtensions
    {
        #region Clear

        public static void Clear(this AnimationCurve curve) {
            for(int i = curve.length - 1; i >= 0; i--)
                curve.RemoveKey(i);
        }

        #endregion
    }
}
