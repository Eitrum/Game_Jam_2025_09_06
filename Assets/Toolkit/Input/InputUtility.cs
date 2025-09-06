using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.InputSystem
{
    public static class InputUtility
    {
        #region Get Axis

        public static float GetAxis(string axis)
            => Input.GetAxis(axis);

        public static float GetAxis(IEnumerable<string> enu) {
            float result = 0f;
            foreach(var e in enu)
                result += Input.GetAxis(e);
            return result;
        }

        public static float GetAxis(IReadOnlyList<string> axises) {
            float result = 0f;
            for(int i = axises.Count - 1; i >= 0; i--) {
                result += Input.GetAxis(axises[i]);
            }
            return result;
        }

        public static float GetAxisRaw(string axis)
            => Input.GetAxisRaw(axis);

        public static float GetAxisRaw(IEnumerable<string> enu) {
            float result = 0f;
            foreach(var e in enu)
                result += Input.GetAxisRaw(e);
            return result;
        }

        public static float GetAxisRaw(IReadOnlyList<string> axises) {
            float result = 0f;
            for(int i = axises.Count - 1; i >= 0; i--) {
                result += Input.GetAxis(axises[i]);
            }
            return result;
        }

        #endregion
    }
}
