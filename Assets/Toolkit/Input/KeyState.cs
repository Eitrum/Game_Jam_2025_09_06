using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.InputSystem
{
    /// <summary>
    /// Class to handle conversion between Input.GetAxis to Input.GetKey(Down/Up)
    /// This makes Input system much more streamlined and easy to build for.
    /// </summary>
    public class KeyState
    {
        #region Variables

        public const float THRESHOLD = 0.05f;

        private float value;
        private int lastUpdate;

        private bool isHolding;
        private bool isUp;
        private bool isDown;

        #endregion

        #region Properties

        public bool IsUpdated => lastUpdate == Time.frameCount;

        public bool IsUp => isUp;
        public bool IsDown => isDown;
        public bool IsHold => isHolding;

        #endregion

        #region Constructor

        public KeyState() {
            lastUpdate = -1;
        }

        #endregion

        #region Update

        public void Update(string axis) {
            Update(Input.GetAxisRaw(axis));
        }

        public void Update(string axis, bool rawValue) {
            Update(rawValue ? Input.GetAxisRaw(axis) : Input.GetAxis(axis));
        }

        public void Update(IReadOnlyList<string> axises) {
            Update(InputUtility.GetAxisRaw(axises));
        }

        public void Update(IReadOnlyList<string> axises, bool rawValue) {
            Update(rawValue ? InputUtility.GetAxisRaw(axises) : InputUtility.GetAxis(axises));
        }

        public void Update(IEnumerable<string> axises) {
            Update(InputUtility.GetAxisRaw(axises));
        }

        public void Update(IEnumerable<string> axises, bool rawValue) {
            Update(rawValue ? InputUtility.GetAxisRaw(axises) : InputUtility.GetAxis(axises));
        }

        public void Update(float value) {
            // Only update once per frame!
            if(lastUpdate == Time.frameCount)
                return;

            isDown = false;
            isUp = false;

            if(isHolding) {
                if(value < this.value - THRESHOLD) {
                    isUp = true;
                    isHolding = false;
                    this.value = value;
                }
                else if(value > this.value)
                    this.value = value;
            }
            else {
                if(value > this.value + THRESHOLD) {
                    isHolding = true;
                    isDown = true;
                    this.value = value;
                }
                else if(value < this.value)
                    this.value = value;
            }

            this.value = value;
            lastUpdate = Time.frameCount;
        }

        #endregion

        #region Operator

        public static implicit operator bool(KeyState state) => state.IsHold;

        #endregion
    }
}
