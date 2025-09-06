using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit
{
    public static class CurveExtensions
    {
        #region Multiply

        public static void Multiply(this AnimationCurve curve, float value) {
            var keys = curve.keys;
            for(int i = 0, length = keys.Length; i < length; i++) {
                keys[i].value *= value;
            }
            curve.keys = keys;
        }

        public static void Multiply(this AnimationCurve curve, AnimationCurve otherCurve) {
            var keys = curve.keys;
            for(int i = 0, length = keys.Length; i < length; i++) {
                keys[i].value *= otherCurve.Evaluate(keys[i].time);
            }
            curve.keys = keys;
        }

        #endregion

        #region Offset

        public static void OffsetValues(this AnimationCurve curve, float value) {
            var keys = curve.keys;
            for(int i = 0, length = keys.Length; i < length; i++) {
                keys[i].value += value;
            }
            curve.keys = keys;
        }

        public static void OffsetTime(this AnimationCurve curve, float value) {
            var keys = curve.keys;
            for(int i = 0, length = keys.Length; i < length; i++) {
                keys[i].time += value;
            }
            curve.keys = keys;
        }

        #endregion
    }
}
